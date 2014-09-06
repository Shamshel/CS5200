using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment_1
{
    public partial class Form1 : Form
    {
        private ServerInterface gameInterface;
        private HostAddress remoteHostAddress;
        private Game currentGame;
        private int totalPoints;

        public Form1()
        {
            InitializeComponent();
            currentGame = null;
            remoteHostAddress = new HostAddress();
            gameInterface = null;
            totalPoints = 0;
            definitionLabel.Visible = false;
            hintLabel.Visible = false;
            resPointsLabel.Visible = false;
            numPointsLabel.Visible = false;
            numCorrectLabel.Visible = false;
            gameSelectButton.Enabled = false;
            gameSelectComboBox.Enabled = false;
            startGameButton.Enabled = false;
            guessButton.Enabled = false;
            getHintButton.Enabled = false;
            quitButton.Enabled = false;
            guessTextBox.Enabled = false;

        }

        private void hostConnectButton_Click(object sender, EventArgs e)
        {
            remoteHostAddress.SetHostInfo(serverAddressTextBox.Text, portNumberTextBox.Text);

            if (gameInterface == null)
            {
                gameInterface = new ServerInterface(remoteHostAddress, "0", aNumberTextBox.Text, firstNameTextBox.Text, lastNameTextBox.Text);

                startGameButton.Enabled = true;

            }

            else
            {
                //shamelessly incorporated from http://www.howtosolutions.net/2013/01/creating-a-new-popup-window-in-winform-using-csharp/#.VAo0eRZrbcs
                PopupForm popup = new PopupForm();
                DialogResult dialogResult = popup.ShowDialog();

                popup.Dispose();

                //user changed their mind
                if (dialogResult == DialogResult.Cancel)
                {
                    return;

                }

                currentGame = null;

                gameInterface.ClearGames();

                gameSelectComboBox.Items.Clear();
                gameSelectComboBox.Text = "";

                definitionLabel.Text = "";
                hintLabel.Text = "";

                DisableLables();

                gameInterface.SetUserInfo(aNumberTextBox.Text, firstNameTextBox.Text, lastNameTextBox.Text);
                gameInterface.ChangeServer(remoteHostAddress);

            }

        }

        private void gameSelectButton_Click(object sender, EventArgs e)
        {
            if(gameSelectComboBox.Items.Count == 0)
            {
                return;

            }

            for(int i = 0; i < gameInterface.GetGameList().Count; i++)
            {
                if ((int)gameSelectComboBox.SelectedItem == gameInterface.GetGameList()[i].GetGameID())
                {
                    currentGame = gameInterface.GetGameList()[i];
                    break;

                }

            }

            UpdateLabels();

        }

        private void guessButton_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(guessTextBox.Text))
            {
                return;

            }

            currentGame.SetGuess(guessTextBox.Text);

            currentGame = gameInterface.TryGuess(currentGame);


            if(currentGame != null)
            {
                //success!
                if(currentGame.GetScore() > -1)
                {
                    resPointsLabel.Text = currentGame.GetScore().ToString();
                    resPointsLabel.Visible = true;

                    totalPoints += currentGame.GetScore();
                    numPointsLabel.Text = totalPoints.ToString();
                    numPointsLabel.Visible = true;

                    EndGame();

                }

                else
                {
                    numCorrectLabel.Visible = true;
                    numCorrectLabel.Text = currentGame.GetNumCorrect().ToString();

                }

            }

        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            EndGame();

        }

        private void EndGame()
        {
            gameInterface.EndGame(currentGame.GetGameID());

            gameSelectComboBox.Items.Remove(currentGame.GetGameID());

            if (gameInterface.GetGameList().Count >= 1)
            {
                currentGame = gameInterface.GetGameList()[gameInterface.GetGameList().Count - 1];
                gameSelectComboBox.Text = currentGame.GetGameID().ToString();

                UpdateLabels();

                if (gameInterface.GetGameList().Count == 1)
                {
                    gameSelectComboBox.Enabled = false;
                    gameSelectButton.Enabled = false;

                }

            }

            else
            {
                currentGame = null;

                gameSelectComboBox.Text = "";
                guessTextBox.Text = "";

                DisableLables();

            }

        }

        private void startGameButton_Click(object sender, EventArgs e)
        {
            currentGame = gameInterface.StartNewGame();

            if (currentGame != null)
            {
                gameSelectComboBox.Items.Add(currentGame.GetGameID());
                gameSelectComboBox.Text = currentGame.GetGameID().ToString();

                if (gameSelectComboBox.Items.Count > 1)
                {
                    gameSelectComboBox.Enabled = true;
                    gameSelectButton.Enabled = true;
                }

                else
                {
                    gameSelectComboBox.Enabled = false;
                    gameSelectButton.Enabled = false;

                }

                EnableLabels();

                UpdateLabels();

            }

        }

        private void getHintButton_Click(object sender, EventArgs e)
        {
            currentGame = gameInterface.GetHint(currentGame);

            UpdateLabels();

        }

        private void UpdateLabels()
        {
            if (currentGame != null)
            {
                definitionLabel.Text = currentGame.GetDefinition();
                hintLabel.Text = currentGame.GetHint() + " ("+currentGame.GetWordLength().ToString()+" letters)";
                numCorrectLabel.Text = currentGame.GetNumCorrect().ToString();
                guessTextBox.Text = currentGame.GetGuess();

                if(currentGame.GetScore() > -1)
                {
                    resPointsLabel.Visible = true;
                    resPointsLabel.Text = currentGame.GetScore().ToString();

                }

                else
                {
                    resPointsLabel.Visible = false;

                }

            }

            else
            {
                definitionLabel.Text = "";
                hintLabel.Text = "";

            }

            numPointsLabel.Text = totalPoints.ToString();

        }

        private void EnableLabels()
        {
            hintLabel.Visible = true;
            definitionLabel.Visible = true;
            guessButton.Enabled = true;
            getHintButton.Enabled = true;
            quitButton.Enabled = true;
            guessTextBox.Enabled = true;

        }

        private void DisableLables()
        {
            hintLabel.Visible = false;
            definitionLabel.Visible = false;
            guessButton.Enabled = false;
            getHintButton.Enabled = false;
            quitButton.Enabled = false;
            guessTextBox.Enabled = false;

        }

    }

}
