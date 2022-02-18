using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCRGame.LineGraph
{
    public class DataPoint : NotifierBase
    {
        private double _turns = new double();
        public double Turns
        {
            get { return _turns; }
            set
            {
                SetProperty(ref _turns, value);
            }
        }

        private double _game = new double();
        public double Game
        {
            get { return _game; }
            set
            {
                SetProperty(ref _game, value);
            }
        }
    }
}
