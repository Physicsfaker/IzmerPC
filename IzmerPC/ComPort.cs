using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IzmerPC
{

    public static class ComPort
    {
        private static SerialPort currentSerial { get; set; } //хранит ком порт

        public delegate void DataReadMetods(string packg);
        public static event DataReadMetods NewDataRecived;

        static ComPort()
        {
            currentSerial = new SerialPort();
            currentSerial.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        public static void InitComPort(string text, int baudRate) //задает номер порта и скорость
        {
            if (currentSerial.IsOpen) currentSerial.Close();

            currentSerial.Close();
            currentSerial.PortName = text;
            currentSerial.BaudRate = baudRate;
            currentSerial.Parity = Parity.None;
            currentSerial.DataBits = 8;
            currentSerial.StopBits = StopBits.One;
            currentSerial.Handshake = Handshake.None;
            currentSerial.RtsEnable = true;
            ReadTimeout = 500;
            WriteTimeout = 500;

            try { currentSerial.Open(); }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show($"Доступ к порту '{CurrentPort}' закрыт.");
            }
        }

        private static async void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Task<string> comReadTask = ComReadTaskMetod();
            NewDataRecived(await comReadTask); 
        }

        static Task<string> ComReadTaskMetod()
        {
            return Task.Run(() => 
            {
                var mess = "";
                if (!IsOpen) return mess;
                while (currentSerial.BytesToRead > 0)
                {
                    mess = Read();
                    //Task.Delay(200); // give the device time to send data
                }
                return mess;
            });
        }

        public static int ReadTimeout //время чтения
        {
            get => currentSerial.ReadTimeout;
            set
            {
                if (value > 0 && value < 1000) currentSerial.ReadTimeout = value;
                if (value < 0) currentSerial.ReadTimeout = 1;
                if (value > 1000) currentSerial.ReadTimeout = 1000;
            }
        }
        public static int WriteTimeout //время отправки
        {
            get => currentSerial.WriteTimeout;
            set
            {
                if (value > 0 && value < 1000) currentSerial.WriteTimeout = value;
                if (value < 0) currentSerial.WriteTimeout = 1;
                if (value > 1000) currentSerial.WriteTimeout = 1000;
            }
        }
        public static string CurrentPort => currentSerial.PortName.ToString(); //получить имя текущего порта
        public static string[] GetPorts() => SerialPort.GetPortNames();          // получить список всех доступных портов
        public static bool IsOpen => currentSerial.IsOpen;
        public static void Close() => currentSerial.Close();
        public static string Read() => currentSerial.ReadExisting();
        public static void Write(string bytes) => currentSerial.Write(bytes);
        public static void Write(byte[] bytes) => currentSerial.Write(bytes, 0, bytes.Length);
    }
}
