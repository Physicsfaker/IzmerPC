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
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            BaudrateComboBox.Items.Add(Settings.BaudRate.ToString());
            BaudrateComboBox.SelectedIndex = 0;
            VersionCheckBox.IsChecked = Settings.Oldversion;
            LogsEnabledCheckBox.IsChecked = Settings.Logining;
        }

        private void BaudrateComboBox_DropDownOpened(object sender, EventArgs e)
        {
            BaudrateComboBox.Items.Clear();
            BaudrateComboBox.Items.Add($"115200");
            BaudrateComboBox.Items.Add($"57600");
            BaudrateComboBox.Items.Add($"38400");
            BaudrateComboBox.Items.Add($"19200");
            BaudrateComboBox.Items.Add($"9600");
        }

        private void BaudrateComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (BaudrateComboBox.SelectedItem == null) return;
            Settings.BaudRate = Convert.ToInt32(BaudrateComboBox.SelectedItem);
            ComPort.InitComPort(Settings.Com, Settings.BaudRate);
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Close();

        private void VersionCheckBox_Checked(object sender, RoutedEventArgs e) => Settings.Oldversion = true;

        private void VersionCheckBox_Unchecked(object sender, RoutedEventArgs e) => Settings.Oldversion = false;

        private void LogsEnabledCheckBox_Checked(object sender, RoutedEventArgs e) => Settings.Logining = true;

        private void LogsEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e) => Settings.Logining = false;
    }


}
