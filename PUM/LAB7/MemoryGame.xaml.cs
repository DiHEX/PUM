using Plugin.Maui.Audio;
using System.Diagnostics;

namespace PUM.LAB7;

public partial class MemoryGame : ContentPage
{
    private readonly IAudioManager audioManager;
    private const int DefaultGridSize = 4; // 4x4
    private int gridSize;
    private int[,] cardValues;
    private Button[,] buttons;
    private Stopwatch stopwatch;
    private int firstRow = -1, firstCol = -1;
    private int pairsFound = 0;

    public MemoryGame(IAudioManager audioManager)
    {
        InitializeComponent();

        this.audioManager = audioManager;

        PromptForGridSize();
    }

    private async void PromptForGridSize()
    {
        string result = await DisplayPromptAsync("Siatka do gry", "Podaj wielkoœæ siatki (2 dla 2x2, 4 dla 4x4, 6 dla 6x6, 8 dla 8x8):", initialValue: DefaultGridSize.ToString(), maxLength: 1, keyboard: Keyboard.Numeric);
        if (int.TryParse(result, out int size) && size > 0 && size % 2 == 0 && size < 9)
        {
            gridSize = size;
        }
        else
        {
            gridSize = DefaultGridSize;
        }


        InitializeGame(gridSize);
        StartTimer();
    }

    private void InitializeGame(int gridSize)
    {
        stopwatch = new Stopwatch();
        pairsFound = 0;
        GameGrid.Children.Clear();
        GameGrid.RowDefinitions.Clear();
        GameGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < gridSize; i++)
        {
            GameGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        cardValues = GenerateCardValues(gridSize);
        buttons = new Button[gridSize, gridSize];

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                var button = new Button
                {
                    BackgroundColor = Colors.Gray,
                    Text = "?",
                    FontSize = FontSize(gridSize),
                    FontAttributes = FontAttributes.Bold
                };

                int localRow = row;
                int localCol = col;
                button.Clicked += (sender, args) => OnCardClicked(localRow, localCol);
                buttons[row, col] = button;
                GameGrid.Add(button, col, row);
            }
        }
    }

    private int FontSize(int gridSize)
    {
        switch (gridSize)
        {
            case 2:
            case 4:
                return 24;
            case 6:
                return 16;
            case 8:
                return 12;
            default:
                return 24;
        }
    }

    private int[,] GenerateCardValues(int gridSize)
    {
        int totalCards = gridSize * gridSize;
        var values = Enumerable.Range(1, totalCards / 2)
                               .Concat(Enumerable.Range(1, totalCards / 2))
                               .OrderBy(_ => Guid.NewGuid())
                               .ToArray();

        int[,] grid = new int[gridSize, gridSize];
        int index = 0;

        for (int row = 0; row < gridSize; row++)
            for (int col = 0; col < gridSize; col++)
                grid[row, col] = values[index++];

        return grid;
    }

    private async void OnCardClicked(int row, int col)
    {
        if (buttons[row, col].Text != "?") return; // Already revealed

        PlaySound("shuffle");

        buttons[row, col].Text = cardValues[row, col].ToString();

        await Task.Delay(500);

        if (firstRow == -1 && firstCol == -1)
        {
            firstRow = row;
            firstCol = col;
        }
        else
        {

            if (cardValues[firstRow, firstCol] == cardValues[row, col])
            {
                buttons[firstRow, firstCol].IsEnabled = false;
                buttons[row, col].IsEnabled = false;
                pairsFound++;
                if (pairsFound == cardValues.Length / 2)
                {
                    PlaySound("win");
                    await DisplayAlert("Gratulacje!", "Wszyskie pary odkryte!", "OK");
                    PromptForGridSize();
                }
            }
            else
            {
                buttons[firstRow, firstCol].Text = "?";
                buttons[row, col].Text = "?";
            }

            firstRow = -1;
            firstCol = -1;
        }
    }

    private void StartTimer()
    {
        stopwatch.Start();
        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            TimerLabel.Text = stopwatch.Elapsed.ToString(@"m\:ss");
            return true; // Continue
        });
    }

    private async void PlaySound(string state)
    {
        string soundFile = state switch
        {
            "shuffle" => "shuffle.mp3",
            "win" => "win.mp3",
            _ => null
        };

        if (!string.IsNullOrEmpty(soundFile))
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(soundFile));

            player.Play();
        }
    }
}
