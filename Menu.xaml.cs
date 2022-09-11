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

namespace SnakeProjekt
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        MainWindow mainWindow;
        public Menu()
        {
            InitializeComponent();
            mainWindow = new MainWindow();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            this.Close();
            mainWindow.Show();
        }

        private void Settings_Click()
        {

        }

        
    }
}
