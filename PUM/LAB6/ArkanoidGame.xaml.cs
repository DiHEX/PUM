using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PUM.LAB6;

public partial class ArkanoidGame : ContentPage
{
    private const double PaddleWidth = 100;
    private const double PaddleHeight = 20;
    private const double BallSize = 20;
    private const double BlockWidth = 60;
    private const double BlockHeight = 30;

    private BoxView _paddle;
    private Ellipse _ball;
    private List<BoxView> _blocks;

    private double _ballXSpeed = 5;
    private double _ballYSpeed = 5;

    private int _lives = 3;
    private int _score = 0;

    private Label _scoreLabel;
    private Label _livesLabel;

    private bool _isGameRunning;
    private bool canColliadeWithPaddle = true;
    private bool canColliadeWithBlock = false;

    private bool _isPanning;
    private double _paddleDeltaX;

    public ArkanoidGame()
    {
        InitializeComponent();
        _paddle = new BoxView();
        _ball = new Ellipse();
        _blocks = new List<BoxView>();
        _scoreLabel = new Label();
        _livesLabel = new Label();
        InitializeGame();
    }

    private void InitializeGame()
    {
        Title = "Arkanoid";
        BackgroundColor = Colors.Black;

        _paddle = new BoxView
        {
            Color = Colors.White,
            WidthRequest = PaddleWidth,
            HeightRequest = PaddleHeight
        };

        _ball = new Ellipse
        {
            Fill = Colors.White,
            WidthRequest = BallSize,
            HeightRequest = BallSize
        };

        _blocks = new List<BoxView>();

        _scoreLabel = new Label
        {
            TextColor = Colors.White,
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Center
        };

        _livesLabel = new Label
        {
            TextColor = Colors.White,
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Center
        };

        var grid = new Grid
        {
            RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star },
                }
        };

        var infoStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Children = { _scoreLabel, _livesLabel }
        };

        grid.Children.Add(infoStack);
        Grid.SetRow(infoStack, 0);

        var gameCanvas = new ContentView { Content = CreateGameCanvas() };

        var panGestureRecognizer = new PanGestureRecognizer();
        panGestureRecognizer.PanUpdated += OnGameCanvasPanUpdated;
        gameCanvas.GestureRecognizers.Add(panGestureRecognizer);

        grid.Children.Add(gameCanvas);
        Grid.SetRow(gameCanvas, 1);

        Content = grid;

        StartGame();
    }

    private void OnGameCanvasPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _isPanning = true;
                break;
            case GestureStatus.Running:
                _paddleDeltaX = e.TotalX;
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _isPanning = false;
                break;
        }
    }

    private void MovePaddle()
    {
        if (_isPanning)
        {
            var paddleBounds = _paddle.Bounds;
            paddleBounds.X = Math.Max(0, Math.Min(Width - PaddleWidth, paddleBounds.X + _paddleDeltaX / 8));
            AbsoluteLayout.SetLayoutBounds(_paddle, paddleBounds);
        }
    }

    private View CreateGameCanvas()
    {
        var absoluteLayout = new AbsoluteLayout();
        
        // Add ball
        AbsoluteLayout.SetLayoutBounds(_ball, new Rect(200, 380, 0, 0));
        absoluteLayout.Children.Add(_ball);

        // Add paddle
        AbsoluteLayout.SetLayoutBounds(_paddle, new Rect(200, 400, 0, 0));
        absoluteLayout.Children.Add(_paddle);

        // Add blocks
        double blockTopMargin = 50;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                var block = new BoxView
                {
                    Color = GetRandomColor(),
                    WidthRequest = BlockWidth,
                    HeightRequest = BlockHeight
                };

                AbsoluteLayout.SetLayoutBounds(block, new Rect(j * BlockWidth + 20, i * BlockHeight + blockTopMargin, BlockWidth, BlockHeight));
                absoluteLayout.Children.Add(block);
                _blocks.Add(block);
            }
        }

        return absoluteLayout;
    }


    private void StartGame()
    {
        _lives = 3;
        _score = 0;

        _isGameRunning = true;

        UpdateLabels();
        Application.Current?.Dispatcher?.StartTimer(TimeSpan.FromMilliseconds(16), GameLoop);
        Application.Current?.Dispatcher?.StartTimer(TimeSpan.FromMilliseconds(16), MovePaddleLoop);
    }

    private bool MovePaddleLoop()
    {
        if (!_isGameRunning)
            return false;

        MovePaddle();
        return true;
    }

    private bool GameLoop()
    {
        if (!_isGameRunning)
            return false;

        if (Height > 0)
        {
            MoveBall();
            CheckCollisions();
        }

        return true;
    }

    private void MoveBall()
    {
        var ballBounds = _ball.Bounds;

        ballBounds.X += _ballXSpeed;
        ballBounds.Y += _ballYSpeed;

        if (ballBounds.Left <= 0 || ballBounds.Right >= Width)
            _ballXSpeed = -_ballXSpeed;

        if (ballBounds.Top <= 0)
        {
            _ballYSpeed = -_ballYSpeed;
            var random = new Random();
            _ballXSpeed = random.NextDouble() * 10 - 5; // Random speed between -5 and 5
            canColliadeWithPaddle = true;
        }

        if (ballBounds.Y >= Height)
        {
            _lives--;
            UpdateLabels();
            if (_lives <= 0)
            {
                EndGame();
            }
            else
            {
                ballBounds.Y = _paddle.Bounds.Y - BallSize;
                ballBounds.X = _paddle.Bounds.X + BallSize * 2;
            }
        }

        AbsoluteLayout.SetLayoutBounds(_ball, ballBounds);
    }

    private void CheckCollisions()
    {
        // Paddle collision
        if (_ball.Bounds.IntersectsWith(_paddle.Bounds) && canColliadeWithPaddle)
        {
            _ballYSpeed = -_ballYSpeed;
            canColliadeWithPaddle = false;
            canColliadeWithBlock = true;
        }

        // Block collisions
        foreach (var block in _blocks.ToList())
        {
            if (_ball.Bounds.IntersectsWith(block.Bounds) && (canColliadeWithBlock || _ball.Bounds.Y <= 170))
            {
                _ballYSpeed = -_ballYSpeed;
                canColliadeWithPaddle = true;
                canColliadeWithBlock = false;

                _blocks.Remove(block);
                // Usuniêcie z uk³adu wizualnego
                if (block.Parent is Layout parentLayout)
                {
                    parentLayout.Children.Remove(block);
                }
                _score += 10;
                UpdateLabels();
            }
        }

        if (!_blocks.Any())
        {
            EndGame();
        }
    }

    private async void EndGame()
    {
        _isGameRunning = false;
        _ballXSpeed = 5;
        _ballYSpeed = 5;

        string message = "Koniec gry";

        if (_score == 300)
        {
            message = "Wygra³eœ!";
        }
        await DisplayAlert($"{message}", $"Wynik: {_score}", "OK");
        InitializeGame();
    }

    private void UpdateLabels()
    {
        _scoreLabel.Text = $"Wynik: {_score}, ";
        _livesLabel.Text = $"¯ycia: {_lives}";
    }

    private Color GetRandomColor()
    {
        var random = new Random();
        return Color.FromRgb(random.Next(256), random.Next(256), random.Next(256));
    }
}
