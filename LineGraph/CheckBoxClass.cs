using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LCRGame.LineGraph
{
    class CheckBoxClass : NotifierBase
    {
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        private bool _isChecked = true;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                SetProperty(ref _isChecked, value);
            }
        }

        private SolidColorBrush _backColor = Brushes.Red;
        public SolidColorBrush BackColor
        {
            get { return _backColor; }
            set
            {
                SetProperty(ref _backColor, value);
            }
        }
    }
}
