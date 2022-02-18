/*~~~CREDIT~~~
 * I followed a blog posted on the Code Project Website
 * when creating the line graph. This was a tool I had not 
 * experienced creating before, so I did some research for help.
 * The blog discusses how to make the line graph as well as reasons 
 * for doing certain methods and creating certain dependency properties
 * The blog and BasicChart code are both open source under
 * Code Project's Open Source License
 * 
 * I did make changes to make the graph more suitable for this project
 * as well as fix some bugs that were overlooked in the blog
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LCRGame.LineGraph.Controls
{
    public partial class LineChartBase : UserControl
    {
        //This a list of the possible colors for lines on the graph.
        private static List<SolidColorBrush> DistinctColorList = new List<SolidColorBrush>();

        public LineChartBase()
        {
            InitializeComponent();

            //Could create a system to add random colors, but for the scope of this project I am just adding these three.
            DistinctColorList.Add(Brushes.Red);
            DistinctColorList.Add(Brushes.Green);
            DistinctColorList.Add(Brushes.Blue);
            DistinctColorList.Add(Brushes.Orange);
            DistinctColorList.Add(Brushes.Magenta);
        }

        private ObservableCollection<YaxisLabelControl> YItems
        {
            get { return (ObservableCollection<YaxisLabelControl>)GetValue(YItemsProperty); }
            set { SetValue(YItemsProperty, value); }
        }

        private static readonly DependencyProperty YItemsProperty =
            DependencyProperty.Register("YItems", typeof(ObservableCollection<YaxisLabelControl>), typeof(LineChartBase), new PropertyMetadata(new ObservableCollection<YaxisLabelControl>()));

        private ObservableCollection<XaxisLabelControl> XItems
        {
            get { return (ObservableCollection<XaxisLabelControl>)GetValue(XItemsProperty); }
            set { SetValue(XItemsProperty, value); }
        }

        private static readonly DependencyProperty XItemsProperty =
            DependencyProperty.Register("XItems", typeof(ObservableCollection<XaxisLabelControl>), typeof(LineChartBase), new PropertyMetadata(new ObservableCollection<XaxisLabelControl>()));

        private ObservableCollection<CheckBoxClass> CurveVisibility
        {
            get { return (ObservableCollection<CheckBoxClass>)GetValue(CurveVisibilityProperty); }
            set { SetValue(CurveVisibilityProperty, value); }
        }

        private static readonly DependencyProperty CurveVisibilityProperty =
            DependencyProperty.Register("CurveVisibility", typeof(ObservableCollection<CheckBoxClass>), typeof(LineChartBase), new PropertyMetadata(new ObservableCollection<CheckBoxClass>()));

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        private static void SetupYAxis(LineChartBase d)
        {
            var MyClass = (LineChartBase)d;

            if (MyClass.AutoScale)
            {
                double TempYMax, TempYMin;
                TempYMax = double.MinValue;
                TempYMin = double.MaxValue;

                if (MyClass.ItemsSource == null) return;
                foreach (var ClassItem in MyClass.ItemsSource)
                {
                    IEnumerable MyCollectionItem = (IEnumerable)GetPropValue(ClassItem, MyClass.DataCollectionName);
                    foreach (var item in MyCollectionItem)
                    {
                        double value = (double)GetPropValue(item, MyClass.DisplayMemberValues);
                        if (value < TempYMin) TempYMin = value;
                        if (value > TempYMax) TempYMax = value;
                    }
                }

                MyClass.YMax = TempYMax + (TempYMax - TempYMin) * .05d;
                MyClass.YMin = TempYMin - (TempYMax - TempYMin) * .05d;
            }

            MyClass.YItems = new ObservableCollection<YaxisLabelControl>();

            double deltaY = (MyClass.YMax - MyClass.YMin) / (MyClass.NumberOfYSteps);

            for (int i = 0; i <= MyClass.NumberOfYSteps; i++)
            {
                var YLabelObject = new YaxisLabelControl();
                if((MyClass.YMin + (double)i * deltaY).ToString(MyClass.DoubleToString).Contains("."))
                {
                    YLabelObject.YLabel = (MyClass.YMin + (double)i * deltaY).ToString(MyClass.DoubleToString).Remove((MyClass.YMin + (double)i * deltaY).ToString(MyClass.DoubleToString).IndexOf("."));
                }
                else
                {
                    YLabelObject.YLabel = (MyClass.YMin + (double)i * deltaY).ToString(MyClass.DoubleToString);
                }
                
                YLabelObject.YLocation = ((double)i) * (double)MyClass.PlotHeight / (double)MyClass.NumberOfYSteps;
                MyClass.YItems.Add(YLabelObject);
            }
        }

        private static void SetupXAxis(LineChartBase d)
        {
            ObservableCollection<XaxisLabelControl> Xitems = new ObservableCollection<XaxisLabelControl>();

            var MyBasicChart = (LineChartBase)d;
            if (MyBasicChart.ItemsSource == null) return;

            int count = 0;
            foreach (var item in MyBasicChart.ItemsSource)
            {
                count += 1;
            }

            if (count == 0) return;

            IEnumerable MyItemsSource = MyBasicChart.ItemsSource;

            var GetItemsSourceEnumerator = MyItemsSource.GetEnumerator();
            GetItemsSourceEnumerator.MoveNext();

            var FirstItemsSource = GetItemsSourceEnumerator.Current;

            IEnumerable test2 = (IEnumerable)GetPropValue(FirstItemsSource, MyBasicChart.DataCollectionName);
            int ItemsNumber = 0;

            foreach (var item in test2)
            {
                var Xlables = GetPropValue(item, MyBasicChart.DisplayMemberLabels);
                if (MyBasicChart.XMin == MyBasicChart.XMax)
                {
                    // Set up the X labels
                    var XLabelObject = new XaxisLabelControl();
                    if ((MyBasicChart.StartSkipAt <= ItemsNumber) &&
                        (ItemsNumber - MyBasicChart.StartSkipAt) % MyBasicChart.SkipLabels == 0)
                        XLabelObject.XLabel = Xlables.ToString();

                    Xitems.Add(XLabelObject);
                }
                else
                {
                    if (MyBasicChart.XMin <= ItemsNumber && ItemsNumber <= MyBasicChart.XMax)
                    {
                        // Set up the X labels
                        var XLabelObject = new XaxisLabelControl();

                        if ((MyBasicChart.StartSkipAt <= ItemsNumber) &&
                            (ItemsNumber - MyBasicChart.StartSkipAt) % MyBasicChart.SkipLabels == 0)
                            XLabelObject.XLabel = Xlables.ToString();

                        Xitems.Add(XLabelObject);

                    }
                }

                ItemsNumber++;
            }

            for (int i = 0; i < Xitems.Count; i++)
            {
                double pos = (double)i * (double)MyBasicChart.PlotWidth / (double)Xitems.Count;
                Xitems[i].LabelAngle = 45;
                Xitems[i].XLocation = pos;
            }

            MyBasicChart.XAxis.ItemsSource = Xitems;
        }

        private static void SetupGraph(LineChartBase sender, IEnumerable ItemsSource)
        {
            List<PointCollection> ListOfChartCurves = new List<PointCollection>();
            List<double> origianlValues = new List<double>();

            if (ItemsSource == null) return;

            //Loop through all the sources
            foreach (var ClassItem in ItemsSource)
            {
                PointCollection PointsOnChart = new PointCollection();
                int X = 0;

                // Get the Collection of dataitems from the current source
                IEnumerable MyCollectionItem = (IEnumerable)GetPropValue(ClassItem, sender.DataCollectionName);

                // For all the chart points, get the relevant Y values
                foreach (var item in MyCollectionItem)
                {
                    var YValues = GetPropValue(item, sender.DisplayMemberValues);

                    // No X value filters are applied
                    if (sender.XMin == sender.XMax)
                    {
                        if (YValues is double)
                        {
                            origianlValues.Add((double)YValues);
                            if (double.IsInfinity((double)YValues) || double.IsNaN((double)YValues) || double.IsNegativeInfinity((double)YValues) || double.IsPositiveInfinity((double)YValues))
                            {
                                PointsOnChart.Add(new Point(0, double.NaN));
                            }
                            else
                            {
                                double YValue = ((double)YValues - sender.YMin) / (sender.YMax - sender.YMin) * sender.PlotHeight;
                                PointsOnChart.Add(new Point(0, YValue));
                            }
                        }
                    }
                    else if (sender.XMin <= X && X <= sender.XMax)
                    {
                        if (YValues is double)
                        {
                            origianlValues.Add((double)YValues);
                            if (double.IsInfinity((double)YValues) || double.IsNaN((double)YValues) || double.IsNegativeInfinity((double)YValues) || double.IsPositiveInfinity((double)YValues))
                            {
                                PointsOnChart.Add(new Point(0, double.NaN));
                            }
                            else
                            {
                                double YValue = ((double)YValues - sender.YMin) / (sender.YMax - sender.YMin) * sender.PlotHeight;
                                PointsOnChart.Add(new Point(0, YValue));
                            }
                        }
                    }
                    X++;
                }
                ListOfChartCurves.Add(PointsOnChart);
            }

            ObservableCollection<FrameworkElement> items = new ObservableCollection<FrameworkElement>();
            bool showPointforHighandLow = false;
            if (ListOfChartCurves.Count > 2) showPointforHighandLow = true;
            for (int k = 0; k < ListOfChartCurves.Count; k++)
            {
                for (int i = 0; i < ListOfChartCurves[k].Count; i++)
                {
                    double pos = (double)i * sender.PlotWidth / (double)ListOfChartCurves[k].Count;
                    ListOfChartCurves[k][i] = new Point(pos, ListOfChartCurves[k][i].Y);

                    if (sender.ShowGraphPoints || (showPointforHighandLow && k > 1 && !double.IsNaN(ListOfChartCurves[k][i].Y)))
                    {
                        Ellipse CurvePoint = new Ellipse();
                        CurvePoint.Width = 10;
                        CurvePoint.Height = 10;
                        CurvePoint.Fill = DistinctColorList[k];
                        CurvePoint.ToolTip = origianlValues[i].ToString();
                        Canvas.SetLeft(CurvePoint, ListOfChartCurves[k][i].X - 5);
                        Canvas.SetTop(CurvePoint, ListOfChartCurves[k][i].Y - 5);
                        items.Add(CurvePoint);
                    }
                }
            }



            for (int k = 0; k < ListOfChartCurves.Count; k++)
            {
                if (!sender.CurveVisibility[k].IsChecked)
                {
                    PointCollection PointsOnChart = ListOfChartCurves[k];
                    List<PointCollection> MyLines = new List<PointCollection>();
                    PointCollection CurrentCollection = new PointCollection();

                    // Create lines even if points are disjoint/missing
                    for (int i = 0; i < PointsOnChart.Count; i++)
                    {
                        // Current point is mission
                        if (double.IsNaN(PointsOnChart[i].Y))
                        {
                            // Any values that should be stored in previous points?
                            if (CurrentCollection.Count != 0)
                                MyLines.Add(CurrentCollection.Clone());

                            // Create a new line to store any new valid points found
                            CurrentCollection = new PointCollection();
                        }
                        else
                        {
                            // It's a valid point, add it to the current pointcollection
                            CurrentCollection.Add(PointsOnChart[i]);
                        }
                    }

                    // Add the last pontcollection, if it has any points in it.
                    if (CurrentCollection.Count != 0)
                        MyLines.Add(CurrentCollection.Clone());

                    //Draw all the lines found in the curve
                    foreach (PointCollection item in MyLines)
                    {
                        Polyline Curve = new Polyline();
                        if (sender.SmoothCurve)
                            Curve.Points = CatmullRomSpline(item, 0.01);
                        else
                            Curve.Points = item;

                        Curve.StrokeThickness = 2;
                        Curve.Stroke = DistinctColorList[k];
                        items.Add(Curve);
                    }
                }
            }
            sender.PlotArea.ItemsSource = items;
        }

        private static void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e, IEnumerable eNewValue)
        {
            var MyClass = (LineChartBase)sender;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                MyClass.CurveVisibility.Add(new CheckBoxClass()
                {
                    BackColor = DistinctColorList[MyClass.CurveVisibility.Count],
                    IsChecked = false,
                    //Name = "Curve nr: " + (MyClass.CurveVisibility.Count + 1).ToString()
                    Name = (e.NewItems[0] as LineSeries).Name
                });

                ((INotifyPropertyChanged)MyClass.CurveVisibility[MyClass.CurveVisibility.Count - 1]).PropertyChanged -= (s, ee) => OnCurveVisibilityChanged(MyClass, eNewValue);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ((INotifyPropertyChanged)MyClass.CurveVisibility[e.OldStartingIndex]).PropertyChanged -= (s, ee) => OnCurveVisibilityChanged(MyClass, eNewValue);
                MyClass.CurveVisibility.RemoveAt(e.OldStartingIndex);
            }
            else if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                MyClass.CurveVisibility.Clear();
            }

            if (MyClass.DisplayMemberValues != "" && MyClass.DisplayMemberLabels != "" && MyClass.DataCollectionName != "")
            {
                SetupYAxis(MyClass);
                SetupXAxis(MyClass);
                SetupGraph(MyClass, eNewValue);
            }
        }

        private static void OnCurveVisibilityChanged(LineChartBase sender, IEnumerable NewValues)
        {
            SetupGraph(sender, NewValues);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var MyChart = (LineChartBase)d;

            foreach (var item in MyChart.ItemsSource)
            {
                int i = MyChart.CurveVisibility.Count;
                MyChart.CurveVisibility.Add(new CheckBoxClass() { BackColor = DistinctColorList[i], Name = "Curve nr: " + (i + 1).ToString() });

                ((INotifyPropertyChanged)MyChart.CurveVisibility[MyChart.CurveVisibility.Count - 1]).PropertyChanged += (s, ee) => OnCurveVisibilityChanged(MyChart, (IEnumerable)e.NewValue);
            }

            if (e.NewValue != null)
            {
                if (e.NewValue is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)e.NewValue).CollectionChanged += (s, ee) => ItemsSource_CollectionChanged(MyChart, ee, (IEnumerable)e.NewValue);
                }
            }

            if (e.OldValue != null)
            {
                if (e.OldValue is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)e.NewValue).CollectionChanged -= (s, ee) => ItemsSource_CollectionChanged(MyChart, ee, (IEnumerable)e.OldValue);
                }
            }

            if (MyChart.DisplayMemberValues != "" && MyChart.DisplayMemberLabels != "" && MyChart.DataCollectionName != "")
            {
                SetupYAxis(MyChart);
                SetupXAxis(MyChart);
                SetupGraph(MyChart, (IEnumerable)e.NewValue);
            }
            else
            {
                MessageBox.Show("No data provided");
            }
        }

        private void PlotAreaBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.PlotWidth = PlotAreaBorder.ActualWidth - 40 - 40;
            this.PlotHeight = PlotAreaBorder.ActualHeight - 40 - 40;
            PlotArea.Height = this.PlotHeight;
            PlotArea.Width = this.PlotWidth;
            XAxisLine.Points[1] = new Point(PlotWidth, 0);
            YAxisLine.Points[1] = new Point(0, PlotHeight);

            Curves.Width = PlotAreaBorder.Width;
            if (this.DisplayMemberValues != "" && this.DisplayMemberLabels != "" && this.DataCollectionName != "")
            {
                SetupXAxis(this);
                SetupYAxis(this);
                SetupGraph(this, this.ItemsSource);
            }
        }

        #region DependencyProperties


        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LineChartBase),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnItemsSourceChanged)));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public bool AutoScale
        {
            get { return (bool)GetValue(AutoScaleProperty); }
            set { SetValue(AutoScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScaleProperty =
            DependencyProperty.Register("AutoScale", typeof(bool),
                typeof(LineChartBase), new PropertyMetadata(true));

        public string ChatTitle
        {
            get { return (string)GetValue(ChatTitleProperty); }
            set { SetValue(ChatTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChatTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChatTitleProperty =
            DependencyProperty.Register("ChatTitle", typeof(string),
                typeof(LineChartBase), new PropertyMetadata(""));

        public string DataCollectionName
        {
            get { return (string)GetValue(DataCollectionNameProperty); }
            set { SetValue(DataCollectionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataCollectionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataCollectionNameProperty =
            DependencyProperty.Register("DataCollectionName", typeof(string),
                typeof(LineChartBase), new PropertyMetadata(""));

        public int SkipLabels
        {
            get { return (int)GetValue(SkipLabelsProperty); }
            set { SetValue(SkipLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkipLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkipLabelsProperty =
            DependencyProperty.Register("SkipLabels", typeof(int),
                typeof(LineChartBase), new PropertyMetadata(0));

        public int StartSkipAt
        {
            get { return (int)GetValue(StartSkipAtProperty); }
            set { SetValue(StartSkipAtProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartSkipAt.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartSkipAtProperty =
            DependencyProperty.Register("StartSkipAt", typeof(int),
                typeof(LineChartBase), new PropertyMetadata(0));

        public bool ShowGraphPoints
        {
            get { return (bool)GetValue(ShowGraphPointsProperty); }
            set { SetValue(ShowGraphPointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGraphPoints.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowGraphPointsProperty =
            DependencyProperty.Register("ShowGraphPoints", typeof(bool),
                typeof(LineChartBase), new PropertyMetadata(false));

        public string DisplayMemberValues
        {
            get { return (string)GetValue(DisplayMemberValuesProperty); }
            set { SetValue(DisplayMemberValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMemberValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMemberValuesProperty =
            DependencyProperty.Register("DisplayMemberValues", typeof(string),
                typeof(LineChartBase), new PropertyMetadata(""));

        public string DisplayMemberLabels
        {
            get { return (string)GetValue(DisplayMemberLabelsProperty); }
            set { SetValue(DisplayMemberLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMemberLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMemberLabelsProperty =
            DependencyProperty.Register("DisplayMemberLabels", typeof(string),
                typeof(LineChartBase), new PropertyMetadata(""));

        public double XMin
        {
            get { return (double)GetValue(XMinProperty); }
            set { SetValue(XMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XMinProperty =
            DependencyProperty.Register("XMin", typeof(double),
                typeof(LineChartBase), new PropertyMetadata(0d));

        public double XMax
        {
            get { return (double)GetValue(XMaxProperty); }
            set { SetValue(XMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XMaxProperty =
            DependencyProperty.Register("XMax", typeof(double),
                typeof(LineChartBase), new PropertyMetadata(0d));

        public double YMin
        {
            get { return (double)GetValue(YMinProperty); }
            set { SetValue(YMinProperty, value); }
        }

        public int NumberOfYSteps
        {
            get { return (int)GetValue(NumberOfYStepsProperty); }
            set { SetValue(NumberOfYStepsProperty, value); }
        }

        private static void YAxisValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetupYAxis((LineChartBase)d);
        }

        public string DoubleToString
        {
            get { return (string)GetValue(DoubleToStringProperty); }
            set { SetValue(DoubleToStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DoubleToString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleToStringProperty =
            DependencyProperty.Register("DoubleToString", typeof(string), typeof(LineChartBase), new PropertyMetadata("N2", new PropertyChangedCallback(YAxisValuesChanged)));



        // Using a DependencyProperty as the backing store for NumberOfYSteps.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumberOfYStepsProperty =
            DependencyProperty.Register("NumberOfYSteps", typeof(int), typeof(LineChartBase), new PropertyMetadata(10, new PropertyChangedCallback(YAxisValuesChanged)));

        // Using a DependencyProperty as the backing store for YMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YMinProperty =
            DependencyProperty.Register("YMin", typeof(double), typeof(LineChartBase), new PropertyMetadata(0d, new PropertyChangedCallback(YAxisValuesChanged)));

        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set { SetValue(YMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YMaxProperty =
            DependencyProperty.Register("YMax", typeof(double), typeof(LineChartBase), new PropertyMetadata(10d, new PropertyChangedCallback(YAxisValuesChanged)));

        public string ChartTitle
        {
            get { return (string)GetValue(ChartTitleProperty); }
            set { SetValue(ChartTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartTitleProperty =
            DependencyProperty.Register("ChartTitle", typeof(string), typeof(LineChartBase), new PropertyMetadata(""));

        public string YLabel
        {
            get { return (string)GetValue(YLabelProperty); }
            set { SetValue(YLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YLable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YLabelProperty =
            DependencyProperty.Register("YLabel", typeof(string),
                typeof(LineChartBase), new PropertyMetadata("Y-Axis"));


        public bool SmoothCurve
        {
            get { return (bool)GetValue(SmoothCurveProperty); }
            set { SetValue(SmoothCurveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmoothCurve.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmoothCurveProperty =
            DependencyProperty.Register("SmoothCurve", typeof(bool),
                typeof(LineChartBase), new PropertyMetadata(false));

        public string XLabel
        {
            get { return (string)GetValue(XLabelProperty); }
            set { SetValue(XLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XLabelProperty =
            DependencyProperty.Register("XLabel", typeof(string),
                typeof(LineChartBase), new PropertyMetadata("X-Axis"));

        public double PlotWidth
        {
            get { return (double)GetValue(PlotWidthProperty); }
            set { SetValue(PlotWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlotWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotWidthProperty =
            DependencyProperty.Register("PlotWidth", typeof(double), typeof(LineChartBase), new PropertyMetadata(400d));

        public double PlotHeight
        {
            get { return (double)GetValue(PlotHeightProperty); }
            set { SetValue(PlotHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlotHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotHeightProperty =
            DependencyProperty.Register("PlotHeight", typeof(double),
                typeof(LineChartBase), new PropertyMetadata(170d));
        #endregion

        #region "CatmullRomSpline"
        public static PointCollection CatmullRomSpline(PointCollection Points, double InterpolationStep = 0.1, bool IsPolygon = false)
        {
            PointCollection result = new PointCollection();

            if (Points.Count <= 2)
            {
                return Points;
            }

            if (IsPolygon)
            {
                for (int i = 0; i <= Points.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        for (double k = 0; k <= (1 - InterpolationStep); k += InterpolationStep)
                        {
                            result.Add(PointOnCatmullRomCurve(Points[Points.Count - 1], Points[i], Points[i + 1], Points[i + 2], k));
                        }
                    }
                    else if (i == Points.Count - 1)
                    {
                        for (double k = 0; k <= (1 - InterpolationStep); k += InterpolationStep)
                        {
                            result.Add(PointOnCatmullRomCurve(Points[i - 1], Points[i], Points[0], Points[1], k));
                        }
                    }
                    else if (i == Points.Count - 2)
                    {
                        for (double k = 0; k <= (1 - InterpolationStep); k += InterpolationStep)
                        {
                            result.Add(PointOnCatmullRomCurve(Points[i - 1], Points[i], Points[i + 1], Points[0], k));
                        }
                    }
                    else
                    {
                        for (double k = 0; k <= (1 - InterpolationStep); k += InterpolationStep)
                        {
                            result.Add(PointOnCatmullRomCurve(Points[i - 1], Points[i], Points[i + 1], Points[i + 2], k));
                        }
                    }
                }
            }
            else
            {
                List<double> yarray = new List<double>();
                List<double> xarray = new List<double>();
                xarray.Add(Points[0].X - (Points[1].X - Points[0].X) / 2);
                yarray.Add(Points[0].Y - (Points[1].Y - Points[0].Y) / 2);

                foreach (System.Windows.Point ps in Points)
                {
                    xarray.Add(ps.X);
                    yarray.Add(ps.Y);
                }

                xarray.Add((Points[Points.Count - 1].X - (Points[Points.Count - 2].X) / 2 + Points[Points.Count - 1].X));
                yarray.Add((Points[Points.Count - 1].Y - (Points[Points.Count - 2].Y) / 2 + Points[Points.Count - 1].Y));

                PointCollection r = new PointCollection();
                for (int i = 0; i <= yarray.Count - 1; i++)
                {
                    r.Add(new System.Windows.Point(xarray[i], yarray[i]));
                }

                for (int i = 3; i <= r.Count - 1; i++)
                {
                    for (double k = 0; k <= (1 - InterpolationStep); k += InterpolationStep)
                    {
                        result.Add(PointOnCatmullRomCurve(r[i - 3], r[i - 2], r[i - 1], r[i], k));
                    }
                }
                result.Add(Points[Points.Count - 1]);
            }

            return result;
        }

        /// <summary>
        /// Calculates interpolated point between two points using Catmull-Rom Spline </summary>
        /// <remarks>
        /// Points calculated exist on the spline between points two and three. </remarks>
        /// <param name="p0">First Point</param>
        /// <param name="p1">Second Point</param>
        /// <param name="p2">Third Point</param>
        /// <param name="p3">Fourth Point</param>
        /// <param name="t">
        /// Normalised distance between second and third point where the spline point will be calculated </param>
        /// <returns>Calculated Spline Point </returns>
        public static System.Windows.Point PointOnCatmullRomCurve(System.Windows.Point p0, System.Windows.Point p1, System.Windows.Point p2, System.Windows.Point p3, double t)
        {
            //Derivative calcualtions
            double lix1 = 0;
            double liy1 = 0;
            double lix2 = 0;
            double liy2 = 0;
            lix1 = 0.5 * (p2.X - p0.X);
            lix2 = 0.5 * (p3.X - p1.X);

            liy1 = 0.5 * (p2.Y - p0.Y);
            liy2 = 0.5 * (p3.Y - p1.Y);

            // Location of Controlpoints
            PointCollection PointList = new PointCollection();
            PointList.Add(p1);
            PointList.Add(new Point(p1.X + (1d / 3d) * lix1, p1.Y + (1d / 3d) * liy1));
            PointList.Add(new Point(p2.X - (1d / 3d) * lix2, p2.Y - (1d / 3d) * liy2));
            PointList.Add(p2);

            // Get the calcualted value from the 3rd degree Bezier curve
            return PointBezierFunction(PointList, t);
        }
        private static Point PointBezierFunction(PointCollection p, double StepSize)
        {
            PointCollection result = new PointCollection();
            double[] B = null;
            double CX = 0;
            double CY = 0;
            double k = StepSize;

            B = AllBernstein(p.Count, k);

            CX = 0;
            CY = 0;
            for (int j = 0; j <= p.Count - 1; j++)
            {
                CX = CX + B[j] * p[j].X;
                CY = CY + B[j] * p[j].Y;
            }
            return new Point(CX, CY);

        }

        /// <summary>
        /// The code uses the recursive relation B_[i,n](u) = (1-u)*B_[i,n-1](u) + u*B_[i-1,n-1](u) to compute all nth-degree Bernstein polynomials
        /// </summary>
        /// <param name="n">The sum of the start point, the end point and all the knot points between. Valid range is from 2 and upwards.</param>
        /// <param name="u">Ranges from 0 to 1, and represents the current position of the curve</param>
        /// <returns></returns>
        /// <remarks>This code is translated to VB from the original C++  code given on page 21 in "The NURBS Book" by Les Piegl and Wayne Tiller </remarks>
        private static double[] AllBernstein(int n, double u)
        {
            double[] B = new double[n];
            B[0] = 1;
            double u1 = 0;
            u1 = 1 - u;
            double saved = 0;
            double temp = 0;

            for (int j = 1; j <= n - 1; j++)
            {
                saved = 0;
                for (int k = 0; k <= j - 1; k++)
                {
                    temp = B[k];
                    B[k] = saved + u1 * temp;
                    saved = u * temp;
                }
                B[j] = saved;
            }

            return B;
        }
        #endregion

        #region "Color generator"
        /// <summary>
        /// http://stackoverflow.com/questions/309149/generate-distinctly-different-rgb-colors-in-graphs/309193#309193
        /// </summary>
        public class ColourGenerator
        {

            private int index = 0;
            private IntensityGenerator intensityGenerator = new IntensityGenerator();

            public string NextColour()
            {
                string colour = string.Format(PatternGenerator.NextPattern(index),
                    intensityGenerator.NextIntensity(index));
                index++;
                return colour;
            }
        }

        public class PatternGenerator
        {
            public static string NextPattern(int index)
            {
                switch (index % 7)
                {
                    case 0: return "{0}0000";
                    case 1: return "00{0}00";
                    case 2: return "0000{0}";
                    case 3: return "{0}{0}00";
                    case 4: return "{0}00{0}";
                    case 5: return "00{0}{0}";
                    case 6: return "{0}{0}{0}";
                    default: throw new Exception("Math error");
                }
            }
        }

        public class IntensityGenerator
        {
            private IntensityValueWalker walker;
            private int current;

            public string NextIntensity(int index)
            {
                if (index == 0)
                {
                    current = 255;
                }
                else if (index % 7 == 0)
                {
                    if (walker == null)
                    {
                        walker = new IntensityValueWalker();
                    }
                    else
                    {
                        walker.MoveNext();
                    }
                    current = walker.Current.Value;
                }
                string currentText = current.ToString("X");
                if (currentText.Length == 1) currentText = "0" + currentText;
                return currentText;
            }
        }

        public class IntensityValue
        {

            private IntensityValue mChildA;
            private IntensityValue mChildB;

            public IntensityValue(IntensityValue parent, int value, int level)
            {
                if (level > 7) throw new Exception("There are no more colours left");
                Value = value;
                Parent = parent;
                Level = level;
            }

            public int Level { get; set; }
            public int Value { get; set; }
            public IntensityValue Parent { get; set; }

            public IntensityValue ChildA
            {
                get
                {
                    return mChildA ?? (mChildA = new IntensityValue(this, this.Value - (1 << (7 - Level)), Level + 1));
                }
            }

            public IntensityValue ChildB
            {
                get
                {
                    return mChildB ?? (mChildB = new IntensityValue(this, Value + (1 << (7 - Level)), Level + 1));
                }
            }
        }

        public class IntensityValueWalker
        {

            public IntensityValueWalker()
            {
                Current = new IntensityValue(null, 1 << 7, 1);
            }

            public IntensityValue Current { get; set; }

            public void MoveNext()
            {
                if (Current.Parent == null)
                {
                    Current = Current.ChildA;
                }
                else if (Current.Parent.ChildA == Current)
                {
                    Current = Current.Parent.ChildB;
                }
                else
                {
                    int levelsUp = 1;
                    Current = Current.Parent;
                    while (Current.Parent != null && Current == Current.Parent.ChildB)
                    {
                        Current = Current.Parent;
                        levelsUp++;
                    }
                    if (Current.Parent != null)
                    {
                        Current = Current.Parent.ChildB;
                    }
                    else
                    {
                        levelsUp++;
                    }
                    for (int i = 0; i < levelsUp; i++)
                    {
                        Current = Current.ChildA;
                    }

                }
            }
        }


        #endregion
    }
}
