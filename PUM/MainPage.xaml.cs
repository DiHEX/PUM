namespace PUM
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnConvertClicked(object sender, EventArgs recivedEvent)
        {
            if (string.IsNullOrWhiteSpace(CelsiusEntry.Text))
            {
                ResultLabel.Text = "Podaj poprawną wartość";
                return;
            }

            double.TryParse(CelsiusEntry.Text, out double celsius);
            double fahrenheit = (celsius * 9 / 5) + 32;
            ResultLabel.Text = $"Wynik: {fahrenheit} °F";
        }

    }

}
