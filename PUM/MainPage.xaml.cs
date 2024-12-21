using PUM.LAB1;
using PUM.LAB3;

namespace PUM
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnOpenTemperaturePageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Temperature());
        }

        private async void OnOpenHangmanPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Hangman());
        }
    }
}
