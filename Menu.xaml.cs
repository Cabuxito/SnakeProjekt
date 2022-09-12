using System;
using System.Collections.ObjectModel;
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
        MainWindow main = new();
        public Menu()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        private void Start_Click(object sender, EventArgs e)
        {
            this.Close();
            main.ShowDialog();
        }
        private void BtnShowHighscoreList_Click(object sender, RoutedEventArgs e)
        {
            StartBtn.Visibility = Visibility.Collapsed;
            bdrHighscoreList.Visibility = Visibility.Visible;
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            main.Close();
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
