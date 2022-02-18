using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LCRGame.Model
{
    class AppMainModel
    {
        public int? winner = null;
        bool gameOver = false;
        public int RunSim(int numPlayers)
        {
            winner = null;
            gameOver = false;
            int centerPot = 0;
            int turns = 0;
            List<Player> players = new List<Player>();

            for(int x = 1; x <= numPlayers; x++)
            {
                players.Add(new Player(x));
            }

            while (!gameOver)
            {
                for(int i = 0; i < players.Count; i++)
                {
                    PlayTurn(players[i], players, i, centerPot);
                    turns++;
                }
            }
            return turns;
        }

        public void PlayTurn(Player p, List<Player> players, int pIndex, int centerPot)
        {
            Random rnd = new Random();
            
            for(int i = 0; i < p.CountChips(); i++)
            {
                int roll = rnd.Next(1, 7);
                if(roll < 4)
                {
                    //This is a dot, do nothing
                }
                else if(roll == 4)
                {
                    //This is a L, pass a chip left
                    //Left is index + 1

                    p.PassChip();
                    if (pIndex == players.Count - 1) players[0].RecieveChip();
                    else players[pIndex + 1].RecieveChip();
                }
                else if(roll == 5)
                {
                    //This is a C, put a chip in center pot
                    p.PassChip();
                    centerPot++; //It doesnt actually matter how much is in the center pot, it is not relevant to the win scenario. Tracking anyway
                }
                else if(roll == 6)
                {
                    //this is a R, pass a chip right
                    //Right is index - 1

                    p.PassChip();
                    if (pIndex == 0) players[players.Count - 1].RecieveChip();
                    else players[pIndex - 1].RecieveChip();
                }
                else
                {
                    //This is an invalid roll
                }
            }

            gameOver = IsGameOver(players);
        }

        public bool IsGameOver(List<Player> players)
        {
            int? victor = null;

            int flag = 0; //Used to determine if a player has chips
            for(int index = 0; index < players.Count; index++)
            {
                if(players[index].CountChips() > 0)
                {
                    if (flag == 1) return false;
                    flag = 1;
                    victor = index;
                }
            }
            winner = victor;
            return true;
        }

        internal class Player
        {
            int chips;
            int playerNumber;

            public Player(int x)
            {
                chips = 3;
                playerNumber = x;
            }

            public void PassChip()
            {
                chips--;
            }

            public void RecieveChip()
            {
                chips++;
            }

            public int CountChips()
            {
                return chips;
            }

            public int GetPlayerNumber()
            {
                return playerNumber;
            }
        }
    }
}
