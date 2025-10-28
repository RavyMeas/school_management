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

namespace school_management.view
{
    /// <summary>
    /// Interaction logic for NotImplementedView.xaml
    /// </summary>
    public partial class NotImplementedView : UserControl
    {
        public NotImplementedView(string fName = "feature")
        {
            InitializeComponent();
            TitleTextBlock.Text = fName;
        }
    }
}
