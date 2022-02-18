using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LCRGame.View
{
    /// <summary>
    /// Interaction logic for AppMainView.xaml
    /// </summary>
    public partial class AppMainView : Window
    {
        public AppMainView()
        {
            InitializeComponent();
            LineGraph.ItemsSource = (DataContext as ViewModel.AppMainViewModel).GraphData;
            (DataContext as ViewModel.AppMainViewModel).PropertyChanged += (sender, e) => HandlePropertyChange(sender, e);
        }

        private void HandlePropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "GraphIncrements")
            {
                LineGraph.SkipLabels = (sender as ViewModel.AppMainViewModel).GraphIncrements;
            }

            if(e.PropertyName == "NumGames")
            {
                LineGraph.XMax = (sender as ViewModel.AppMainViewModel).NumGames;
            }

            if(e.PropertyName == "LongTurns")
            {
                LineGraph.YMax = (sender as ViewModel.AppMainViewModel).LongTurns;
            }
        }
    }
}
