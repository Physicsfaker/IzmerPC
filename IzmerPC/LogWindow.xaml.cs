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
using System.Windows.Shapes;

namespace IzmerPC
{
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
            ComPort.NewDataRecived += rdata => AddDataInListBox(rdata, true);
            ComPort.NewDataTransfered += tdata => AddDataInListBox(tdata, false);
        }

        private void AddDataInListBox(byte[] data, bool rxOrTx)
        {
            if (!LogListBox.CheckAccess())
            {
                LogListBox.Dispatcher.Invoke(new Action<byte[], bool>(AddDataInListBox), data, rxOrTx);
            }
            else
            {
                string bytes = string.Join(" ", data.Select(i => i.ToString("X2")));
                if (rxOrTx) LogListBox.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} TI->PC: " + bytes);
                else LogListBox.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} PC->TI: " + bytes);

            }
        }
    }
}
