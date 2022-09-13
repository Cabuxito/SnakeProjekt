using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeProjekt.Models
{
    public class HighScore
    {
        public ObservableCollection<SnakeHighScore> HighscoreList
        {
            get; set;
        } = new ObservableCollection<SnakeHighScore>();
    }
}
