using MineSweeper.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MineSweeper.Converters
{
    internal class StatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case MineFieldPieceStatus when value != null:
                    {
                        var status = (MineFieldPieceStatus)value;
                        return status switch
                        {
                            MineFieldPieceStatus.Hidden => new Uri(@"/Images/MineFieldPieceImages/Hidden.png", UriKind.Relative),
                            MineFieldPieceStatus.ZeroAdjacentMines => new Uri(@"/Images/MineFieldPieceImages/0.png", UriKind.Relative),
                            MineFieldPieceStatus.Mine => new Uri(@"/Images/MineFieldPieceImages/Mine.png", UriKind.Relative),
                            MineFieldPieceStatus.Flagged => new Uri(@"/Images/MineFieldPieceImages/Flagged.png", UriKind.Relative),
                            MineFieldPieceStatus.GameoverMine => new Uri(@"/Images/MineFieldPieceImages/GameOverMine.png", UriKind.Relative),
                            MineFieldPieceStatus.IncorrectFlagged => new Uri(@"/Images/MineFieldPieceImages/IncorrectFlagged.png", UriKind.Relative),
                            MineFieldPieceStatus.CorrectFlagged => new Uri(@"/Images/MineFieldPieceImages/CorrectFlagged.png", UriKind.Relative),
                            MineFieldPieceStatus.OneAdjacentMine => new Uri(@"/Images/MineFieldPieceImages/1.png", UriKind.Relative),
                            MineFieldPieceStatus.TwoAdjacentMines => new Uri(@"/Images/MineFieldPieceImages/2.png", UriKind.Relative),
                            MineFieldPieceStatus.ThreeAdjacentMines => new Uri(@"/Images/MineFieldPieceImages/3.png", UriKind.Relative),
                            MineFieldPieceStatus.FourAdjacentMines => new Uri(@"/Images/MineFieldPieceImages/4.png", UriKind.Relative),
                            MineFieldPieceStatus.FiveAdjacentMines => new Uri(@"/Images/MineFieldPieceImages/5.png", UriKind.Relative),
                            MineFieldPieceStatus.SixAdjacentMines => new Uri(@"/Images/MineFieldPieceImages/6.png", UriKind.Relative),
                            MineFieldPieceStatus.SevenAdjacentMines => new Uri(@"/Images/MineFieldPieceImages/7.png", UriKind.Relative),
                            MineFieldPieceStatus.EightAdjacentMines => new Uri(@"/Images/MineFieldPieceImages/8.png", UriKind.Relative),
                            _ => new Uri(@"/Images/MineFieldPieceImages/Hidden.png", UriKind.Relative)
                        };
                    }

                default:
#pragma warning disable CS8603 // Possible null reference return.
                    return value;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
