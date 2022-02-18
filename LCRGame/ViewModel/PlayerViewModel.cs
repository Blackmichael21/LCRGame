using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LCRGame.ViewModel
{
    class PlayerViewModel : ViewModelBase
    {
        private int _playerNumber;
        private string _playerNumberText;
        private bool _isWinner = false;
        private SolidColorBrush _fill = Brushes.White;

        public int PlayerNumber
        {
            get { return _playerNumber; }
            set
            {
                if(_playerNumber != value)
                {
                    _playerNumber = value;
                    OnPropertyChanged(nameof(PlayerNumber));
                    PlayerNumberText = "Player " + _playerNumber.ToString();
                }
            }
        }
        public string PlayerNumberText
        {
            get { return _playerNumberText; }
            set
            {
                if(value != _playerNumberText)
                {
                    _playerNumberText = value;
                    OnPropertyChanged(nameof(PlayerNumberText));
                }
            }
        }

        public bool IsWinner
        {
            get { return _isWinner; }
            set
            {
                if(value != _isWinner)
                {
                    _isWinner = value;
                    OnPropertyChanged(nameof(IsWinner));
                    if(value == true)
                    {
                        Fill = Brushes.Green;
                    }
                    else
                    {
                        Fill = Brushes.White;
                    }
                }
            }
        }

        public SolidColorBrush Fill
        {
            get { return _fill; }
            set
            {
                if(value != _fill)
                {
                    _fill = value;
                    OnPropertyChanged(nameof(Fill));
                }
            }
        }
    }
}
