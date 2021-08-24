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

    public MainWindow()
    {
      InitializeComponent();
      
    }

    private static void SetTimer()
    {
      // Create a timer with a two second interval.
      aTimer = new System.Timers.Timer(50);
      // Hook up the Elapsed event for the timer. 
      aTimer.Elapsed += OnTimedEvent;
      aTimer.AutoReset = true;
      aTimer.Enabled = true;
    }

    private static int tmp_i = 0;

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
      ususama.ReceiveData();
      //UpdateTextBlock();
      Console.WriteLine("{0}, {1}, {2}, ",
        ususama.register[UsusamaProtocol.REPLY_COMMAND_X],
        ususama.register[UsusamaProtocol.REPLY_COMMAND_Y],
        ususama.register[UsusamaProtocol.REPLY_COMMAND_THETA]
      );
    }

    private static void UpdateTextBlock()
    {
        mainWindow.command_x_reply_textblock.Text = ususama.register[UsusamaProtocol.REPLY_COMMAND_X].ToString();
        mainWindow.command_y_reply_textblock.Text = ususama.register[UsusamaProtocol.REPLY_COMMAND_Y].ToString();
        mainWindow.command_theta_reply_textblock.Text = ususama.register[UsusamaProtocol.REPLY_COMMAND_THETA].ToString();
        mainWindow.state_x_textblock.Text = ususama.register[UsusamaProtocol.REPLY_STATE_X].ToString();
        mainWindow.state_y_textblock.Text = ususama.register[UsusamaProtocol.REPLY_STATE_Y].ToString();
        mainWindow.state_theta_textblock.Text = ususama.register[UsusamaProtocol.REPLY_STATE_THETA].ToString();
    }

    private void connect_button_Click(object sender, RoutedEventArgs e)
    {
      ususama = new UsusamaController();
    }

    private void read_start_button_Click(object sender, RoutedEventArgs e)
    {
      SetTimer();
    }

    private void write_start_button_Click(object sender, RoutedEventArgs e)
    {

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
      ususama.Move();
    }

    private void stop_button_Click(object sender, RoutedEventArgs e)
    {
      ususama.Stop();
    }

  }
}
