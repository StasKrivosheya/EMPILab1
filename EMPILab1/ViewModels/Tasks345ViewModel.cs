using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EMPILab1.Helpers;
using EMPILab1.Models;
using EMPILab1.Pages;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Tasks345ViewModel : BaseViewModel
    {
        public Tasks345ViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        #region -- Public properties --

        private string _classesAmount;
        public string ClassesAmount
        {
            get => _classesAmount;
            set => SetProperty(ref _classesAmount, value);
        }

        private double _classWidth;
        public double ClassWidth
        {
            get => _classWidth;
            set => SetProperty(ref _classWidth, value);
        }

        private ObservableCollection<VariantItemViewModel> _variants;
        public ObservableCollection<VariantItemViewModel> Variants
        {
            get => _variants;
            set => SetProperty(ref _variants, value);
        }

        private ObservableCollection<ClassViewModel> _classes;
        public ObservableCollection<ClassViewModel> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        private PlotModel _histogramModel;
        public PlotModel HistogramModel
        {
            get => _histogramModel;
            set => SetProperty(ref _histogramModel, value);
        }

        private List<double> _initialDataset;
        public List<double> InitialDataset
        {
            get => _initialDataset;
            set => SetProperty(ref _initialDataset, value);
        }

        private string _bandwidth = string.Empty;
        public string Bandwidth
        {
            get => _bandwidth;
            set => SetProperty(ref _bandwidth, value);
        }

        private ICommand _recalculateCommand;
        public ICommand RecalculateCommand => _recalculateCommand ??= new DelegateCommand(OnRecalculateCommandAsync);

        private ICommand _continueCommand;
        public ICommand ContinueCommand => _continueCommand ??= new DelegateCommand(async () => await OnContinueCommandAsync());

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(Variants), out IEnumerable<VariantItemViewModel> variants))
            {
                Variants = new(variants);
            }

            if (parameters.TryGetValue(nameof(InitialDataset), out IEnumerable<double> initailDataset))
            {
                InitialDataset = new(initailDataset);
            }

            var optimalClassCount = GetOptimalClassCount();
            var classes = SplitOnClasses(optimalClassCount);

            ClassesAmount = optimalClassCount.ToString();
            Classes = new(classes);
            HistogramModel = GetClassesChartModel();
        }

        #endregion

        #region -- Private helpers --

        private Task OnContinueCommandAsync()
        {
            var prms = new NavigationParameters
            {
                { nameof(List<double>), InitialDataset },
                { nameof(Variants), Variants },
            };

            return NavigationService.NavigateAsync(nameof(Task6), prms);
        }

        private void OnRecalculateCommandAsync()
        {
            if (int.TryParse(ClassesAmount, out var number))
            {
                var classes = SplitOnClasses(number);

                Classes = new(classes);
                HistogramModel = GetClassesChartModel();
            }
        }

        private IEnumerable<ClassViewModel> SplitOnClasses(int classCount)
        {
            var minVal = Variants.AsQueryable().Min(v => v.Value);
            var maxVal = Variants.AsQueryable().Max(v => v.Value);
            var h = ClassWidth = Math.Round((maxVal - minVal) / classCount, 4, MidpointRounding.AwayFromZero);

            var classes = new List<ClassViewModel>();

            var empiricalDistrFuncValue = 0d;
            for (int i = 1; i <= classCount; i++)
            {
                var leftBound = minVal;
                var rightBound = minVal + h;

                classes.Add(new ClassViewModel
                {
                    Index = i,
                    ClassName = $"p{i}",
                    ClassWidth = h,
                    Bounds = new Tuple<double, double>(leftBound, rightBound),
                });

                var includedVariants = Variants.Where(v => v.Value >= leftBound && v.Value < rightBound).ToList();
                if (i == classCount && !includedVariants.Contains(Variants.LastOrDefault()))
                {
                    includedVariants.Add(Variants.LastOrDefault());
                }

                classes[i - 1].Frequency = includedVariants.Count;
                classes[i - 1].RelativeFrequency = classes[i - 1].Frequency / Variants.Count;
                classes[i - 1].EmpiricalDistrFuncValue = empiricalDistrFuncValue += classes[i - 1].RelativeFrequency;

                minVal += h;
            }

            return classes;
        }

        private int GetOptimalClassCount()
        {
            var m = Math.Round(1 + 3.32 * Math.Log10(Variants.Count), 0);
            var nearestOdd = (int)(2 * Math.Floor(m / 2) + 1);

            return nearestOdd;
        }

        private PlotModel GetClassesChartModel()
        {
            var plotModel = new PlotModel();

            var xAxis = new LinearAxis
            {
                Title = "X (classes' range)",
                Position = AxisPosition.Bottom,
                LabelFormatter = (param) => Math.Round(param, 3).ToString()
            };

            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = "P (relative frequency)",
                Position = AxisPosition.Left
            };

            plotModel.Axes.Add(yAxis);

            var barSeries = new RectangleBarSeries();

            foreach (var item in Classes)
            {
                var bar = new RectangleBarItem(item.Bounds.Item1, 0, item.Bounds.Item2, item.RelativeFrequency);

                if (Classes.Count < 25)
                {
                    bar.Title = item.ClassName;
                }

                barSeries.Items.Add(bar);
            }

            plotModel.Series.Add(barSeries);

            var kdeLineSeries = GetKDELineSeries();
            plotModel.Series.Add(kdeLineSeries);

            return plotModel;
        }

        private LineSeries GetKDELineSeries()
        {
            var result = new LineSeries();

            if (string.IsNullOrEmpty(Bandwidth) || !double.TryParse(Bandwidth, out var _))
            {
                var bandwidth = MathHelpers.GetScottBandwidth(Variants.ToList());

                Bandwidth = bandwidth.ToString();
            }
            var sortedDataset = InitialDataset.OrderBy(u => u).ToList();
            var points = MathHelpers.GetGaussianKdePoints(sortedDataset, double.Parse(Bandwidth));

            foreach (var point in points)
            {
                result.Points.Add(new DataPoint(point.Item1, point.Item2 * ClassWidth));
            }

            return result;
        }

        #endregion
    }
}
