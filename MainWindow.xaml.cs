using SnakeProjekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using System.Xml.Serialization;
using static SnakeProjekt.MainWindow;

namespace SnakeProjekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Snake Information
        const int _snakeSquareSize = 15;
        const int _snakeStartLenght = 3;
        const int _snakeStartSpeed = 300;
        const int _snakeSpeedThreshold = 100;
        private int _snakeLength;
        private SolidColorBrush _snakeHead = Brushes.DarkOrange;
        private SolidColorBrush _snakeBody = Brushes.Orange;
        private List<SnakeParts> _snakeParts = new();
        #endregion

        #region Food Information
        private UIElement _snakeFood = null;
        private UIElement _bonusFood = null;
        private SolidColorBrush _foodBrush = Brushes.Red;
        private SolidColorBrush _bonusBrush = Brushes.Black;
        private Random rnd = new();
        #endregion

        const int MaxHighscoreListEntryCount = 5;
        

        private DispatcherTimer _gameTickTimer = new DispatcherTimer();
        private int _currentScore = 0;
        HighScore _score;

        public MainWindow(HighScore score)
        {
            InitializeComponent();
            this.DataContext = this;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _gameTickTimer.Tick += GameTickTimer_Tick;
            _score = score;
        }

        private void Window_ContentRendered(object item, EventArgs e)
        {
            DrawGameArea();
            StartNewGame();
        }
        private void Window_MouseDown(Object sender, MouseButtonEventArgs btnArgs)
        {
            this.DragMove();
        }

        #region Start/End Game
        private void StartNewGame()
        {
            //Remove potential dead snake parts and leftover food...
            foreach (SnakeParts item in _snakeParts)
            {
                if (item.UiElement != null)
                    GameArea.Children.Remove(item.UiElement);
            }
            _snakeParts.Clear();
            if (_snakeFood != null)
                GameArea.Children.Remove(_snakeFood);

            // Reset stuff
            _currentScore = 0;
            _snakeLength = _snakeStartLenght;
            _direction = SnakeDirection.Right;
            _snakeParts.Add(new SnakeParts() { Position = new Point(_snakeSquareSize * 5, _snakeSquareSize * 5) });
            _gameTickTimer.Interval = TimeSpan.FromMilliseconds(_snakeStartSpeed);

            // Draw the snake again and some new food...
            DrawSnake();
            DrawSnakeFood();

            // Update status
            UpdateGameStatus();

            // Go!        
            _gameTickTimer.IsEnabled = true;
            bdrEndOfGame.Visibility = Visibility.Collapsed;
        }
        private void EndGame()
        {
            bool isNewHighscore = false;
            if (_currentScore > 0)
            {
                int lowestHighscore = (_score.HighscoreList.Count > 0 ? _score.HighscoreList.Min(x => x.Score) : 0);
                if ((_currentScore > lowestHighscore) || (_score.HighscoreList.Count < MaxHighscoreListEntryCount))
                {
                    bdrNewHighscore.Visibility = Visibility.Visible;
                    txtPlayerName.Focus();
                    isNewHighscore = true;
                }
            }
            if (!isNewHighscore)
            {
                tbFinalScore.Text = _currentScore.ToString();
                bdrEndOfGame.Visibility = Visibility.Visible;
            }
            _gameTickTimer.IsEnabled = false;
        }
        #endregion

        #region Snake Movements
        public enum SnakeDirection { Left , Right, Up , Down }
        private void Window_KeyUp(object item, KeyEventArgs e)
        {
            SnakeDirection direction = _direction;
            switch (e.Key)
            {
                case Key.Up:
                    if (_direction != SnakeDirection.Down)
                        _direction = SnakeDirection.Up;
                    break;
                case Key.Down:
                    if (_direction != SnakeDirection.Up)
                        _direction = SnakeDirection.Down;
                    break;
                case Key.Left:
                    if (_direction != SnakeDirection.Right)
                        _direction = SnakeDirection.Left;
                    break;
                case Key.Right:
                    if(_direction != SnakeDirection.Left)
                        _direction = SnakeDirection.Right;
                    break;
                case Key.Space:
                    StartNewGame();
                    break;
                case Key.Escape:
                    this.Hide();
                    break;
            }
            if (_direction != direction)
                MoveSnake();
            
        }
        private SnakeDirection _direction = SnakeDirection.Right;
        private void GameTickTimer_Tick(object sender, EventArgs e) => MoveSnake();
        private void MoveSnake()
        {
            while (_snakeParts.Count >= _snakeLength)
            {
                GameArea.Children.Remove(_snakeParts[0].UiElement);
                _snakeParts.RemoveAt(0);
            }
            foreach (SnakeParts item in _snakeParts)
            {
                (item.UiElement as Rectangle).Fill = _snakeBody;
                item.IsHead = false;
            }
            SnakeParts head = _snakeParts[_snakeParts.Count - 1];
            double nextX = head.Position.X;
            double nextY = head.Position.Y;
            switch (_direction)
            {
                case SnakeDirection.Left:
                    nextX -= _snakeSquareSize;
                    break;
                case SnakeDirection.Right:
                    nextX += _snakeSquareSize;
                    break;
                case SnakeDirection.Up:
                    nextY -= _snakeSquareSize;
                    break;
                case SnakeDirection.Down:
                    nextY += _snakeSquareSize;
                    break;
            }
            _snakeParts.Add(new SnakeParts()
            {
                Position = new Point(nextX, nextY),
                IsHead = true
            });
            DrawSnake();
            DoCollisionCheck();
        }
        #endregion

        #region Draw Game and Snake.
        private void DrawGameArea()
        {
            bool doneDrawingBackground = false;
            int nextX = 0, nextY = 0;
            int rowCounter = 0;
            bool nextIsOdd = false;

            while (doneDrawingBackground == false)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = _snakeSquareSize,
                    Height = _snakeSquareSize,
                    Fill = nextIsOdd ? Brushes.Gray : Brushes.Black
                };
                GameArea.Children.Add(rectangle);
                Canvas.SetTop(rectangle, nextY);
                Canvas.SetLeft(rectangle, nextX);

                nextIsOdd = !nextIsOdd;
                nextX += _snakeSquareSize;
                if (nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;
                    nextY += _snakeSquareSize;
                    rowCounter++;
                    nextIsOdd = (rowCounter % 2 != 0);
                }

                if (nextY >= GameArea.ActualHeight)
                    doneDrawingBackground = true;
            }
        }
        private void DrawSnake()
        {
            foreach (SnakeParts item in _snakeParts)
            {
                if (item.UiElement == null)
                {
                    item.UiElement = new Rectangle()
                    {
                        Width = _snakeSquareSize,
                        Height = _snakeSquareSize,
                        Fill = (item.IsHead ? _snakeHead : _snakeBody)
                    };
                    GameArea.Children.Add(item.UiElement);
                    Canvas.SetTop(item.UiElement, item.Position.Y);
                    Canvas.SetLeft(item.UiElement, item.Position.X);
                }
            }
        }
        #endregion

        #region Food Services
        private Point GetNextFoodPosition()
        {
            int maxX = (int)(GameArea.ActualWidth / _snakeSquareSize);
            int maxY = (int)(GameArea.ActualHeight / _snakeSquareSize);
            int foodX = rnd.Next(0, maxX) * _snakeSquareSize;
            int foodY = rnd.Next(0, maxY) * _snakeSquareSize;

            foreach (SnakeParts item in _snakeParts)
            {
                if ((item.Position.X == foodX) && (item.Position.Y == foodY))
                {
                    return GetNextFoodPosition();
                }
            }
            return new Point(foodX, foodY);
        }
        private void DrawSnakeFood()
        {
            Point foodPosition = GetNextFoodPosition();
            _snakeFood = new Ellipse()
            {
                Width = _snakeSquareSize,
                Height = _snakeSquareSize,
                Fill = _foodBrush
            };
            GameArea.Children.Add(_snakeFood);
            Canvas.SetTop(_snakeFood, foodPosition.Y);
            Canvas.SetLeft(_snakeFood, foodPosition.X);

            //Point BonusPoint = GetNextFoodPosition();
            //_bonusFood = new Ellipse()
            //{
            //    Width = _snakeSquareSize,
            //    Height = _snakeSquareSize,
            //    Fill = _bonusBrush
            //};
            //GameArea.Children.Add(_bonusFood);
            //Canvas.SetTop(_bonusFood, foodPosition.Y);
            //Canvas.SetLeft(_bonusFood, foodPosition.X);
        }
        private void EatSnakeFood()
        {
            _snakeLength++;
            _currentScore++;
            int timerInterval = Math.Max(_snakeSpeedThreshold, (int)_gameTickTimer.Interval.TotalMilliseconds - (_currentScore * 2));
            _gameTickTimer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            GameArea.Children.Remove(_snakeFood);
            DrawSnakeFood();
            UpdateGameStatus();
        }
        #endregion

        #region Collision Detection
        private void DoCollisionCheck()
        {
            SnakeParts snakeHead = _snakeParts[_snakeParts.Count - 1];
            if ((snakeHead.Position.X == Canvas.GetLeft(_snakeFood)) && (snakeHead.Position.Y == Canvas.GetTop(_snakeFood)))
            {
                EatSnakeFood();
                return;
            }
            if ((snakeHead.Position.Y < 0 ) || (snakeHead.Position.Y >= GameArea.ActualHeight) ||
                (snakeHead.Position.X < 0 ) || (snakeHead.Position.X >= GameArea.ActualWidth))
            {
                EndGame();   
            }
            foreach (SnakeParts item in _snakeParts.Take(_snakeParts.Count - 1))
            {
                if ((snakeHead.Position.X == item.Position.X) && (snakeHead.Position.Y == item.Position.Y))
                {
                    EndGame();
                }
            }
        }
        private void UpdateGameStatus()
        {
            this.tbStatusScore.Text = _currentScore.ToString();
            this.tbStatusSpeed.Text = _gameTickTimer.Interval.TotalMilliseconds.ToString();
        }
        #endregion

        #region Score
        
        private void SaveHighscoreList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<SnakeHighScore>));
            using (Stream writer = new FileStream("snake_highscorelist.xml", FileMode.Create))
            {
                serializer.Serialize(writer, _score.HighscoreList);
            }
        }
        private void BtnAddToHighscoreList_Click(object sender, RoutedEventArgs e)
        {
            int newIndex = 0;
            // Where should the new entry be inserted?
            if ((_score.HighscoreList.Count > 0) && (_currentScore < _score.HighscoreList.Max(x => x.Score)))
            {
                SnakeHighScore justAbove = _score.HighscoreList.OrderByDescending(x => x.Score).First(x => x.Score >= _currentScore);
                if (justAbove != null)
                    newIndex = _score.HighscoreList.IndexOf(justAbove) + 1;
            }
            // Create & insert the new entry
            _score.HighscoreList.Insert(newIndex, new SnakeHighScore()
            {
                PlayerName = txtPlayerName.Text,
                Score = _currentScore
            });
            // Make sure that the amount of entries does not exceed the maximum
            while (_score.HighscoreList.Count > MaxHighscoreListEntryCount)
                _score.HighscoreList.RemoveAt(MaxHighscoreListEntryCount);

            SaveHighscoreList();

            bdrNewHighscore.Visibility = Visibility.Collapsed;

            this.Hide();
        }
        #endregion

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }   
}
