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

    public MainWindow()
    {
      InitializeComponent();
    }

    // タスク(複数の移動を1つのタスクとする)たち
    private async void step1_button_Click(object sender, RoutedEventArgs e) { await UsusamaManager.RunSequenceWithStop(UsusamaManager.task_routes.move_seq_1); }
    private async void step2_button_Click(object sender, RoutedEventArgs e) { await UsusamaManager.RunSequenceWithStop(UsusamaManager.task_routes.move_seq_2); }
    private async void step3_button_Click(object sender, RoutedEventArgs e) { await UsusamaManager.RunSequenceWithStop(UsusamaManager.task_routes.move_seq_3); }
    private async void step4_button_Click(object sender, RoutedEventArgs e) { await UsusamaManager.RunSequenceWithStop(UsusamaManager.task_routes.move_seq_4); }
    private void step5_button_Click(object sender, RoutedEventArgs e) {  }
    private async void return_button_Click(object sender, RoutedEventArgs e) { await UsusamaManager.RunSequenceWithStop(UsusamaManager.task_routes.home_seq); }
    private void connect_button_Click(object sender, RoutedEventArgs e) { UsusamaManager.Setup("COM7"); }

    private async void stop_button_Click(object sender, RoutedEventArgs e) { await UsusamaManager.SendStopTask(); }

    private void read_start_button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void disconnect_button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void move_button_Click(object sender, RoutedEventArgs e)
    {
      //float x = Convert.ToSingle(x_textBox.Text);
      //float y = Convert.ToSingle(y_textBox.Text);
      //float theta = Convert.ToSingle(theta_textBox.Text);
      //ususama.SendRefPose(x, y, theta);
    }
    private void go_button_Click(object sender, RoutedEventArgs e)
    {
      //ususama.ReleaseStop();
      //ususama.Move();
    }

  }
}
