using SnakeProjekt.Models;
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
using System.Windows.Threading;

namespace SnakeProjekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Snake Information
        const int _snakeSquareSize = 20;
        const int SnakeStartLenght = 3;
        const int SnakeStartSpeed = 300;
        const int SnakeSpeedThreshold = 100;
        private SolidColorBrush _snakeHead = Brushes.Green;
        private SolidColorBrush _snakeBody = Brushes.LightGreen;
        private List<SnakeParts> _snakeParts = new();
        #endregion

        public enum SnakeDirection { Left , Right, Top , Down }
        private SnakeDirection _direction = SnakeDirection.Right;
        private DispatcherTimer _gameTickTimer = new DispatcherTimer();
        private int _snakeLength;

        public MainWindow()
        {
            InitializeComponent();
            _gameTickTimer.Tick += GameTickTimer_Tick;
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DrawGameArea();
            StartNewGame();
        }
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
                    Fill = nextIsOdd ? Brushes.DarkGray : Brushes.Gray
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
                case SnakeDirection.Top:
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
        }

        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }

        private void StartNewGame()
        {
            _snakeLength = SnakeStartLenght;
            _direction = SnakeDirection.Right;
            _snakeParts.Add(new SnakeParts()
            {
                Position = new Point(_snakeSquareSize * 5, _snakeSquareSize * 5)
            });
            _gameTickTimer.Interval = TimeSpan.FromMilliseconds(SnakeStartSpeed);

            DrawSnake();
            _gameTickTimer.IsEnabled = true;
        }
    }
}
