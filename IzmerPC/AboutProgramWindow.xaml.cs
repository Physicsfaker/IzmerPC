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
    /// Interaction logic for AboutProgramWindow.xaml
    /// </summary>
    public partial class AboutProgramWindow : Window
    {
        public AboutProgramWindow() => InitializeComponent();
        private void Ok_Button_Click(object sender, RoutedEventArgs e) => Close();
    }
}
