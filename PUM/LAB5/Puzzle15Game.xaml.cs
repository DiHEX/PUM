using Plugin.Maui.Audio;

namespace PUM.LAB5;

public partial class Puzzle15Game : ContentPage
{
    private int[,] board = new int[4, 4];
    private (int Row, int Col) emptyTile;
    private int moveCount;
    private Timer gameTimer;
    private TimeSpan elapsedTime;
    private readonly IAudioManager audioManager;
    private bool isTimerStarted;

    public Puzzle15Game(IAudioManager audioManager)
    {
        InitializeComponent();

        this.audioManager = audioManager;

        InitializeGame();
    }

    private void InitializeGame()
    {
        var numbers = Enumerable.Range(1, 15).OrderBy(_ => Guid.NewGuid()).ToList();
        numbers.Add(0); // Puste pole
        int index = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                board[i, j] = numbers[index++];
                if (board[i, j] == 0)
                {
                    emptyTile = (i, j);
                }
            }
        }

        moveCount = 0;
        elapsedTime = TimeSpan.Zero;
        
        isTimerStarted = false;

        UpdateUI();
    }

    private void StartTimer()
    {
        gameTimer = new Timer(_ =>
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            MainThread.BeginInvokeOnMainThread(() => TimerLabel.Text = $"Czas: {elapsedTime:mm\\:ss}");
        }, null, 0, 1000);
    }

    private async void ShuffleClicked(object sender, EventArgs e)
    {
        PlaySound("shuffle");
        if (gameTimer is not null) { 
            ResetTimer();
            gameTimer.Dispose();
        }
        InitializeGame();
    }

    private void ReturnClicked(object sender, EventArgs e)
    {
        if (gameTimer is not null)
        {
            gameTimer.Dispose();
        }
        Navigation.PopAsync();
    }

    private async void OnTileTapped(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            int tileNumber = int.Parse(button.Text);
            var position = FindTilePosition(tileNumber);

            if (IsMoveValid(position.Row, position.Col))
            {
                if (!isTimerStarted)
                {
                    StartTimer();
                    isTimerStarted = true;
                }

                MoveTile(position.Row, position.Col);
                moveCount++;
                PlaySound("move");
                UpdateUI();

                if (CheckVictory())
                {
                    gameTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    PlaySound("win");
                    await DisplayAlert("Wygrana!", $"U³o¿y³eœ uk³adankê w czasie {elapsedTime:mm\\:ss} z {moveCount} ruchami.", "OK");
                    InitializeGame();
                }
            }
        }
    }

    private (int Row, int Col) FindTilePosition(int tileNumber)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (board[i, j] == tileNumber)
                {
                    return (i, j);
                }
            }
        }
        return (-1, -1);
    }

    private bool IsMoveValid(int row, int col)
    {
        return (Math.Abs(emptyTile.Row - row) == 1 && emptyTile.Col == col) ||
               (Math.Abs(emptyTile.Col - col) == 1 && emptyTile.Row == row);
    }

    private void MoveTile(int row, int col)
    {
        board[emptyTile.Row, emptyTile.Col] = board[row, col];
        board[row, col] = 0;
        emptyTile = (row, col);
    }

    private void UpdateUI()
    {
        PuzzleGrid.Children.Clear();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (board[i, j] != 0)
                {
                    var button = new Button
                    {
                        Text = board[i, j].ToString(),
                        FontSize = 24,
                        BackgroundColor = Colors.LightGray
                    };
                    button.Clicked += OnTileTapped;
                    PuzzleGrid.Add(button, j, i);
                }
            }
        }

        MoveCountLabel.Text = $"Ruchy: {moveCount}";
    }

    private bool CheckVictory()
    {
        int expected = 1;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (i == 3 && j == 3) return true; // Puste pole
                if (board[i, j] != expected++) return false;
            }
        }
        return true;
    }

    private async void PlaySound(string state)
    {
        string soundFile = state switch
        {
            "move" => "move.mp3",
            "win" => "win.mp3",
            "shuffle" => "shuffle.mp3",
            _ => null
        };

        if (!string.IsNullOrEmpty(soundFile))
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(soundFile));
            player.Play();
        }
    }

    private void ResetTimer()
    {
        elapsedTime = TimeSpan.Zero;
        TimerLabel.Text = $"Czas: {elapsedTime:mm\\:ss}";
    }
}
