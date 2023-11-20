using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MineSweeper.Enums
{
    public enum MineFieldPieceStatus
    {
        ZeroAdjacentMines, 
        OneAdjacentMine,
        TwoAdjacentMines,
        ThreeAdjacentMines,
        FourAdjacentMines,
        FiveAdjacentMines,
        SixAdjacentMines,
        SevenAdjacentMines,
        EightAdjacentMines,
        Hidden,
        GameoverMine,
        Mine,
        Flagged,
        IncorrectFlagged,
        CorrectFlagged,
    }
    
}
