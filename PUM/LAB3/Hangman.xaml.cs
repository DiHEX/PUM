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
            }
            else
            {
                remainingAttempts--;
            }

            UpdateUI();

            if (new string(guessedWord) == secretWord)
            {
                DisplayAlert("Wygrana!", "Gratulacje, odgad�e� s�owo!", "OK");
                ResetGame();
            }
            else if (remainingAttempts == 0)
            {
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
