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
    private static System.Timers.Timer aTimer2;

    public MainWindow()
    {
      InitializeComponent();
    }

    private static void SetTimer()
    {
      // Create a timer with a two second interval.
      aTimer = new System.Timers.Timer(500);
      // Hook up the Elapsed event for the timer. 
      aTimer.Elapsed += OnTimedEvent;
      aTimer.AutoReset = true;
      aTimer.Enabled = true;
    }
    private static void SetTimer2()
    {
      // Create a timer with a two second interval.
      aTimer2 = new System.Timers.Timer(50);
      // Hook up the Elapsed event for the timer. 
      aTimer2.Elapsed += OnTimedEvent2;
      aTimer2.AutoReset = true;
      aTimer2.Enabled = true;
    }

    private static int tmp_i = 0;

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
      tmp_i++;
      ususama.Demo(1907997, 10);
      Console.Write("send {0}, ", tmp_i);
    }
    private static void OnTimedEvent2(Object source, ElapsedEventArgs e)
    {
      ususama.ReceiveData();
    }

    private void button_Click(object sender, RoutedEventArgs e)
    {
      ususama = new UsusamaController();
      SetTimer();
    }

    private void button_Copy_Click(object sender, RoutedEventArgs e)
    {
      SetTimer2();
    }

    private void button2_Click(object sender, RoutedEventArgs e)
    {
      ususama.CloseInterface();
    }
  }
}
