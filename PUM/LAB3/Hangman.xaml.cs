using System;
using Microsoft.Maui.Controls;

namespace PUM.LAB3
{
    public partial class Hangman : ContentPage
    {
        private string secretWord = "programowanie"; // Ukryte s�owo
        private char[] guessedWord;
        private int remainingAttempts = 6;
        private string guessedLetters = "";

        public Hangman()
        {
            InitializeComponent();
            guessedWord = new string('_', secretWord.Length).ToCharArray();
            UpdateUI();
        }

        private void OnGuessButtonClicked(object sender, EventArgs e)
        {
            string input = GuessEntry.Text?.ToLower();

            if (string.IsNullOrWhiteSpace(input) || input.Length != 1)
            {
                DisplayAlert("B��d", "Prosz� wprowadzi� jedn� liter�.", "OK");
                return;
            }

            char guessedLetter = input[0];

            if (guessedLetters.Contains(guessedLetter))
            {
                DisplayAlert("B��d", "Ta litera zosta�a ju� u�yta.", "OK");
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
                DisplayAlert("Wygrana!", "Gratulacje, odgad�e� s�owo!", "OK");
                ResetGame();
            }
            else if (remainingAttempts == 0)
            {
                PlaySound("lose");
                DisplayAlert("Przegrana", $"Przegra�e�! Ukryte s�owo to: {secretWord}", "OK");
                ResetGame();
            }
        }

        private void UpdateUI()
        {
            WordLabel.Text = new string(guessedWord);
            AttemptsLabel.Text = $"Pozosta�e pr�by: {remainingAttempts}";
            UsedLettersLabel.Text = $"U�yte litery: {guessedLetters}";
            GuessEntry.Text = string.Empty;
            UpdateImage();
        }

        private void UpdateImage()
        {
            string imageName = $"hangman_{6 - remainingAttempts}"; // Obrazy: hangman_0.png, hangman_1.png, itd.
            HangmanImage.Source = ImageSource.FromFile(imageName + ".png");
        }

        private void PlaySound(string state)
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
                var player = new Microsoft.Maui.Audio.AudioManager();
                player.PlayAsync(AudioManager.CreateAudioPlayer(soundFile));
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
