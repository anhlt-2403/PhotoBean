using System;
using System.Collections.Generic;
using System.IO;
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

namespace PhotoBeanApp.View
{
    /// <summary>
    /// Interaction logic for WelcomeScreen.xaml
    /// </summary>
    public partial class WelcomeScreen : UserControl
    {
        public event EventHandler StartButtonClick;
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectDirectory;
        public WelcomeScreen()
        {
            InitializeComponent();

        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
