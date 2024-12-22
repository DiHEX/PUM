using System;
using Microsoft.Maui.Controls;

namespace PUM.LAB3
{
    public partial class Hangman : ContentPage
    {
        private string secretWord = "programowanie"; // Ukryte s³owo
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
                DisplayAlert("B³¹d", "Proszê wprowadziæ jedn¹ literê.", "OK");
                return;
            }

            char guessedLetter = input[0];

            if (guessedLetters.Contains(guessedLetter))
            {
                DisplayAlert("B³¹d", "Ta litera zosta³a ju¿ u¿yta.", "OK");
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
            }
            else
            {
                remainingAttempts--;
            }

            UpdateUI();

            if (new string(guessedWord) == secretWord)
            {
                DisplayAlert("Wygrana!", "Gratulacje, odgad³eœ s³owo!", "OK");
                ResetGame();
            }
            else if (remainingAttempts == 0)
            {
                DisplayAlert("Przegrana", $"Przegra³eœ! Ukryte s³owo to: {secretWord}", "OK");
                ResetGame();
            }
        }

        private void UpdateUI()
        {
            WordLabel.Text = new string(guessedWord);
            AttemptsLabel.Text = $"Pozosta³e próby: {remainingAttempts}";
            UsedLettersLabel.Text = $"U¿yte litery: {guessedLetters}";
            GuessEntry.Text = string.Empty;
        }

        private void ResetGame()
        {
            guessedWord = new string('_', secretWord.Length).ToCharArray();
            remainingAttempts = 6;
            guessedLetters = "";
            UpdateUI();
        }
    }
}
