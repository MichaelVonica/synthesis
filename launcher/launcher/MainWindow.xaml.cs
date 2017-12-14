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

namespace launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void synthesis_Click(object sender, RoutedEventArgs e)
        {
            Launcher.launch_synthesis();
        }

        private void emulator_Click(object sender, RoutedEventArgs e)
        {
            Launcher.launch_emulator();
        }

        private void website_Click(object sender, RoutedEventArgs e)
        {
            Launcher.open_website();
        }

        private void tutorial_Click(object sender, RoutedEventArgs e)
        {
            Launcher.open_tutorials();
        }
    }
}
