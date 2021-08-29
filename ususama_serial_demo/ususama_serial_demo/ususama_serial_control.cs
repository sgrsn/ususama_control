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
    private static System.Timers.Timer timeout_timer;
    public static UsusamaRoutes task_routes = new UsusamaRoutes();
    private static bool current_seq_completed = false;
    private static string port_name = "";

    // 停止ボタンが押されたときにOperationCanceledExceptionを発生させるCTS
    private static CancellationTokenSource cts_stop;

    public static List<Pose2D> completed_seq = new List<Pose2D>();

    public static void Setup(string com_port)
    {
      port_name = com_port;
      ususama = new UsusamaController(com_port);
      SetTimer();
    }

    public static void ResetOdometry()
    {
      // 待機処理はとりあえずなし
      ususama.ResetOdometry();
    }

    public static bool IsSeqCompleted()
    {
      return current_seq_completed;
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
      current_seq_completed = false;
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
      current_seq_completed = true;
    }

    // 目標姿勢の送信
    public static async Task SendPoseTask(Pose2D ref_pose)
    {
      Console.Write("   Pose task start... -> ");
      await SendRefposeAndWait(ref_pose);
      await Task.Delay(100);
      Console.WriteLine("   Pose task end");
    }

    public static async Task SendRefposeAndWait(Pose2D ref_pose)
    {
      ususama.SendRefPose(ref_pose.x, ref_pose.y, ref_pose.theta);
      while (!ususama.IsCommandPoseCorrect(ref_pose.x, ref_pose.y, ref_pose.theta))
      {
        await Task.Delay(100);
        ususama.SendRefPose(ref_pose.x, ref_pose.y, ref_pose.theta);
        if (button == ButtonState.Pushed) { Console.Write("   stop (pose task)"); return; }
      };
      Console.WriteLine("Move to x:{0}, y:{1}, theta:{2}", ref_pose.x, ref_pose.y, ref_pose.theta);
    }

    public static async Task SendMoveTask()
    {
      Console.Write("   Move task start... -> ");
      await SendMoveAndWait();
      await Task.Delay(1000);
      Console.WriteLine("   Move task end");
    }
    public static async Task SendMoveAndWait()
    {
      ususama.Move();
      while (!ususama.IsMoving())
      {
        await Task.Delay(100);
        ususama.Move();
        if (button == ButtonState.Pushed) { Console.Write("   stop (move task)"); return; }
        // すでにゴールしている可能性
        if (ususama.IsReached()) break;
      };
      await Task.Delay(100);
    }

    public static async Task WaitGoalTask()
    {
      Console.Write("   Goal task start... -> ");
      await WaitGoal();
      await Task.Delay(100);
      Console.WriteLine("   Goal task end");
    }

    public static async Task WaitGoal()
    {
      while (!ususama.IsReached())
      {
        await Task.Delay(100);
        if (button == ButtonState.Pushed) { Console.Write("   stop (goal task)"); return; }
      };
    }
    public static async Task SendStopTask()
    {
      Console.Write("   Stop task start -> ");
      // buttonの状態を変更
      button = ButtonState.Pushed;
      cts_stop.Cancel();
      await SendStopAndWait();
      Console.WriteLine("   Stop task end");
    }
    public static async Task SendStopAndWait()
    {
      ususama.Stop();
      while (!ususama.IsStopping())
      {
        await Task.Delay(100);
        ususama.Stop();
      };
      await Task.Delay(1000);
    }

    public static void ButtonRelease()
    {
      button = ButtonState.Released;
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
      timeout_timer = new System.Timers.Timer(5000);
      timeout_timer.Elapsed += ResetTimerOnTimedEvent;
      timeout_timer.AutoReset = true;
      timeout_timer.Enabled = true;
    }

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
      ususama.ReceiveData();
    }
    private static void ResetTimerOnTimedEvent(Object source, ElapsedEventArgs e)
    {
      if (!ususama.is_received)
      {
        Resetup();
      }
      ususama.is_received = false;
    }

    public static void Resetup()
    {
      Console.WriteLine("Connection reset");
      aTimer.Dispose();
      timeout_timer.Dispose();
      ususama.CloseInterface();
      ususama = new UsusamaController(port_name);
      SetTimer();
    }

    private static void Close()
    {
      ususama.CloseInterface();
    }

  }
}