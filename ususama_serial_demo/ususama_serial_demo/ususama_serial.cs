using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace ususama_serial
{
  public class UsusamaController
  {
    private UsusamaInterface my_interface;
    private int[] register = new int[25];
    public UsusamaController()
    {
      my_interface = new UsusamaSerial("COM6", 115200);
    }

    // 推奨:タイマ割り込みなどで一定時間ごとに呼び出すこと
    // 受信割り込みはこわいのでやめておく
    public void ReceiveData()
    {
      byte[] buffer = UsusamaProtocol.ReceiveDataUntilHeader(my_interface);
      UsusamaProtocol.UsusamaData data_t = UsusamaProtocol.ProcessingReceivedData(buffer);
      if (data_t.valid)
      {
        register[data_t.reg] = data_t.data;
      }
    }

    // 目的姿勢を送る
    // マイコンはこれを受信すると次の指令までREPLY_POSE_X,Y,THETAに同じ値を返してくる
    // ちゃんと受信されたか返信を確認してからMove()で移動を許可するとよい
    public void SendRefPose(float x, float y, float theta)
    {
      UsusamaProtocol.UsusamaData data_t;

      data_t.data = UsusamaProtocol.EncodeFloat2Int(x);
      data_t.reg = UsusamaProtocol.COMMAND_POSE_X;
      data_t.valid = true;
      UsusamaProtocol.SendPacketData(my_interface, data_t);

      data_t.data = UsusamaProtocol.EncodeFloat2Int(y);
      data_t.reg = UsusamaProtocol.COMMAND_POSE_Y;
      data_t.valid = true;
      UsusamaProtocol.SendPacketData(my_interface, data_t);

      data_t.data = UsusamaProtocol.EncodeFloat2Int(theta);
      data_t.reg = UsusamaProtocol.COMMAND_POSE_THETA;
      data_t.valid = true;
      UsusamaProtocol.SendPacketData(my_interface, data_t);
    }

    // 目的姿勢への移動を許可する
    // マイコンはこれを受信すると次の指令までREPLY_STATE_X,Y,THETAに現在の姿勢を返してくる
    public void Move()
    {
      UsusamaProtocol.UsusamaData data_t;
      data_t.data = 1;
      data_t.reg = UsusamaProtocol.COMMAND_MOVE;
      data_t.valid = true;
      UsusamaProtocol.SendPacketData(my_interface, data_t);
    }

    // 移動を停止させる, 緊急停止の場合など
    // 再開はMove()を使用
    // ホームへ戻す場合は先にSendRefPose()してからMove()する
    // マイコンはこれを受信すると次の指令までREPLY_STATE_X,Y,THETAに現在の姿勢を返してくる
    public void Stop()
    {
      UsusamaProtocol.UsusamaData data_t;
      data_t.data = 1;
      data_t.reg = UsusamaProtocol.COMMAND_STOP;
      data_t.valid = true;
      UsusamaProtocol.SendPacketData(my_interface, data_t);
    }

    // Stopを解除する
    // Moveだけでも大丈夫
    public void ReleaseStop()
    {
      UsusamaProtocol.UsusamaData data_t;
      data_t.data = 0;
      data_t.reg = UsusamaProtocol.COMMAND_STOP;
      data_t.valid = true;
      UsusamaProtocol.SendPacketData(my_interface, data_t);
    }

    public void CloseInterface()
    {
      my_interface.Close();
    }
  }

  public static class UsusamaProtocol
  {
    private const byte HEAD_BYTE = 0x1D;
    private const byte ESCAPE_BYTE = 0x1E;
    private const byte ESCAPE_MASK = 0x1F;

    public const byte COMMAND_MOVE = 0x04;
    public const byte COMMAND_POSE_X = 0x05;
    public const byte COMMAND_POSE_Y = 0x06;
    public const byte COMMAND_POSE_THETA = 0x07;

    public const byte COMMAND_STOP = 0x08;

    public const byte REPLY_MOVE = 0x04;
    public const byte REPLY_POSE_X = 0x05;
    public const byte REPLY_POSE_Y = 0x06;
    public const byte REPLY_POSE_THETA = 0x07;
    public const byte REPLY_STOP = 0x08;

    public const byte REPLY_STATE_X = 0x010;
    public const byte REPLY_STATE_Y = 0x011;
    public const byte REPLY_STATE_THETA = 0x12;
    public struct UsusamaData
    {
      public int data;
      public byte reg;
      public bool valid;
    }

    public static int EncodeFloat2Int(float value)
    {
      return (int)(value * 1000);
    }

    public static void SendPacketData(UsusamaInterface my_interface, UsusamaData data_t)
    {
      byte[] dataBytes = new byte[4]
      {
        (byte)((data_t.data >> 24) & 0xFF),
        (byte)((data_t.data >> 16) & 0xFF),
        (byte)((data_t.data >>  8) & 0xFF),
        (byte)((data_t.data >>  0) & 0xFF)
      };

      List<byte> buffer_w = new List<byte>();

      buffer_w.Add(HEAD_BYTE);
      buffer_w.Add(data_t.reg);

      byte checksum = 0;
      for (int i = 0; i < 4; ++i)
      {
        if ((dataBytes[i] == ESCAPE_BYTE) || (dataBytes[i] == HEAD_BYTE))
        {
          buffer_w.Add(ESCAPE_BYTE);
          checksum += ESCAPE_BYTE;
          buffer_w.Add((byte)((int)dataBytes[i] ^ (int)ESCAPE_MASK));
          checksum += (byte)((int)dataBytes[i] ^ (int)ESCAPE_MASK);
        }
        else
        {
          buffer_w.Add(dataBytes[i]);
          checksum += dataBytes[i];
        }
      }
      // 末尾にチェックサムを追加で送信する
      buffer_w.Add(checksum);
      int size = buffer_w.Count - 1;
      buffer_w.Insert(1, (byte)size);

      byte[] buffer = buffer_w.ToArray();
      my_interface.SendBytes(buffer, buffer_w.Count);
    }

    public static byte[] ReceiveDataUntilHeader(UsusamaInterface my_interface)
    {
      byte indata = my_interface.ReadOneByteData();
      // HEAD_BYTE以外は無視
      if (indata != UsusamaProtocol.HEAD_BYTE)
      {
        return null;
      }
      // HEAD_BYTEを受信したらパケットを受信
      else if (indata == HEAD_BYTE)
      {
        int size = my_interface.ReadOneByteData();
        byte[] buffer = new byte[16];
        if (size < buffer.Length)
        {
          my_interface.ReadBytes(buffer, size);
        }
        return buffer;
      }
      return null;
    }

    public static UsusamaData ProcessingReceivedData(byte[] buffer)
    {
      if (buffer == null)
      {
        UsusamaData data_t;
        data_t.data = 0;
        data_t.reg = 0;
        data_t.valid = false;
        return data_t;
      }
      int index = 0;
      byte reg = buffer[0];
      index++;
      byte checksum = 0;

      checksum += reg;
      byte[] bytes = new byte[4] { 0, 0, 0, 0 };

      for (int i = 0; i < 4; ++i)
      {
        byte d = buffer[index++];
        if (d == ESCAPE_BYTE)
        {
          byte nextByte = buffer[index++];
          bytes[i] = (byte)((int)nextByte ^ (int)ESCAPE_MASK);
          checksum += (byte)((int)d + (int)nextByte);
        }
        else
        {
          bytes[i] = d;
          checksum += d;
        }
      }

      byte checksum_recv = buffer[index++];
      int DATA = 0x00;
      for (int i = 0; i < 4; i++)
      {
        DATA |= (((int)bytes[i]) << (24 - (i * 8)));
      }

      if (checksum == checksum_recv)
      {
        UsusamaData data_t;
        data_t.data = DATA;
        data_t.reg = reg;
        data_t.valid = true;
        return data_t;
      }
      else
      {
        // data error
        Console.WriteLine("data error, checksum is wrong.");
        UsusamaData data_t;
        data_t.data = 0;
        data_t.reg = 0;
        data_t.valid = false;
        return data_t;
      }

    }
  }

  // Serialなどのインターフェースを抽象化したclass
  public class UsusamaInterface
  {
    public UsusamaInterface()
    {

    }

    public virtual void DiscardInBuffer() { }
    public virtual void Close() { }
    public virtual void SendByteData(byte data) { }
    public virtual void SendBytes(byte[] buffer, int size) { }

    public virtual byte ReadOneByteData() { return 0; }
    public virtual void ReadBytes(byte[] buffer, int size) { }
  }


  // UsusamaInterfaceを継承したSerialの実体class
  public class UsusamaSerial : UsusamaInterface
  {
    public static SerialPort port;
    public UsusamaSerial(string port_name, int baud_rate)
    {
      port = new SerialPort(port_name, baud_rate, Parity.None, 8, StopBits.One);
      port.ReadBufferSize = 64;
      try
      {
        port.Open();
        port.DtrEnable = true;
        port.RtsEnable = true;
        Console.WriteLine("Connected.");
      }
      catch (Exception err)
      {
        Console.WriteLine("Unexpected exception : {0}", err.ToString());
      }
    }

    public override void Close()
    {
      port.Close();
      port.Dispose();
    }

    public override void DiscardInBuffer()
    {
      port.DiscardInBuffer();
    }

    public override byte ReadOneByteData()
    {
      try
      {
        int data = port.ReadByte();
        if (data < 0)
        {
          return 0; // fault
        }
        return (byte)data;
      }
      catch (System.IO.IOException e)
      {
        return 0;
      }
    }
    public override void ReadBytes(byte[] buffer, int size)
    {
      port.Read(buffer, 0, size);
    }
    public override void SendByteData(byte data)
    {
      WriteOneByteData(data);
    }
    public override void SendBytes(byte[] buffer, int size)
    {
      port.Write(buffer, 0, size);
    }

    private void WriteOneByteData(byte data)
    {
      byte[] buffer = { data, 0 };
      port.Write(buffer, 0, 1);
    }

  }
}
