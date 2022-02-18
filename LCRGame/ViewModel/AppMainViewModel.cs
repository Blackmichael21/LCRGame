using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LCRGame.Commands;

namespace LCRGame.ViewModel
{
    class AppMainViewModel : ViewModelBase
    {

        #region Private Members

        private Model.AppMainModel model = new Model.AppMainModel(); //Reference to the model that handles back end logic
        private int runningSum = 0; //Used to track a running sum of turns, later to get the average


        private int _numPlayers; //Set game parameter for player count
        private int _numGames; //Set parameter for number of games to iterate
        private int _numTurns; //Count how many turns in current game
        private int _shortGame; //Record shortest game
        private int _longGame; //Record longest game
        private int _avgTurns; //Record average turns
        private int _longTurns; //Record number of turns in longest game
        private int _shortTurns; //Record number of turns in shortest game
        private int _presetSelection; //This is the selected index of the combo box that holds the presets in the UI
        private int _graphIncrements = 1; //This is a integer to determine what x-values to label on the graph
        private int _winner; //The number of the player who won the most games
        private string _winnerText = "Let's see which player wins the most games!"; //The text for the banner above the players pannel
        private ObservableCollection<PlayerViewModel> _players; //the source list for the players panel in the UI

        //This collection contains the lines that will be on the graph. 
        //One line for the game results, one for the average, one for the high, and one for the low
        private ObservableCollection<LineGraph.LineSeries> _graphData;
        
        #endregion

        //Properties will be used for data binding, they directly reference a private member
        #region Public Properties
        public int NumPlayers
        {
            get { return _numPlayers; }
            set
            {
                //If the value the user is trying to set as number of players is less than three
                //then it is under the min requirement.
                if(value <= 2)
                {
                    MessageBox.Show("The mininum number of players required to play is 3.");
                    _numPlayers = 3;
                }
                else if (value != _numPlayers)
                {
                    _numPlayers = value;
                    OnPropertyChanged(nameof(NumPlayers));
                    PopulatePlayerViews(_numPlayers);
                }
            }
        }

        public int NumGames
        {
            get { return _numGames; }
            set
            {
                if (value != _numGames)
                {
                    _numGames = value;
                    OnPropertyChanged(nameof(NumGames));
                    if(_numGames >= 10001)
                    {
                        MessageBox.Show("Due to the amount of points, sometimes the process of drawing the graph stalls when there are more than 10k games. The game logic still completes.");
                    }
                }
            }
        }

        public int NumTurns
        {
            get { return _numTurns; }
            set
            {
                if (value != _numTurns)
                {
                    _numTurns = value;
                    OnPropertyChanged(nameof(NumTurns));
                }
            }
        }

        public int ShortGame
        {
            get { return _shortGame; }
            set
            {
                if (value != _shortGame)
                {
                    _shortGame = value;
                    OnPropertyChanged(nameof(ShortGame));
                }
            }
        }

        public int LongGame
        {
            get { return _longGame; }
            set
            {
                if (value != _longGame)
                {
                    _longGame = value;
                    OnPropertyChanged(nameof(LongGame));
                }
            }
        }

        public int AvgTurns
        {
            get { return _avgTurns; }
            set
            {
                if (value != _avgTurns)
                {
                    _avgTurns = value;
                    OnPropertyChanged(nameof(AvgTurns));
                }
            }
        }

        public int LongTurns
        {
            get { return _longTurns; }
            set
            {
                if (value != _longTurns)
                {
                    _longTurns = value;
                    OnPropertyChanged(nameof(LongTurns));
                }
            }
        }

        public int ShortTurns
        {
            get { return _shortTurns; }
            set
            {
                if (value != _shortTurns)
                {
                    _shortTurns = value;
                    OnPropertyChanged(nameof(ShortTurns));
                }
            }
        }

        public int PresetSelection
        {
            get { return _presetSelection; }
            set
            {
                if(value != _presetSelection)
                {
                    _presetSelection = value;

                    //This switch case handles setting the number of players and number of games
                    //when the preset option is used instead of manually setting them
                    switch (_presetSelection) 
                    {
                        case 0: 
                            NumPlayers = 3;
                            NumGames = 100;
                            break;
                        case 1:
                            NumPlayers = 4;
                            NumGames = 100;
                            break;
                        case 2:
                            NumPlayers = 5;
                            NumGames = 100;
                            break;
                        case 3:
                            NumPlayers = 5;
                            NumGames = 1000;
                            break;
                        case 4:
                            NumPlayers = 5;
                            NumGames = 10000;
                            break;
                        case 5:
                            NumPlayers = 5;
                            NumGames = 100000;
                            break;
                        case 6:
                            NumPlayers = 6;
                            NumGames = 100;
                            break;
                        case 7:
                            NumPlayers = 7;
                            NumGames = 100;
                            break;
                    };

                    OnPropertyChanged(nameof(PresetSelection));
                }
            }
        }

        public int GraphIncrements
        {
            get { return _graphIncrements; }
            set
            {
                if(value != _graphIncrements)
                {
                    _graphIncrements = value;
                    OnPropertyChanged(nameof(GraphIncrements));
                }
            }
        }

        public int Winner
        {
            get { return _winner; }
            set
            {
                if(value != _winner)
                {
                    _winner = value;
                    OnPropertyChanged(nameof(Winner));
                    WinnerText = "Player " + (_winner + 1).ToString() + " has won the most games!";
                }
            }
        }

        public string WinnerText
        {
            get { return _winnerText; }
            set
            {
                if(value != _winnerText)
                {
                    _winnerText = value;
                    OnPropertyChanged(nameof(WinnerText));
                }
            }
        }

        public ObservableCollection<LineGraph.LineSeries> GraphData
        {
            get
            {
                if (_graphData == null)
                {
                    _graphData = new ObservableCollection<LineGraph.LineSeries>();
                }
                return _graphData;
            }
        }

        public ObservableCollection<PlayerViewModel> Players
        {
            get
            {
                if(_players == null)
                {
                    _players = new ObservableCollection<PlayerViewModel>();
                }
                return _players;
            }
        }
        #endregion

        public AppMainViewModel()
        {
            //These are defaults for when the game opens
            PresetSelection = 0;
            NumPlayers = 3;
            NumGames = 100;
            PopulatePlayerViews(3);
        }

        //Commands are used for button events
        #region Commands

        //This command fires from the 'Play' button
        private ICommand _runSimCommand;
        public ICommand RunSimCommand
        {
            get
            {
                if (_runSimCommand == null)
                {
                    _runSimCommand = new RelayCommand((ob) => { return true; }, (ob) => { RunGame(); });
                }
                return _runSimCommand;
            }
        }

        //This command fires from the 'Cancel' button
        //It is unclear from the directions what the intent for this button is
        //so for now it just shows a message that the user cancelled
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand((ob) => { return true; }, (ob) => { Cancel(true); });
                }
                return _cancelCommand;
            }
        }

        #endregion

        public void RunGame()
        {
            if (GraphData.Count > 0) Cancel(false);
            LineGraph.LineSeries gameResults = new LineGraph.LineSeries(); //This is the main line on the graph
            gameResults.Name = "Game Results";
            LineGraph.LineSeries avgLine = new LineGraph.LineSeries(); //This is the line to show average turns
            avgLine.Name = "Average Turns";
            LineGraph.LineSeries highGame = new LineGraph.LineSeries(); //This is the line to show high game turns
            highGame.Name = "High Game";
            LineGraph.LineSeries lowGame = new LineGraph.LineSeries(); //This is the line to show low game turns
            lowGame.Name = "Low Game";

            Dictionary<int, int> winners = new Dictionary<int, int>();
            
            //Game count starts at 1 instead of 0, makes it easier for later reporting
            for (int i = 1; i <= NumGames; i++)
            {
                //Runs the sim for one game, based off number of players.
                //Returns number of turns the sim took to complete
                int tempTurns = model.RunSim(NumPlayers);
                if (model.winner != null && winners.Keys.Contains((int)model.winner))
                {
                    winners[(int)model.winner]++;
                }
                else if(model.winner != null)
                {
                    winners[(int)model.winner] = 1;
                }
                if (i == 1) ShortTurns = tempTurns; //Set the first game as the shortest game, otherwise shortest game will always be zero or effectively null

                //If current number of turns is less than shortest amount so far, current turns is the new short game
                if (tempTurns < ShortTurns)
                {
                    ShortTurns = tempTurns;
                    ShortGame = i;
                }

                //If current number of turns is more than longest amount so far, current turns in the new long game
                if (tempTurns > LongTurns)
                {
                    LongTurns = tempTurns;
                    LongGame = i;
                }

                //This is the running sum of turns, this will be used to get the average later
                runningSum += tempTurns;

                //at the end of each iteration we create a data point for that game.
                LineGraph.DataPoint pt = new LineGraph.DataPoint() { Game = i, Turns = tempTurns };
                gameResults.Data.Add(pt);
            }

            AvgTurns = runningSum / gameResults.Data.Count;

            if (NumGames >= 10001)
            {
                MessageBox.Show("Game Stats for large number of sims: " +
                    "\nNumber of Games: " + NumGames.ToString() +
                    "\nNumber of Players: " + NumPlayers.ToString() +
                    "\nAverage Turns: " + AvgTurns.ToString() +
                    "\nLong Game: " + LongGame.ToString() + ", turns: " + LongTurns.ToString() +
                    "\nShort Game: " + ShortGame.ToString() + ", turns: " + ShortTurns.ToString() +
                    "\n\nPress OK to continue to Graphing...");
            }
            //In a more in depth project this should be set by the user
            //For the scope of this project I just set it to ten increments.
            GraphIncrements = NumGames / 10;
            GraphData.Add(gameResults);
            
            highGame.Name += " (" + LongTurns.ToString() + ")";
            lowGame.Name += " (" + ShortTurns.ToString() + ")";
            for (int i = 1; i <= NumGames; i++)
            {
                avgLine.Data.Add(new LineGraph.DataPoint { Turns = AvgTurns, Game = i });
                if(i == LongGame) highGame.Data.Add(new LineGraph.DataPoint() { Turns = LongTurns, Game = i });
                else highGame.Data.Add(new LineGraph.DataPoint() { Turns = double.NaN, Game = i });
                if(i == ShortGame) lowGame.Data.Add(new LineGraph.DataPoint() { Turns = ShortTurns, Game = i });
                else lowGame.Data.Add(new LineGraph.DataPoint() { Turns = double.NaN, Game = i });
            }
            GraphData.Add(avgLine);
            GraphData.Add(highGame);
            GraphData.Add(lowGame);

            Winner = winners.Keys.ElementAt(0);
            foreach(int x in winners.Keys) 
            {
                if (winners[x] > winners[Winner]) Winner = x;
            }

            foreach(PlayerViewModel p in Players)
            {
                if (p.PlayerNumber == Winner + 1) p.IsWinner = true;
            }
        }

        private void Cancel(bool param)
        {
            if(param) MessageBox.Show("Cancelled Game");
            GraphData.Clear();
            runningSum = 0;
            AvgTurns = 0;
            ShortGame = 0;
            ShortTurns = 0;
            LongGame = 0;
            LongTurns = 0;
            PopulatePlayerViews(NumPlayers);
        }

        private void PopulatePlayerViews(int numPlayers)
        {
            Players.Clear();
            for(int i = 1; i <= numPlayers; i++)
            {
                Players.Add(new PlayerViewModel() { PlayerNumber = i, IsWinner = false });
            }
        }
    }
}
