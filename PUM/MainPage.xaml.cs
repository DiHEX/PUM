using Plugin.Maui.Audio;
using PUM.LAB1;
using PUM.LAB3;

namespace PUM
{
    public partial class MainPage : ContentPage
    {
        private readonly IAudioManager audioManager;

        public MainPage(IAudioManager audioManager)
        {
            InitializeComponent();

            this.audioManager = audioManager;
        }

        private async void OnOpenTemperaturePageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Temperature());
        }

        private async void OnOpenHangmanPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Hangman(audioManager));
        }
    }
}
