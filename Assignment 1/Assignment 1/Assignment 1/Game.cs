using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_1
{
    class Game
    {
        private int gameID;
        private string guess;
        private string definition;
        private string hint;
        private int numCorrect;
        private int score;

        public Game(int gameID, string definition, string hint)
        {
            this.gameID = gameID;
            guess = "";
            this.definition = definition;
            this.hint = hint;
            numCorrect = 0;
            score = -1;

        }

        public int GetGameID()
        {
            return gameID;

        }

        public string GetGuess()
        {
            return guess;

        }

        public void SetGuess(string guess)
        {
            this.guess = guess;

        }

        public string GetDefinition()
        {
            return definition;

        }

        public string GetHint()
        {
            return hint;

        }

        public void SetHint(string hint)
        {
            this.hint = hint;

        }

        public int GetWordLength()
        {
            return hint.Length;

        }

        public int GetNumCorrect()
        {
            return numCorrect;

        }

        public void SetNumCorrect(int numCorrect)
        {
            this.numCorrect = numCorrect;

        }

        public int GetScore()
        {
            return score;

        }

        public void SetScore(int score)
        {
            this.score = score;

        }

    }

}
