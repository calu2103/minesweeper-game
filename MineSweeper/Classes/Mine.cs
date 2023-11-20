using MineSweeper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace MineSweeper.Classes
{
    public class Mine : UserControl
    {
        public int X { get; set; }
        public int Y { get; set; }

        public void SetCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

    }
}
