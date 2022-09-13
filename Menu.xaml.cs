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
using SnakeProjekt.Models;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace SnakeProjekt
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        static HighScore score = new();
        MainWindow main = new(score);
        public Menu()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            LoadHighscoreList();
            this.DataContext = score;
        }
        
        private void Start_Click(object sender, EventArgs e)
        {
            main.Show();
        }

        #region Score
        private void BtnShowHighscoreList_Click(object sender, RoutedEventArgs e)
        {
            StartBtn.Visibility = Visibility.Collapsed;
            bdrHighscoreList.Visibility = Visibility.Visible;
        }
        private void LoadHighscoreList()
        {
            if (File.Exists("snake_highscorelist.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<SnakeHighScore>));
                using (Stream reader = new FileStream("snake_highscorelist.xml", FileMode.Open))
                {
                    List<SnakeHighScore> tempList = (List<SnakeHighScore>)serializer.Deserialize(reader);
                    score.HighscoreList.Clear();
                    foreach (var item in tempList.OrderByDescending(x => x.Score))
                        score.HighscoreList.Add(item);
                }
            }
        }
        #endregion

        #region Buttons Interactions
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            bdrHighscoreList.Visibility = Visibility.Collapsed;
            StartBtn.Visibility = Visibility.Visible;
        }
        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        private void OptionsWindow_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}   
