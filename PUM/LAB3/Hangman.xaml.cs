using System;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;

namespace PUM.LAB3
{
    public partial class Hangman : ContentPage
    {
        private string secretWord = "programowanie"; // Ukryte s³owo
        private readonly IAudioManager audioManager;
        private char[] guessedWord;
        private int remainingAttempts = 6;
        private string guessedLetters = "";

        public Hangman(IAudioManager audioManager)
        {
            InitializeComponent();

            this.audioManager = audioManager;

            guessedWord = new string('_', secretWord.Length).ToCharArray();
            UpdateUI();
        }

        private async void OnGuessButtonClicked(object sender, EventArgs e)
        {
            string input = GuessEntry.Text?.ToLower();

            if (string.IsNullOrWhiteSpace(input) || input.Length != 1)
            {
                await DisplayAlert("B³¹d", "Proszê wprowadziæ jedn¹ literê.", "OK");
                return;
            }

            char guessedLetter = input[0];

            if (guessedLetters.Contains(guessedLetter))
            {
                await DisplayAlert("B³¹d", "Ta litera zosta³a ju¿ u¿yta.", "OK");
                return;
            }

            guessedLetters += guessedLetter;

            if (secretWord.Contains(guessedLetter))
            {
                for (int i = 0; i < secretWord.Length; i++)
                {
                    if (secretWord[i] == guessedLetter)
                    {
                        guessedWord[i] = guessedLetter;
                    }
                }
                PlaySound("correct");
            }
            else
            {
                remainingAttempts--;
                PlaySound("incorrect");
            }

            UpdateUI();

            if (new string(guessedWord) == secretWord)
            {
                PlaySound("win");
                await DisplayAlert("Wygrana!", "Gratulacje, odgad³eœ s³owo!", "OK");
                ResetGame();
            }
            else if (remainingAttempts == 0)
            {
                PlaySound("lose");
                await DisplayAlert("Przegrana", $"Przegra³eœ! Ukryte s³owo to: {secretWord}", "OK");
                ResetGame();
            }
        }

        private void UpdateUI()
        {
            WordLabel.Text = new string(guessedWord);
            AttemptsLabel.Text = $"Pozosta³e próby: {remainingAttempts}";
            UsedLettersLabel.Text = $"U¿yte litery: {guessedLetters}";
            GuessEntry.Text = string.Empty;
            UpdateImage();
        }

        private void UpdateImage()
        {
            string imageName = $"hangman_{6 - remainingAttempts}"; // Obrazy: hangman_0.png, hangman_1.png, itd.
            HangmanImage.Source = ImageSource.FromFile(imageName + ".png");
        }

        private async void PlaySound(string state)
        {
            string soundFile = state switch
            {
                "correct" => "correct.mp3",
                "incorrect" => "incorrect.mp3",
                "win" => "win.mp3",
                "lose" => "lose.mp3",
                _ => null
            };

            if (!string.IsNullOrEmpty(soundFile))
            {
                var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(soundFile));
            
                player.Play();
            }
        }

        private void ResetGame()
        {
            guessedWord = new string('_', secretWord.Length).ToCharArray();
            remainingAttempts = 6;
            guessedLetters = "";
            UpdateUI();
        }

        private void ReturnClicked(object sender, EventArgs e)
        {
            ResetGame();
            Navigation.PopAsync();
        }
    }
}
