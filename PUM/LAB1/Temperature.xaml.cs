namespace PUM.LAB1;

public partial class Temperature : ContentPage
{
	public Temperature()
	{
		InitializeComponent();
	}

    private void OnConvertClicked(object sender, EventArgs recivedEvent)
    {
        if (string.IsNullOrWhiteSpace(CelsiusEntry.Text))
        {
            ResultLabel.Text = "Podaj poprawn¹ wartoœæ";
            return;
        }

        double.TryParse(CelsiusEntry.Text, out double celsius);
        double fahrenheit = (celsius * 9 / 5) + 32;
        ResultLabel.Text = $"Wynik: {fahrenheit} °F";
    }

    private void ReturnClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}