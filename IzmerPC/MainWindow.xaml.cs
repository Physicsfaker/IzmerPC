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
        private int connectAttempts = 3; //флаг того что был ответ на команду
        private int commandQueue = 0;

        int[] pArr1 = { 100, 6000, 10048, 12224, 14784, 16896, 19456, 21952, 23744, 27200, 33344, 36928, 38400, 40064, //arr
                                42170, 43728, 49152, 54208, 58944, 60096, 62656, 63360, 64448, 64789, 65132, 65472 };
        double[] pArr2 = { 0.01, 0.1, 2.1, 4.0, 7.0, 10.0, 15.0, 20.0, 25.0, 35.0, 60.0, 80.0, 90.0, 100.0, 130.0, 200.0, //ar1
                                300.0, 500.0, 800.0, 1000.0, 2000.0, 3000.0, 5700.0, 10000.0, 50000.0, 100000.0 };

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;

            #region COM_INIT and COM_EVENTS
            ComPort.NewDataRecived += rdata => WriteLog(rdata, true);
            ComPort.NewDataRecived += rdata => Manager(rdata);
            ComPort.NewDataTransfered += tdata => WriteLog(tdata, false);
            ComPortBox.Items.Add(Settings.Com);
            ComPortBox.SelectedIndex = 0;
            ComPort.InitComPort(Settings.Com, Settings.BaudRate);
            #endregion

            #region SendTimerInit
            System.Windows.Threading.DispatcherTimer interrogatorTimer = new System.Windows.Threading.DispatcherTimer();
            interrogatorTimer.Tick += new EventHandler(DataTransferTimer_Tick);
            interrogatorTimer.Interval = new TimeSpan(0, 0, 0, 0, 250); //инит таймера отправки: дни; часы; минуты; секунды; милсек.
            interrogatorTimer.Start();
            #endregion
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

        #region App Closing
        private void MainWindow_Closing(object sender, RoutedEventArgs args) //действия при закрытии приложения через меню
        {
            if (MessageBoxResult.No == MessageBox.Show("Вы действительно хотите закрыть программу?", "Закрытие клиента", MessageBoxButton.YesNo, MessageBoxImage.Warning)) return;
            ComPort.Close();
            App.Current.Shutdown();
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) //действия при закрытии приложения
        {
            if (MessageBoxResult.No == MessageBox.Show("Вы действительно хотите закрыть программу?", "Закрытие клиента", MessageBoxButton.YesNo, MessageBoxImage.Warning))
            {
                e.Cancel = true;
                return;
            }
            ComPort.Close();
            App.Current.Shutdown();
        }
        #endregion

        #region Windows
        private void ShowDebagWin(object sender, ExecutedRoutedEventArgs e) //метод на событие по вызову дебаг окна по нажатию клавишь
        {
            LogWindow secondWindow = new LogWindow();
            secondWindow.Show();
        }
        private void ShowSettingsWin(object sender, RoutedEventArgs args) //окно пользовательских настроек
        {
            SettingsWindow secondWindow = new SettingsWindow();
            secondWindow.Show();
        }
        private void ShowAboutProgramWin(object sender, RoutedEventArgs args) //окно о программе
        {
            AboutProgramWindow secondWindow = new AboutProgramWindow();
            secondWindow.Show();
        }
        #endregion

        private void DataTransferTimer_Tick(object sender, EventArgs e) //порядок опроса ТИ
        {
            if (ComPort.IsOpen)
            {
                switch (commandQueue)
                {
                    case 0: BytesOperations.State_pressure(); break;
                    case 1: BytesOperations.State_valve(); break;
                    //case 2: BytesOperations.State_electrometer(); break;
                    //case 3: BytesOperations.State_full(); break;
                    default: commandQueue = 0; goto case 0;
                }
            }
            if (connectAttempts >= 1 && connectAttempts <= 3) { ConnectLabel.Foreground = Brushes.Green; ConnectLabel.Content = "Связь: Установленна"; }
            if (connectAttempts > 0) connectAttempts--;
            else { ConnectLabel.Foreground = Brushes.Red; ConnectLabel.Content = "Связь: Отсутствует"; }
        }

        void Manager(byte[] reciveBytes)
        {
            if (reciveBytes.Length == 0) return; //защитка, хз почему-то иногда может прийти "ничего" ¯\_(ツ)_/¯

            #region 2F and 2D script
            if (reciveBytes[0] == 0x2D && reciveBytes.Length == 1)
            {
                ConnectLabel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                 {
                     connectAttempts = 8;
                     ConnectLabel.Foreground = Brushes.Yellow; ConnectLabel.Content = "Связь: подождите немного";
                 })); return;
            }
            if (reciveBytes[0] == 0x2F && reciveBytes.Length == 1)
            {
                ConnectLabel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                 {
                     connectAttempts = 3;
                     ConnectLabel.Foreground = Brushes.Green; ConnectLabel.Content = "Связь: Установленна"; BytesOperations.Trash_off();
                 })); return;
            }
            #endregion

            switch (commandQueue)
            {
                case 0:
                    if (reciveBytes[0] == 0xAB && reciveBytes.Length == 12)
                    {
                        double[] pressure = new double[2] { reciveBytes[8] + reciveBytes[9] * 256, reciveBytes[10] + reciveBytes[11] * 256 }; //хранит п1 и п2 соответственно

                        for (int n = 0; n < pressure.Length; n++)
                        {
                            if (pressure[n] < 100) pressure[n] = 100;
                            if (pressure[n] > 65471) pressure[n] = 65471;
                            for (int i = 0; i < pArr1.Length; i++)
                            {
                                if (pressure[n] < pArr1[i])
                                {
                                    pressure[n] = ((pArr2[i] - pArr2[i - 1]) / (pArr1[i] - pArr1[i - 1]) * pressure[n] + pArr2[i - 1]
                                        - (pArr2[i] - pArr2[i - 1]) / (pArr1[i] - pArr1[i - 1]) * pArr1[i - 1]);
                                    break;
                                }
                            }
                            if (pressure[n] > 90000) pressure[n] = 100000;
                        }

                        ConnectLabel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                        {
                            P1Label.Content = $"P1 = {pressure[0]:0.0}  Па";
                            P3Label.Content = $"P3 = {pressure[1]:0.0}  Па";
                        }));

                        commandQueue++; connectAttempts = 3;
                    }
                    break;  //State_pressure();
                case 1:
                    if (reciveBytes[0] == 0xAB && reciveBytes.Length == 5)
                    {

                    }
                    break;
                //case 2: BytesOperations.State_electrometer(); break;
                //case 3: BytesOperations.State_full(); break;
                //default: commandQueue = 0; goto case 0; break;
                default: break;
            }
        }

        private void WriteLog(byte[] reciveBytes, bool rxOrTx) //запись логов в файл
        {
            if (!Directory.Exists(@"logs"))
            {
                Directory.CreateDirectory(@"logs");
            }
            if (Settings.Logining)
                using (FileStream logWriter = new FileStream(@"logs\" + $"{DateTime.Now.ToString("dd_MMMM_yyyy")}_log.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    byte[] input;
                    string bytes = string.Join(" ", reciveBytes.Select(i => i.ToString("X2")));
                    if (rxOrTx) input = Encoding.Default.GetBytes($"{DateTime.Now.ToString("HH:mm:ss")} TI->PC: " + bytes + Environment.NewLine);
                    else input = Encoding.Default.GetBytes($"{DateTime.Now.ToString("HH:mm:ss")} PC->TI: " + bytes + Environment.NewLine);
                    logWriter.Write(input, 0, input.Length);
                }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //ComPort.Write(WriteBox.Text);
            byte[] X = { 0xAB, 0x10 };
            ComPort.Write(X);
        }

    }
}
