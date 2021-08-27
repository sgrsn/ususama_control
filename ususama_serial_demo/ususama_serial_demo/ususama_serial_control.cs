using System;
using ususama_serial;
using ususama_routes;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using System.Threading;

namespace ususama_serial
{

  public static class UsusamaManager
  {
    public enum ButtonState
    {
      Start,
      Pushed,
      Released,
    };
    private static ButtonState button = ButtonState.Start;

    public static UsusamaController ususama;
    private static System.Timers.Timer aTimer;
    public static UsusamaRoutes task_routes = new UsusamaRoutes();

    // SendPose, Move, WaitGoal各タスクのタイムアウトに使用するCTS
    private static CancellationTokenSource cts;

    // 停止ボタンが押されたときにOperationCanceledExceptionを発生させるCTS
    private static CancellationTokenSource cts_stop;

    public static List<Pose2D> completed_seq = new List<Pose2D>();

    public static void Setup(String com_port)
    {
      ususama = new UsusamaController(com_port);
      SetTimer();
    }

    public static void Resetup()
    {
      Console.WriteLine("Connection reset");
      aTimer.Dispose();
      ususama.CloseInterface();
      //await Task.Delay(100);
      ususama = new UsusamaController();
      SetTimer();
    }

    public static async Task RunSequenceWithStop(List<Pose2D> routes)
    {
      cts_stop = new CancellationTokenSource();
      try
      {
        await RunSequence(routes, cts_stop.Token);
      }
      catch (OperationCanceledException ce)
      {
        Console.WriteLine("Cancel with Stop");
      }
    }

    // 目標姿勢に移動する関数
    public static async Task RunSequence(List<Pose2D> routes, CancellationToken ct)
    {
      foreach (Pose2D ref_pose in routes)
      {
        Console.WriteLine("Run next task");
        if (ref_pose.state == CleanState.Home) button = ButtonState.Released;
        if (ref_pose.state == CleanState.Move)
        {
          // 順に実行
          ct.ThrowIfCancellationRequested();
          await SendPoseTask(ref_pose);
          ct.ThrowIfCancellationRequested();
          await SendMoveTask();
          ct.ThrowIfCancellationRequested();
          await WaitGoalTask();
          ct.ThrowIfCancellationRequested();

          // 実行したseqを保存
          completed_seq.Add(new Pose2D(ref_pose.x, ref_pose.y, ref_pose.theta, ref_pose.state));
          Console.WriteLine("Save seq");
        }
      }
      Console.WriteLine("Seq completed");
    }

    // 目標姿勢の送信
    // タイムアウトを2秒設定し、タイムアウトの場合再接続
    public static async Task SendPoseTask(Pose2D ref_pose)
    {
      Console.Write("   Pose task start... -> ");
      cts = new CancellationTokenSource();
      var timer = new System.Timers.Timer();
      timer.Interval = 2000;
      timer.Enabled = true;
      timer.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs e) => { Console.WriteLine("cancel pose task "); cts.Cancel(); });
      try
      {
        await SendRefposeAndWait(ref_pose, cts.Token);
      }
      catch (OperationCanceledException ce)
      {
        Resetup();
      }
      timer.Dispose();
      await Task.Delay(100);
      Console.WriteLine("   Pose task end");
    }

    public static async Task SendRefposeAndWait(Pose2D ref_pose, CancellationToken ct)
    {
      ususama.SendRefPose(ref_pose.x, ref_pose.y, ref_pose.theta);
      while (!ususama.IsCommandPoseCorrect(ref_pose.x, ref_pose.y, ref_pose.theta))
      {
        await Task.Delay(100);
        ususama.SendRefPose(ref_pose.x, ref_pose.y, ref_pose.theta);
        ct.ThrowIfCancellationRequested();
        if (button == ButtonState.Pushed) { Console.Write("   stop (pose task)"); return; }
      };
      Console.WriteLine("Move to x:{0}, y:{1}, theta:{2}", ref_pose.x, ref_pose.y, ref_pose.theta);
    }

    public static async Task SendMoveTask()
    {
      Console.Write("   Move task start... -> ");
      cts = new CancellationTokenSource();
      var timer = new System.Timers.Timer();
      timer.Interval = 2000;
      timer.Enabled = true;
      timer.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs e) => { Console.WriteLine("cancel move task"); cts.Cancel(); });
      try
      {
        await SendMoveAndWait(cts.Token);
        if (button == ButtonState.Pushed) { Console.Write("   stop (move task)"); return; }
      }
      catch (OperationCanceledException ce)
      {
        Resetup();
      }
      timer.Dispose();
      await Task.Delay(1000);
      Console.WriteLine("   Move task end");
    }
    public static async Task SendMoveAndWait(CancellationToken ct)
    {
      ususama.Move();
      while (!ususama.IsMoving())
      {
        await Task.Delay(100);
        ususama.Move();
        ct.ThrowIfCancellationRequested();
        if (ususama.IsReached()) break;
      };
      await Task.Delay(100);
    }

    public static async Task WaitGoalTask()
    {
      Console.Write("   Goal task start... -> ");
      cts = new CancellationTokenSource();
      var timer = new System.Timers.Timer();
      timer.Interval = 30000; //さすがに30秒たったらリスタート
      timer.Enabled = true;
      timer.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs e) => { Console.WriteLine("cancel goal task"); cts.Cancel(); });
      try
      {
        await WaitGoal(cts.Token);
      }
      catch (OperationCanceledException ce)
      {
        Resetup();
      }
      timer.Dispose();
      await Task.Delay(100);
      Console.WriteLine("   Goal task end");
    }

    public static async Task WaitGoal(CancellationToken ct)
    {
      while (!ususama.IsReached())
      {
        await Task.Delay(10);
        ct.ThrowIfCancellationRequested();
        if (button == ButtonState.Pushed) { Console.Write("   stop (goal task)"); return; }
      };
    }

    public static async Task SendStopTask()
    {
      Console.Write("   Stop task start -> ");
      // buttonの状態を変更
      button = ButtonState.Pushed;
      cts_stop.Cancel();

      // 送信用
      cts = new CancellationTokenSource();
      var timer = new System.Timers.Timer();
      timer.Interval = 2000;
      timer.Enabled = true;
      timer.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs e) => { Console.WriteLine("cancel stop task"); cts.Cancel(); });
      try
      {
        await SendStopAndWait(cts.Token);
      }
      catch (OperationCanceledException ce)
      {
        Resetup();
      }
      timer.Dispose();
      Console.WriteLine("   Stop task end");
    }
    public static async Task SendStopAndWait(CancellationToken ct)
    {
      ususama.Stop();
      while (!ususama.IsStopping())
      {
        await Task.Delay(100);
        ususama.Stop();
        ct.ThrowIfCancellationRequested();
      };
      await Task.Delay(1000);
    }

    public static async Task ReturnHome()
    {
      Console.WriteLine("Return Home");
      button = ButtonState.Released;
      // 完了したseqを逆にして処理する
      completed_seq.Reverse();
      // 最後に完了したseqはいらないはず そんなわけない
      //completed_seq.RemoveAt(0); 
      var copy_list = new List<Pose2D>(completed_seq);
      //await RunSequence(copy_list);
      await RunSequenceWithStop(copy_list);

      // 実行したseqを保存するリストを初期化
      completed_seq = new List<Pose2D>();
    }

    private static void SetTimer()
    {
      // Create a timer with a two second interval.
      aTimer = new System.Timers.Timer(10);
      // Hook up the Elapsed event for the timer. 
      aTimer.Elapsed += OnTimedEvent;
      aTimer.AutoReset = true;
      aTimer.Enabled = true;
    }

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
      ususama.ReceiveData();
      /*Console.WriteLine("{0}, {1}, {2}",
        ususama.current_pose_reply.x,
        ususama.current_pose_reply.y,
        ususama.current_pose_reply.theta
      );*/
    }

    private static void Close()
    {
      ususama.CloseInterface();
    }

  }
}