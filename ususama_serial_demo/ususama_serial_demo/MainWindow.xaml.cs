using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using ususama_serial;
using ususama_routes;

namespace ususama_serial_demo
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    public static UsusamaController ususama;
    private static System.Timers.Timer aTimer;
    public static MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
    private static UsusamaRoutes my_routes = new UsusamaRoutes();

    public MainWindow()
    {
      InitializeComponent();
    }

    // Step 0: シリアル接続、受信タイマーの設定
    public static void Setup()
    {
      ususama = new UsusamaController();
      SetTimer();
    }

    // タスク(複数の移動を1つのタスクとする)たち
    private void step1_button_Click(object sender, RoutedEventArgs e) { RunSequence(my_routes.move_seq_test); }
    private void step2_button_Click(object sender, RoutedEventArgs e) { RunSequence(my_routes.move_seq_2); }
    private void step3_button_Click(object sender, RoutedEventArgs e) { RunSequence(my_routes.move_seq_3); }
    private void step4_button_Click(object sender, RoutedEventArgs e) { RunSequence(my_routes.move_seq_4); }
    private void step5_button_Click(object sender, RoutedEventArgs e) { RunSequence(my_routes.move_seq_5); }

    // 目標姿勢に移動する関数
    public static async void RunSequence(List<Pose2D> routes)
    {
      foreach (Pose2D ref_pose in routes)
      {
        if (ref_pose.state == CleanState.Move)
        {
          ususama.SendRefPose(ref_pose.x, ref_pose.y, ref_pose.theta);
          await Task.Delay(1000);
          ususama.Move();
          await Task.Delay(1000);
          while (!ususama.IsReachedGoal())
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

    private void connect_button_Click(object sender, RoutedEventArgs e)
    {
      ususama = new UsusamaController();
    }

    private void read_start_button_Click(object sender, RoutedEventArgs e)
    {
      SetTimer();
    }

    private void disconnect_button_Click(object sender, RoutedEventArgs e)
    {
      ususama.CloseInterface();
    }

    private void move_button_Click(object sender, RoutedEventArgs e)
    {
      float x = Convert.ToSingle(x_textBox.Text);
      float y = Convert.ToSingle(y_textBox.Text);
      float theta = Convert.ToSingle(theta_textBox.Text);
      ususama.SendRefPose(x, y, theta);
    }
    private void go_button_Click(object sender, RoutedEventArgs e)
    {
      ususama.ReleaseStop();
      ususama.Move();
    }

    private void stop_button_Click(object sender, RoutedEventArgs e)
    {
      ususama.Stop();
    }
  }
}
