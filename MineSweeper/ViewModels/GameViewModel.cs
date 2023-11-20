
using MineSweeper.Views.Components;
using MineSweeper.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Accessibility;
using System.Windows.Threading;
using System.Windows.Input;
using MineSweeper.Commands;

using System.Windows;
using Point = System.Drawing.Point;
using System.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Configuration;
using MineSweeper.ViewModels.Base;
using System.Runtime.CompilerServices;
using MineSweeper.Classes;
using System.Windows.Navigation;

namespace MineSweeper.ViewModels
{
    internal class GameViewModel : BaseViewModel
    {
        public int MineFieldSizeHorizontal { get; set; }
        public int MineFieldSizeVertical { get; set; }
        private GameStatus GameStatus { get; set; } = GameStatus.NewGame;
        private static readonly Sound _sounds = new();
        public ObservableCollection<MineFieldPiece> MineField { get; private set; } = new ObservableCollection<MineFieldPiece>();
        public ObservableCollection<Difficulty> Difficulties { get; set; } = new ObservableCollection<Difficulty>();
        public ICommand LeftClickCommand { get; }
        public ICommand DisplayResultCommand { get; }
        public ICommand RightClickCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand MuteSoundCommand { get; }
        public ICommand DisplayRulesCommand { get; }
        public DispatcherTimer _timers = new();
        public string SecondsLabel { get; set; } = "00";
        public string MinutesLabel { get; set; } = "00";
        public string MilliSecondsLabel { get; set; } = "00";
        public int _seconds = 0;
        public int _minutes = 0;
        public int _milliseconds = 0;
        public Visibility MessageBoxLost { get; set; } = Visibility.Hidden;
        public Visibility MessageBoxWin { get; set; } = Visibility.Hidden;
        public Visibility MessageBoxRules { get; set; } = Visibility.Hidden;

        private Difficulty _selectedDifficulty;

        public Difficulty SelectedDifficulty
        {
            get { return _selectedDifficulty; }
            set 
            {
                _selectedDifficulty = value;
                RestartGame();
            }
        }
        public static Cursor Cursor { get; set; } = new Cursor(Application.GetResourceStream(new Uri("/Images/AnimatedCursorDig.ani", UriKind.Relative)).Stream);

        public Cursor GameArea { get; set; }

        public int NumberOfFlags { get; set; }

        public GameViewModel()
        {
            FillDifficulty();
            ChangeGameByDifficulty();
            LeftClickCommand = new RelayCommand(x => LeftClick(x), predicate: x => DisableLeftClick(x));
            RightClickCommand = new RelayCommand(x => RightClick(x), predicate: x => DisableRightClick(x));
            RestartCommand = new RelayCommand(x => RestartGame(),predicate: x => true);
            DisplayResultCommand = new RelayCommand(x => DisplayResult(), predicate: x => true);
            MuteSoundCommand = new RelayCommand(x => _sounds.MuteAllSound(), predicate: x => true);
            DisplayRulesCommand = new RelayCommand(x => DisplayRules(), predicate: x => true);
            GameArea = Cursor;
        }

        #region Methods for restarting the game
        /// <summary>
        /// This Method restarts the game witch includes: Clearing lists of mines and minefield, resets timer, checks what difficulty the player has chosen and change the game based on that and sets the gamestatus back to new game.
        /// </summary>
        private void RestartGame()
        {
            GameEngine.Instance._mines.Clear();
            MineField.Clear();
            GameEngine.MineField.Clear();
            ResetTimer();
            ChangeGameByDifficulty();
            GameStatus = GameStatus.NewGame;
            MessageBoxLost = Visibility.Hidden;
            MessageBoxWin = Visibility.Hidden;
        }

        #endregion

        #region Methods of profits and loses
        /// <summary>
        /// Stops the timer, shows a messagebox with the time and plays the winning sound.
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        private void GameIsWon(MineFieldPiece currentMineFieldPiece)
        {
            StopTimer();
            MessageBoxWin = Visibility.Visible;
            _sounds.PlayClickSound(currentMineFieldPiece, MessageBoxWin);
        }
        /// <summary>
        /// Shows the result of mines and flags for the user, set status of minefieldpiece to gameovermine, stops the timer, plays the losing sound and shows the losing end screen.
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        private void GameIsLost(MineFieldPiece currentMineFieldPiece)
        {
            DisplayResultOfMinesAndFlags();
            currentMineFieldPiece.CurrentStatus = MineFieldPieceStatus.GameoverMine;
            _sounds.PlayClickSound(currentMineFieldPiece, MessageBoxWin);
            StopTimer();
            MessageBoxLost = Visibility.Visible;         
        }
        /// <summary>
        /// Shows all mines position and shows whether the flag placements are correct or incorrect.
        /// </summary>
        private void DisplayResultOfMinesAndFlags()
        {
            var flaggedMineFieldPieces = CollectFlaggedPieces();
            ExposeAllMines();
            CheckIfFlagsArePlacedCorrectly(flaggedMineFieldPieces);
        }

        /// <summary>
        /// Set status of all minefieldpieces thats have a mine to status mine which shows the position of all the mines in the minefield
        /// </summary>
        private void ExposeAllMines()
        {
            foreach (var mineFieldPiece in MineField)
            {
                if (GameEngine.Instance.DoPieceContainMine(mineFieldPiece))
                {
                    mineFieldPiece.CurrentStatus = MineFieldPieceStatus.Mine;                   
                }
            }
            
        }
        #endregion

        #region Methods for flags

        /// <summary>
        /// Gets the number of flags by the chosen difficulty. easy = 10 flags medium = 40 flags and hard = 70 flags. 
        /// </summary>
        /// <returns>returns the chosen difficulties flags</returns>
        private int GetNumberOfFlags()
        {
            switch (_selectedDifficulty)
            {
                case Difficulty.Easy:
                    NumberOfFlags = 10;
                    break;
                case Difficulty.Medium:
                    NumberOfFlags = 40;
                    break;
                case Difficulty.Hard:
                    NumberOfFlags = 70;
                    break;
            }
            return NumberOfFlags;
        }
        /// <summary>
        /// Puts the flagged minefieldpieces in a list, if a piece is flagged then it put in the list.
        /// </summary>
        /// <returns> The list of all flagged minefieldpieces </returns>
        private List<MineFieldPiece> CollectFlaggedPieces()
        {
            List<MineFieldPiece> flaggedMineFieldPieces = new();
            foreach (var mineFieldPiece in MineField)
            {
                if (mineFieldPiece.CurrentStatus == MineFieldPieceStatus.Flagged)
                {
                    flaggedMineFieldPieces.Add(mineFieldPiece);
                }
            }
            return flaggedMineFieldPieces;
        }
        /// <summary>
        /// Checks if flagged minefieldpieces in the list is correct flagged, so we later can show the player if the flag was correct or incorrect placed
        /// </summary>
        /// <param name="flaggedMineFieldPieces"></param>
        private static void CheckIfFlagsArePlacedCorrectly(List<MineFieldPiece> flaggedMineFieldPieces)
        {
            foreach (var mineFieldPiece in flaggedMineFieldPieces)
            {
                if (GameEngine.Instance.DoPieceContainMine(mineFieldPiece))
                {
                    mineFieldPiece.CurrentStatus = MineFieldPieceStatus.CorrectFlagged;
                }
                else
                {
                    mineFieldPiece.CurrentStatus = MineFieldPieceStatus.IncorrectFlagged;
                }
            }
        }
        #endregion

        #region Methods for the timer
        /// <summary>
        /// Shows the time and sets the correct time type when the correct amount of time has passed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TimerTicker(object sender, EventArgs e)
        {
            if (_timers.IsEnabled) //när klockan startas
            {
                _milliseconds++;
                if (_milliseconds >= 60) //betyder att en sekund har passerat
                {
                    _seconds++;
                    _milliseconds = 0;

                    if (_seconds >= 60) //en minut har passerat
                    {
                        _minutes++;
                        _seconds = 0;
                    }
                }
            }
            DisplayTime();
        }
        /// <summary>
        /// Displays the time and allways sets the format to 00:00
        /// </summary>
        public void DisplayTime()
        {
            SecondsLabel = _seconds.ToString();
            SecondsLabel = String.Format("{0:00}", _seconds);
            MinutesLabel = _minutes.ToString();
            MinutesLabel = String.Format("{0:00}", _minutes);
            MilliSecondsLabel = _milliseconds.ToString();
            MilliSecondsLabel = String.Format("{0:00}", _milliseconds);
        }
        /// <summary>
        /// Sets the interval for the timer and starts the timer
        /// </summary>
        public void StartTimer()
        {
            _timers.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timers.Tick += TimerTicker;
            _timers.Start();
        }
        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timers.Stop();
        }
        /// <summary>
        /// Resets the timer
        /// </summary>
        public void ResetTimer()
        {
            _timers.Stop();
            _seconds = 0;
            _minutes = 0;
            _milliseconds = 0;
            _timers.Tick -= TimerTicker;
            DisplayTime();
        }
        #endregion

        #region Methods used to fill gameboard
        /// <summary>
        /// Adding three different difficulties to the list that will be shown in the combobox for the player
        /// </summary>
        private void FillDifficulty()
        {
            Difficulties.Add(Difficulty.Easy);
            Difficulties.Add(Difficulty.Medium);
            Difficulties.Add(Difficulty.Hard);
        }
        /// <summary>
        /// Changes the gameboard depending on chosen difficulty and adds the correct amount of flags
        /// </summary>
        private void ChangeGameByDifficulty()
        {
            SetMinefieldSize();
            FillMineField();
            GetNumberOfFlags();
        }
        /// <summary>
        /// Determines the size of the minefield depending on chosen difficulty
        /// </summary>
        public void SetMinefieldSize()
        {
            switch (_selectedDifficulty)
            {
                case Difficulty.Easy:
                    MineFieldSizeHorizontal = 9;
                    MineFieldSizeVertical = 9;
                    GameEngine.MineFieldSize = 9;
                    break;
                case Difficulty.Medium:
                    MineFieldSizeHorizontal = 14;
                    MineFieldSizeVertical = 14;
                    GameEngine.MineFieldSize = 14;
                    break;
                case Difficulty.Hard:
                    MineFieldSizeHorizontal = 19;
                    MineFieldSizeVertical = 19;
                    GameEngine.MineFieldSize = 19;
                    break;
            }
        }
        /// <summary>
        /// Fill the gameboard with minefieldpieces depending on chosen difficulty
        /// </summary>
        public void FillMineField()
        {
            for (int x = 0; x < MineFieldSizeHorizontal; x++)
            {
                for (int y = 0; y < MineFieldSizeVertical; y++)
                {
                    var currentMineFieldPiece = new MineFieldPiece()
                    {
                        X = x,
                        Y = y,
                        Id = MineField.Count,
                    };
                    MineField.Add(currentMineFieldPiece);
                    GameEngine.MineField.Add(currentMineFieldPiece);
                }
            }
        }        
        #endregion

        #region Methods for ClickCommands
        /// <summary>
        /// If a minefieldpiece is flagged or has been revealed you cant leftclick on that piece
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool DisableLeftClick(object x)
        {
            foreach (var mineFieldPiece in MineField)
            {
                if (mineFieldPiece.Id == (int)x)
                {
                    if (mineFieldPiece.CurrentStatus == MineFieldPieceStatus.Flagged)
                    {
                        return false;
                    }
                    else if (mineFieldPiece.CurrentStatus != MineFieldPieceStatus.Hidden)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Makes sure you can only place flags on minefieldpieces that hasn't been leftclicked
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool DisableRightClick(object x)
        {
            var clickedMineFieldPiece = MineField.First(m => m.Id == (int)x);
            var flaggedMineFieldPieces = CollectFlaggedPieces();

            foreach (var mineFieldPiece in MineField)
            {
                if (mineFieldPiece.Id == clickedMineFieldPiece.Id)
                {
                    if (mineFieldPiece.CurrentStatus == MineFieldPieceStatus.Flagged)
                    {
                        return true;
                    }
                    if (flaggedMineFieldPieces.Count >= GameEngine.Instance._mines.Count)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Places/removes a flag from a minefieldpiece depending on the pieces status. Also removes/ads to the number of flags a player can place
        /// </summary>
        /// <param name="x"></param>
        private void RightClick(object x)
        {
            var clickedMineFieldPiece = MineField.First(m => m.Id == (int)x);

            if (clickedMineFieldPiece.CurrentStatus == MineFieldPieceStatus.Flagged)
            {
                clickedMineFieldPiece.CurrentStatus = MineFieldPieceStatus.Hidden;
                NumberOfFlags++;
            }
            else if (clickedMineFieldPiece.CurrentStatus == MineFieldPieceStatus.Hidden)
            {
                clickedMineFieldPiece.CurrentStatus = MineFieldPieceStatus.Flagged;
                NumberOfFlags--;
                _sounds.PlayClickSound(clickedMineFieldPiece, MessageBoxWin);
            }
        }
        /// <summary>
        /// Starts the timer if it isn't enabled, changes gamestatus to started game, and manages the clicked minefieldpiece
        /// </summary>
        /// <param name="x"></param>
        private void LeftClick(object x)
        {
            if (!_timers.IsEnabled)
            {
                StartTimer();
            }
            var clickedMineFieldPiece = MineField.First(m => m.Id == (int)x);
            if (GameStatus == GameStatus.NewGame)
            {
                GameEngine.Instance.StartTheGame(SelectedDifficulty, clickedMineFieldPiece);
                GameStatus = GameStatus.StartedGame;
                ManageClickedMineFieldPiece(clickedMineFieldPiece);
            }
            else
            {
                ManageClickedMineFieldPiece(clickedMineFieldPiece);
            }
        }
        #endregion

        #region Methods for Clicked MineFieldPiece
        /// <summary>
        /// Managing the clciked minefieldpiece and checks if the piece contains mine otherwise the game continues and the piece gets its number.
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        private void ManageClickedMineFieldPiece(MineFieldPiece currentMineFieldPiece)
        {
            if (GameEngine.Instance.IsTheGameLost(currentMineFieldPiece))
            {
                GameIsLost(currentMineFieldPiece);
            }
            else
            {
                _sounds.PlayClickSound(currentMineFieldPiece, MessageBoxWin);
                SetMineFieldPieceNumber(currentMineFieldPiece);
            }
        }

        /// <summary>
        /// Decide and present the value of the clicked minefieldpiece
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        public void SetMineFieldPieceNumber(MineFieldPiece currentMineFieldPiece)
        {
            var listAdjacentMineFieldPieces = GameEngine.Instance.CollectAdjacentMineFieldPieces(currentMineFieldPiece);
            currentMineFieldPiece.CurrentStatus = GameEngine.Instance.CountAdjacentMines(listAdjacentMineFieldPieces);
            if (GameEngine.Instance.IsTheGameWon(currentMineFieldPiece))
            {
                GameIsWon(currentMineFieldPiece);
            }
            if (currentMineFieldPiece.CurrentStatus == MineFieldPieceStatus.ZeroAdjacentMines)
            {
                ManageZeroAdjacentMine(listAdjacentMineFieldPieces);
            }
        }
        /// <summary>
        /// if the piece has zero adjacent mine then the method checks the closest 8 pieces to see if they also have zero adjacent mines. The method keeps going until all adjecent minfieldpieces havent zero adjacent mines, 
        /// </summary>
        /// <param name="listAdjacentMineFieldPieces"></param>
        public void ManageZeroAdjacentMine(List<MineFieldPiece> listAdjacentMineFieldPieces)
        {
            foreach (var adjacentMineFieldPiece in listAdjacentMineFieldPieces)
            {
                if (adjacentMineFieldPiece.CurrentStatus != MineFieldPieceStatus.Hidden)
                {
                    continue;
                }
                else
                {
                    SetMineFieldPieceNumber(adjacentMineFieldPiece);
                }
            }
        }

        #endregion

        #region Methods for messageboxes

        /// <summary>
        /// Show user instructions when user press button
        /// </summary>
        /// <returns></returns>
        public void DisplayRules()
        {

            if (MessageBoxRules == Visibility.Hidden)
            {
                MessageBoxRules = Visibility.Visible;
            }
            else
            {
                MessageBoxRules = Visibility.Hidden;
            }

        }
        /// <summary>
        /// Closes the messabox for loss/win and makes the gamefield/minefieldpieces unable to be clicked.
        /// </summary>
        private void DisplayResult()
        {
            if (MessageBoxLost == Visibility.Visible)
            {
                MessageBoxLost = Visibility.Hidden;
            }
            if (MessageBoxWin == Visibility.Visible)
            {
                MessageBoxWin = Visibility.Hidden;
            }
            foreach (var mineFieldPiece in MineField)
            {
                mineFieldPiece.IsHitTestVisible = false;
            }
        }

        #endregion
    }
}



