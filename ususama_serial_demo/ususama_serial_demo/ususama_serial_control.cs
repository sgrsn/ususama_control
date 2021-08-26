using System;
using ususama_serial;
using ususama_routes;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;

namespace ususama_serial
{
  public static class UsusamaManager
  {
    public static UsusamaController ususama;
    private static System.Timers.Timer aTimer;
    public static UsusamaRoutes task_routes = new UsusamaRoutes();

    public static void Setup()
    {
      ususama = new UsusamaController();
      SetTimer();
    }

    // 目標姿勢に移動する関数
    public static async void RunSequence(List<Pose2D> routes)
    {
      foreach (Pose2D ref_pose in routes)
      {
        if (ref_pose.state == CleanState.Move)
        {
          ususama.SendRefPose(ref_pose.x, ref_pose.y, ref_pose.theta);
          while (!ususama.IsCommandPoseCorrect(ref_pose.x, ref_pose.y, ref_pose.theta))
          {
            await Task.Delay(10);
          };
          ususama.Move();
          while (!ususama.IsMoving())
          {
            await Task.Delay(10);
          };
          while (!ususama.IsReached())
          {
            await Task.Delay(10);
          };
        }
      }
      Console.WriteLine("Seq completed");
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
      /*Console.WriteLine("{0}, {1}, {2}, {3}",
        ususama.current_pose_reply.x,
        ususama.current_pose_reply.y,
        ususama.current_pose_reply.theta,
        ususama.move_commmand_reply.reached
      );*/
    }

    private static void Close()
    {
      ususama.CloseInterface();
    }

  }
}