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
using ususama_serial;

namespace ususama_serial_demo
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    public UsusamaController ususama;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void button_Click(object sender, RoutedEventArgs e)
    {
      ususama = new UsusamaController();
    }

    private void button_Copy_Click(object sender, RoutedEventArgs e)
    {
      for (int i = 0; i < 30; i++)
      {
        ususama.demo(i*100, 10);
        ususama.ReceiveData();
      }
    }

    private void button2_Click(object sender, RoutedEventArgs e)
    {
      ususama.CloseInterface();
    }
  }
}
