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

namespace PhotoBeanApp.Helper.UserControls
{
    /// <summary>
    /// Interaction logic for UpDownField.xaml
    /// </summary>
    public partial class UpDownField : UserControl
    {
        public UpDownField()
        {
            InitializeComponent();
        }
        private void btn_PlusNum_Click(object sender, RoutedEventArgs e)
        {
            int i = Convert.ToInt32(btn_Num.Content);
            if (i < 99)
                btn_Num.Content = i + 1;
        }

        private void btn_MinNum_Click(object sender, RoutedEventArgs e)
        {
            int i = Convert.ToInt32(btn_Num.Content);
            if (i > 0)
                btn_Num.Content = i - 1;
        }

        public int getNum()
        {
            return Convert.ToInt32(btn_Num.Content);
        }
    }
}
