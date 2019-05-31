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
using System.IO;

namespace IzmerPC
{

    public partial class MainWindow : Window
    {
        public static RoutedCommand DebugKey_Command = new RoutedCommand(); //событие по вызову дебаг окна по нажатию клавишь

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            ComPort.NewDataRecived += rdata => WriteLog(rdata, true);
            ComPort.NewDataTransfered += tdata => WriteLog(tdata, false);

            ComPortBox.Items.Add(Settings.Com);
            ComPortBox.SelectedIndex = 0;
            ComPort.InitComPort(Settings.Com, Settings.BaudRate);
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
            Settings.Com = ComPortBox.SelectedItem.ToString();
            ComPort.InitComPort(Settings.Com, Settings.BaudRate);
        }
        #endregion

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) //действия при закрытии приложения
        {
            ComPort.Close();
            App.Current.Shutdown();
        }
        private void ShowDebagWin(object sender, ExecutedRoutedEventArgs e) //метод на событие по вызову дебаг окна по нажатию клавишь
        {
            LogWindow secondWindow = new LogWindow();
            secondWindow.Show();
        }
        private void WriteLog(byte[] reciveBytes, bool rxOrTx) //запись логов в файл
        {
            if (!Directory.Exists(@"logs"))
            {
                Directory.CreateDirectory(@"logs");
            }
            using (StreamWriter logWriter = new StreamWriter(@"logs\"+$"{DateTime.Now.ToString("dd_MMMM_yyyy")}_log.txt", true, Encoding.Default))
            {
                string bytes = string.Join(" ", reciveBytes.Select(i => i.ToString("X2")));
                if (rxOrTx) logWriter.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} TI->PC: " + bytes);
                else logWriter.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} PC->TI: " + bytes);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //ComPort.Write(WriteBox.Text);
            byte[] X = { 0xAB, 0x00, 0xFB };
            ComPort.Write(X);
        }


    }
}