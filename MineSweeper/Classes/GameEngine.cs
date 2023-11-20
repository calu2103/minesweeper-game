using MineSweeper.Enums;
using MineSweeper.Views.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Classes
{
    public sealed class GameEngine
    {
        private static GameEngine? instance = null;

        private static readonly Random _randoms = new();

        private static int _mineFieldSize;

        public List<Mine> _mines = new();

        private static List<MineFieldPiece> _mineField = new();

        public static GameEngine Instance
        {
            get
            {
                instance ??= new GameEngine();
                return instance;
            }
        }

        public static int MineFieldSize { get => _mineFieldSize; set => _mineFieldSize = value; }
        public static List<MineFieldPiece> MineField { get => _mineField; set => _mineField = value; }

        #region Methods for Profits and loses
        /// <summary>
        /// checks if the game is won. first it checks if the piece is not a mine, if that is true that piece is removed from the list and if minefield has the same amount as mines then the game is won.
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        /// <returns></returns>
        public bool IsTheGameWon(MineFieldPiece currentMineFieldPiece)
        {
            if (!DoPieceContainMine(currentMineFieldPiece))
            {
                MineField.Remove(currentMineFieldPiece);
            }
            if (MineField.Count == _mines.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if the current piece contains a mine if that is true the game is lost, otherwise it continues.
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        /// <returns></returns>
        public bool IsTheGameLost(MineFieldPiece currentMineFieldPiece)
        {
            if (DoPieceContainMine(currentMineFieldPiece))
            {
                return true;
            }
            return false;
        } 
        #endregion

        #region Methods for mine deployment
        /// <summary>
        /// the methods that starts the game by filling mines and placing the mines randomly.
        /// </summary>
        /// <param name="selectedDifficulty"></param>
        /// <param name="currentMineFieldPiece"></param>
        public void StartTheGame(Difficulty selectedDifficulty, MineFieldPiece currentMineFieldPiece)
        {
            FillMines(selectedDifficulty);
            PlaceMinesRandomly(currentMineFieldPiece);
        }

        /// <summary>
        /// Fill the minefield by the chosen difficulty and adding mines.
        /// </summary>
        /// <param name="selectedDifficulty"></param>
        private void FillMines(Difficulty selectedDifficulty)
        {
            Mine mine;
            switch (selectedDifficulty)
            {
                case Difficulty.Easy:
                    for (int i = 0; i < 10; i++)
                    {
                        mine = new Mine();
                        _mines.Add(mine);
                    }
                    break;
                case Difficulty.Medium:
                    for (int i = 0; i < 40; i++)
                    {
                        mine = new Mine();
                        _mines.Add(mine);
                    }
                    break;
                case Difficulty.Hard:
                    for (int i = 0; i < 70; i++)
                    {
                        mine = new Mine();
                        _mines.Add(mine);
                    }
                    break;
            }
        }
        /// <summary>
        /// PLaces mines randomly threw out the minefield. the loop goes on as long untill mineIsAdded = true;
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        private void PlaceMinesRandomly(MineFieldPiece currentMineFieldPiece)
        {
            bool mineIsAdded;
            foreach (var mine in _mines)
            {
                do
                {
                    var point = GetRandomMineFieldPoint();
                    if (IsPointClickedPieceOrAdjacentPieces(currentMineFieldPiece, point))
                    {
                        mineIsAdded = false;
                    }
                    else if (IsPointOccupied(point))
                    {
                        mineIsAdded = false;
                    }
                    else
                    {
                        mine.SetCoordinate(point.X, point.Y);
                        mineIsAdded = true;
                    }
                } while (!mineIsAdded);
            }
        }
        private static System.Drawing.Point GetRandomMineFieldPoint()
        {
            return new System.Drawing.Point(_randoms.Next(MineFieldSize), _randoms.Next(MineFieldSize));
        }
        /// <summary>
        /// Checks if clicked and adjacent pieces have same coordinates as the point
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static bool IsPointClickedPieceOrAdjacentPieces(MineFieldPiece currentMineFieldPiece, System.Drawing.Point point)
        {
            bool occupied;
            if (currentMineFieldPiece.X == point.X && currentMineFieldPiece.Y == point.Y)
            {
                occupied = true;
            }
            else if (currentMineFieldPiece.X - 1 == point.X && currentMineFieldPiece.Y + 1 == point.Y)
            {
                occupied = true;
            }
            else if (currentMineFieldPiece.X == point.X && currentMineFieldPiece.Y + 1 == point.Y)
            {
                occupied = true;
            }
            else if (currentMineFieldPiece.X + 1 == point.X && currentMineFieldPiece.Y + 1 == point.Y)
            {
                occupied = true;
            }
            else if (currentMineFieldPiece.X - 1 == point.X && currentMineFieldPiece.Y == point.Y)
            {
                occupied = true;
            }
            else if (currentMineFieldPiece.X + 1 == point.X && currentMineFieldPiece.Y == point.Y)
            {
                occupied = true;
            }
            else if (currentMineFieldPiece.X - 1 == point.X && currentMineFieldPiece.Y - 1 == point.Y)
            {
                occupied = true;
            }
            else if (currentMineFieldPiece.X == point.X && currentMineFieldPiece.Y - 1 == point.Y)
            {
                occupied = true;
            }
            else if (currentMineFieldPiece.X + 1 == point.X && currentMineFieldPiece.Y - 1 == point.Y)
            {
                occupied = true;
            }
            else
            {
                occupied = false;
            }
            return occupied;
        }
        /// <summary>
        /// checks if the point already has a mine so the piece cannot contain several mines.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        private bool IsPointOccupied(System.Drawing.Point coordinate)
        {
            foreach (var mine in _mines)
            {
                if (mine.X == coordinate.X && mine.Y == coordinate.Y)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Methods for determining the number of adjacent mines 

        /// <summary>
        /// Collects  all the adjacentsminefieldpieces and returns that as a list.
        /// </summary>
        /// <param name="currentMineFieldPiece"></param>
        /// <returns></returns>
        public List<MineFieldPiece> CollectAdjacentMineFieldPieces(MineFieldPiece currentMineFieldPiece)
        {
            var listOfAdjacentMineFieldPieces = new List<MineFieldPiece>();
            foreach (var mineFieldpiece in MineField)
            {
                if (mineFieldpiece.X - 1 == currentMineFieldPiece.X && mineFieldpiece.Y + 1 == currentMineFieldPiece.Y)
                {
                    listOfAdjacentMineFieldPieces.Add(mineFieldpiece);
                }
                if (mineFieldpiece.X == currentMineFieldPiece.X && mineFieldpiece.Y + 1 == currentMineFieldPiece.Y)
                {
                    listOfAdjacentMineFieldPieces.Add(mineFieldpiece);
                }
                if (mineFieldpiece.X + 1 == currentMineFieldPiece.X && mineFieldpiece.Y + 1 == currentMineFieldPiece.Y)
                {
                    listOfAdjacentMineFieldPieces.Add(mineFieldpiece);
                }
                if (mineFieldpiece.X - 1 == currentMineFieldPiece.X && mineFieldpiece.Y == currentMineFieldPiece.Y)
                {
                    listOfAdjacentMineFieldPieces.Add(mineFieldpiece);
                }
                if (mineFieldpiece.X + 1 == currentMineFieldPiece.X && mineFieldpiece.Y == currentMineFieldPiece.Y)
                {
                    listOfAdjacentMineFieldPieces.Add(mineFieldpiece);
                }
                if (mineFieldpiece.X - 1 == currentMineFieldPiece.X && mineFieldpiece.Y - 1 == currentMineFieldPiece.Y)
                {
                    listOfAdjacentMineFieldPieces.Add(mineFieldpiece);
                }
                if (mineFieldpiece.X == currentMineFieldPiece.X && mineFieldpiece.Y - 1 == currentMineFieldPiece.Y)
                {
                    listOfAdjacentMineFieldPieces.Add(mineFieldpiece);
                }
                if (mineFieldpiece.X + 1 == currentMineFieldPiece.X && mineFieldpiece.Y - 1 == currentMineFieldPiece.Y)
                {
                    listOfAdjacentMineFieldPieces.Add(mineFieldpiece);
                }
            }
            return listOfAdjacentMineFieldPieces;
        }
        /// <summary>
        /// counts the adjacents mines to the minefieldpiece.
        /// </summary>
        /// <param name="listAdjacentMineFieldPieces"></param>
        /// <returns></returns>
        public MineFieldPieceStatus CountAdjacentMines(List<MineFieldPiece> listAdjacentMineFieldPieces)
        {
            int count = 0;
            foreach (var mineFieldPiece in listAdjacentMineFieldPieces)
            {
                if (DoPieceContainMine(mineFieldPiece))
                {
                    count++;
                }
            }
            return (MineFieldPieceStatus)count;
        }
        /// <summary>
        /// </summary>
        /// Checks i the clicked piece contains a mine.
        /// <param name="currentMineFieldPiece"></param>
        /// <returns></returns>
        public bool DoPieceContainMine(MineFieldPiece currentMineFieldPiece)
        {
            foreach (var mine in _mines)
            {
                if (currentMineFieldPiece.X == mine.X && currentMineFieldPiece.Y == mine.Y)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

}
