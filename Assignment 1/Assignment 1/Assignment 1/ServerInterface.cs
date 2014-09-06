using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Assignment_1
{
    class ServerInterface
    {
        private ConnectionSocket serverConnection;
        private HostAddress hostAddress;
        private string localPort;
        private string aNumber;
        private string firstName;
        private string lastName;
        private List<Game> gameList;

        public ServerInterface(HostAddress hostAddress, string localPort, string aNumber, string firstName, string lastName)
        {
            this.hostAddress = hostAddress;
            this.localPort = localPort;
            this.aNumber = aNumber;
            this.firstName = firstName;
            this.lastName = lastName;
            gameList = new List<Game>();

            serverConnection = new ConnectionSocket(hostAddress, localPort, SocketType.Dgram, ProtocolType.Udp);
            serverConnection.Start();

        }

        public void ChangeServer(HostAddress hostAddress)
        {
            for(int i = 0; i < gameList.Count; i++)
            {
                EndGame(i);

            }

            serverConnection.Stop();

            serverConnection = new ConnectionSocket(hostAddress, localPort, SocketType.Dgram, ProtocolType.Udp);

        }

        public void SetUserInfo(string aNumber, string firstName, string lastName)
        {
            this.aNumber = aNumber;
            this.firstName = firstName;
            this.lastName = lastName;

        }

        //Protocol 1
        public Game StartNewGame()
        {
            //start new game
            //should be of format: newgame:<aNumber>,<lastName>,<firstName>
            string response = "";

            serverConnection.Send("newgame:" + aNumber + "," + lastName + "," + firstName );

            System.Threading.Thread.Sleep(200);

            if (serverConnection.Available() > 0)
            {
                response = serverConnection.Receive();

            }

            string[] result;

            //recieve answer/error
            //answer should be of format: def:<gameID>,<hint>,<definition>
            if(!string.IsNullOrWhiteSpace(response))
            {
                result = response.Split(':');

                if (result.Length != 0 && result[0].Equals("def"))
                {
                    result = result[1].Split(',');

                    //should be length 3+ (some definitions have commas)
                    if (result.Length >= 3)
                    {
                        try
                        {
                            //success!
                            string definition = "";

                            for (int i = 2; i < result.Length; i++ )
                            {
                                definition += result[i];
                                if(i != result.Length-1)
                                {
                                    definition += ",";

                                }

                            }

                            gameList.Add(new Game(Convert.ToInt32(result[0]), definition, result[1]));
                            return gameList[gameList.Count-1];

                        }

                        catch(Exception e)
                        {
                            MessageBox.Show("Failed to parse new game: " + e.Message);

                        }    
                        
                    }

                }

            }

            //error
            else
            {
                MessageBox.Show("Game start error: empty response.");
                return null;

            }

            MessageBox.Show("Game start error: " + response);

            return null;

        }

        //Protocol 2
        public Game TryGuess(Game currentGame)
        {
            bool foundID = false;
            int gameIndex = -1;
            string response = "";

            //send guess
            //should be of format: guess:<gameID>,<word>
            for(int i = 0; i < gameList.Count; i++)
            {
                if(gameList[i].GetGameID() == currentGame.GetGameID())
                {
                    gameList[i].SetGuess(currentGame.GetGuess());

                    serverConnection.Send("guess:"+gameList[i].GetGameID()+","+gameList[i].GetGuess());
                    foundID = true;
                    gameIndex = i;
                    break;

                }

            }

            if (foundID == false)
            {
                MessageBox.Show("Try guess error: game not found.");
                return null;

            }

            //recieve answer/error
            //answer should be of format: answer:<gameID>.<result>,<score>
            System.Threading.Thread.Sleep(200);

            if (serverConnection.Available() > 0)
            {
                response = serverConnection.Receive();

            }

            if(!string.IsNullOrWhiteSpace(response))
            {
                string[] result = response.Split(':');
                if(result.Length > 0)
                {
                    if(result[0].Equals("answer"))
                    {
                        result = result[1].Split(',');

                        if(result[1].Equals("T"))
                        {
                            try
                            {
                                gameList[gameIndex].SetScore(Convert.ToInt32(result[2]));
                                return gameList[gameIndex];

                            }

                            catch(Exception e)
                            {
                                MessageBox.Show("Try guess error, could not parse score: "+e.Message);
                                return null;

                            }

                        }

                        else if(result[1].Equals("F"))
                        {
                            try
                            {
                                gameList[gameIndex].SetNumCorrect(Convert.ToInt32(result[2]));
                                return gameList[gameIndex];

                            }

                            catch(Exception e)
                            {
                                MessageBox.Show("Try guess error, could not parse correct number of letters: "+e.Message);
                                return null;

                            }

                        }

                    }

                }

            }

            else
            {
                MessageBox.Show("Try guess error: empty response.");
                if (gameIndex != -1)
                {
                    return gameList[gameIndex];

                }

                else
                {
                    return null;

                }

            }

            MessageBox.Show("Try guess error: " + response);
            return null;

        }

        //Protocol 3
        public Game GetHint(Game currentGame)
        {
            bool gameFound = false;
            int gameIndex = 0;
            string response = "";

            //get hint
            //should be of format: gethint:<gameID>
            for(int i = 0; i < gameList.Count; i++)
            {
                if(currentGame.GetGameID() == gameList[i].GetGameID())
                {
                    serverConnection.Send("gethint:"+gameList[i].GetGameID());
                    gameFound = true;
                    gameIndex = i;
                    break;

                }

            }

            if(gameFound == false)
            {
                MessageBox.Show("Get hint failed: game does not exist.");
                return null;

            }
            
            //recieve answer/error
            //answer should be of format: hint:<gameID>,<hint>
            System.Threading.Thread.Sleep(200);

            if (serverConnection.Available() > 0)
            {
                response = serverConnection.Receive();

            }

            if(!string.IsNullOrWhiteSpace(response))
            {
                string[] result = response.Split(':');

                if(result.Length == 2)
                {
                    result = result[1].Split(',');

                    if(result.Length == 2)
                    {
                        gameList[gameIndex].SetHint(result[1]);
                        return gameList[gameIndex];

                    }

                }

            }

            else
            {
                MessageBox.Show("Get hint failed: empty response.");
                return null;

            }

            MessageBox.Show("Get hint failed: " + response);
            return null;

        }

        //Protocol 4
        public void EndGame(int gameNumber)
        {
            for(int i = 0; i < gameList.Count; i++)
            {
                if(gameList[i].GetGameID() == gameNumber)
                {
                    serverConnection.Send("exit:" + gameNumber);

                    //wait for potential response
                    System.Threading.Thread.Sleep(200);

                    if(serverConnection.Available() > 0)
                    {
                        string response = serverConnection.Receive();
                        MessageBox.Show("Game end error: " + response);

                    }

                    gameList.Remove(gameList[i]);

                }

            }

        }

        public void ClearGames()
        {
            while(gameList.Count > 0)
            {
                EndGame(gameList[0].GetGameID());
                    
            }

        }

        public List<Game> GetGameList()
        {
            return gameList;

        }

    }

}
