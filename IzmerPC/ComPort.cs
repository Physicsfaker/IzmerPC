﻿using System;
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

        public delegate void DataRxTxMetods(byte[] packg);
        public static event DataRxTxMetods NewDataRecived;
        public static event DataRxTxMetods NewDataTransfered;

        static ComPort()
        {
            currentSerial = new SerialPort();
            currentSerial.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        public static void InitComPort(string text, int baudRate) //задает номер порта и скорость
        {
            if (currentSerial.IsOpen) currentSerial.Close();

            currentSerial.PortName = text;
            currentSerial.BaudRate = baudRate;
            currentSerial.Parity = Parity.None;
            currentSerial.DataBits = 8;
            currentSerial.StopBits = StopBits.One;
            currentSerial.Handshake = Handshake.None;
            currentSerial.RtsEnable = true;
            currentSerial.ReadTimeout = 500;
            currentSerial.WriteTimeout = 500;

            try { currentSerial.Open(); }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show($"Доступ к порту '{CurrentPort}' закрыт.");
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show($"Порт '{CurrentPort}' не существует.");
            }
        }

        private static async void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (!IsOpen) return;
            NewDataRecived(packg: await Task.Run(() => {
                byte[] reciveBytes = new byte[currentSerial.BytesToRead];
                while (currentSerial.BytesToRead > 0) { currentSerial.Read(reciveBytes, 0, currentSerial.BytesToRead); }
                return reciveBytes;
            }));
        }

        public static string CurrentPort => currentSerial.PortName.ToString(); //получить имя текущего порта
        public static string[] GetPorts() => SerialPort.GetPortNames();          // получить список всех доступных портов
        public static bool IsOpen => currentSerial.IsOpen;
        public static void Close() => currentSerial.Close();
        public static void Write(byte[] bytes)
        {
            try { currentSerial.Write(bytes, 0, bytes.Length); }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show($"Доступ к порту '{CurrentPort}' закрыт.");
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show($"Порт '{CurrentPort}' не существует.");
            }
            NewDataTransfered(bytes);
        }
    }
}
