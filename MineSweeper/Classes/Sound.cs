using MineSweeper.Enums;
using MineSweeper.ViewModels;
using MineSweeper.Views.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MineSweeper.Classes
{
    public class Sound
    {
        public SoundPlayer _soundPlayer = new();
        public SoundPlayerStatus Sounds { get; set; } = SoundPlayerStatus.Enabled;

        #region Methods for the sound
        /// <summary>
        /// Plays sound if the status is enabled
        /// </summary>
        /// <param name="mineFieldPiece"></param>
        public void PlayClickSound(MineFieldPiece mineFieldPiece, Visibility messageBoxWin)
        {
            
            if (Sounds == SoundPlayerStatus.Enabled)
            {
                _soundPlayer.Stream = mineFieldPiece.CurrentStatus switch
                {
                    MineFieldPieceStatus.Flagged => Properties.Resources.Flagged,
                    MineFieldPieceStatus.GameoverMine => Properties.Resources.Explosion,
                    _ => Properties.Resources.Digging,
                };
                if (messageBoxWin == Visibility.Visible)
                {
                    _soundPlayer.Stream = Properties.Resources.WinnerSound;
                }
                _soundPlayer.Play();
            }
            
        }

        /// <summary>
        /// Switches status between enabled/disabled
        /// </summary>
        public void MuteAllSound()
        {
            if (Sounds == SoundPlayerStatus.Enabled)
            {
                Sounds = SoundPlayerStatus.Disabled;
            }
            else
            {
                Sounds = SoundPlayerStatus.Enabled;
            }
        }
        #endregion
    }
}
