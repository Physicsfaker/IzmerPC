using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IzmerPC
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //ComPortBox.Items.Add(ComPort.CurrentPort);
            //ComPortBox.SelectedItem = ComPort.CurrentPort;
            ComPortBox.Items.Add("COM5");
            ComPortBox.SelectedItem = "COM5";
            ComPort.InitComPort(ComPortBox.SelectedItem.ToString(), 115200);
            ComPort.NewDataRecived += x => WriteLog(x);
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) //действия при закрытии приложения
        {
            ComPort.Close();
            App.Current.Shutdown();
        }

        void WriteLog(string message)
        {
            if (!InfoWindow.CheckAccess())
            {
                InfoWindow.Dispatcher.Invoke(new Action<string>(WriteLog), message);
            }
            else 
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message);

                foreach (byte item in bytes)
                {
                    InfoWindow.Items.Add(Convert.ToString(item, 16).ToUpper());
                }
            }
        }

        #region ComPortBox_Events
        private void ComPortBox_Opened(object sender, EventArgs e)
        {
            ComPortBox.Items.Clear();
            ComPortBox.SelectedItem = ComPort.CurrentPort;

            foreach (string s in ComPort.GetPorts())
            {
                ComPortBox.Items.Add($"{s}");
            }
        }

        private void ComPortBox_Closed(object sender, EventArgs e)
        {
            if (ComPortBox.SelectedItem == null) return;
            ComPort.InitComPort(ComPortBox.SelectedItem.ToString(), 115200);
        }
        #endregion


        private  void Button_Click(object sender, RoutedEventArgs e)
        {
            LogWindow secondWindow = new LogWindow();
            secondWindow.Show();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //ComPort.Write(WriteBox.Text);
            byte[] X = {0xAB, 0x00, 0xFB};
            ComPort.Write(X);
        }
    }
}