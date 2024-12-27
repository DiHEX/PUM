using Plugin.Maui.Audio;
using System.Diagnostics;

namespace PUM.LAB7;

public partial class MemoryGame : ContentPage
{
	private readonly IAudioManager audioManager;
    private const int DefaultGridSize = 4; // 4x4
    private int[,] cardValues;
    private Button[,] buttons;
    private Stopwatch stopwatch;
    private int firstRow = -1, firstCol = -1;
    private int pairsFound = 0;

    public MemoryGame(IAudioManager audioManager)
	{
		InitializeComponent();

		this.audioManager = audioManager;
        InitializeGame(DefaultGridSize);
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
                    FontSize = 24,
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
        if (buttons[row, col].Text != "?") return; // Ju¿ odkryta

        buttons[row, col].Text = cardValues[row, col].ToString();

        if (firstRow == -1 && firstCol == -1)
        {
            firstRow = row;
            firstCol = col;
        }
        else
        {
            await Task.Delay(500);

            if (cardValues[firstRow, firstCol] == cardValues[row, col])
            {
                buttons[firstRow, firstCol].IsEnabled = false;
                buttons[row, col].IsEnabled = false;
                pairsFound++;
                if (pairsFound == cardValues.Length / 2)
                    await DisplayAlert("Gratulacje!", "Wszystkie pary odkryte!", "OK");
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
}