using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EMPILab1.Helpers;
using EMPILab1.Models;
using EMPILab1.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Task8SubtasksViewModel : BaseViewModel
    {
        private const double ALPHA = 0.05;

        private readonly IRuntimeStorage _runtimeStorage;

        public Task8SubtasksViewModel(
            INavigationService navigationService,
            IRuntimeStorage runtimeStorage)
            : base(navigationService)
        {
            _runtimeStorage = runtimeStorage;
        }

        #region -- Public properties --

        private PlotModel _newHistogramModel;
        public PlotModel NewHistogramModel
        {
            get => _newHistogramModel;
            set => SetProperty(ref _newHistogramModel, value);
        }

        private List<double> _initialDataset = new();
        public List<double> InitialDataset
        {
            get => _initialDataset;
            set => SetProperty(ref _initialDataset, value);
        }

        private ObservableCollection<ParameterEstimationViewModel> _parametersEstimations;
        public ObservableCollection<ParameterEstimationViewModel> ParametersEstimations
        {
            get => _parametersEstimations;
            set => SetProperty(ref _parametersEstimations, value);
        }

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(List<double>), out List<double> dataset))
            {
                InitialDataset = new List<double>(dataset);
            }

            EstimateParameters(ALPHA);

            NewHistogramModel = CalculateModelForRenewedDensityFunction(_runtimeStorage.HistogramModel);
        }

        #endregion

        #region -- Private helpers --

        private void EstimateParameters(double alpha)
        {
            // по условию варианта 11, закон распределения - экспоненциальный
            // рассчеты см в ../FormulasAndReferences/Calculations.jpeg

            // согласно рассчетам, имеем точечную оценку лямбда и ее сред.кв.отклон:
            var lambda_ = 1 / InitialDataset.Average();
            var stdDevLambda_ = Math.Pow(lambda_, 2) / InitialDataset.Count;

            // 1.96
            var u = MathHelpers.QuantileU(1 - (alpha / 2));
            var lambdaLow = lambda_ - u * Math.Sqrt(stdDevLambda_);
            var lambdaHigh = lambda_ + u * Math.Sqrt(stdDevLambda_);

            var parameter = new ParameterEstimationViewModel
            {
                Name = "λ",
                Value = 1 / InitialDataset.Average(),
                StandardDeviation = stdDevLambda_,
                ConfidenceInterval = new Tuple<double, double>(lambdaLow, lambdaHigh),
            };

            ParametersEstimations = new ObservableCollection<ParameterEstimationViewModel>
            {
                { parameter },
            };
        }

        private PlotModel CalculateModelForRenewedDensityFunction(PlotModel histogramModel)
        {
            var plotModel = new PlotModel
            {
                Title = "Restored density function"
            };

            var xAxis = new LinearAxis
            {
                Title = "classRange",
                Position = AxisPosition.Bottom,
                LabelFormatter = (param) => Math.Round(param, 3).ToString()
            };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = "RelativeFreq",
                Position = AxisPosition.Left
            };
            plotModel.Axes.Add(yAxis);

            foreach (var item in histogramModel.Series)
            {
                if (item is RectangleBarSeries series)
                {
                    var seriesCopy = new RectangleBarSeries();

                    foreach (var subitem in series.Items)
                    {
                        seriesCopy.Items.Add(new RectangleBarItem(subitem.X0, subitem.Y0, subitem.X1, subitem.Y1));
                    }

                    plotModel.Series.Add(seriesCopy);
                }
                else
                {
                    var densitySeries = item as LineSeries;
                    var seriesCopy = new LineSeries();
                    foreach (var subitem in densitySeries.Points)
                    {
                        seriesCopy.Points.Add(new DataPoint(subitem.X, subitem.Y));
                    }
                    plotModel.Series.Add(seriesCopy);
                }

            }

            var lineSeries = new LineSeries
            {
                Title = "RestoredDensityFunction",
                Color = OxyColor.Parse("#FF0000")
            };

            var xs = GetXs(1000);

            foreach (var x in xs)
            {
                lineSeries.Points.Add(
                    new DataPoint(x,
                        MathHelpers.DensityFuncForExponential(x, ParametersEstimations.FirstOrDefault().Value) * _runtimeStorage.ClassWidth));
            }

            plotModel.Series.Add(lineSeries);

            return plotModel;
        }

        private List<double> GetXs(int count)
        {
            var min = InitialDataset.Min();
            var max = InitialDataset.Max();

            double step = (max - min) / count;

            var result = new List<double>();

            for (var i = min; i <= max; i += step)
            {
                result.Add(i);
            }

            return result;
        }

        #endregion
    }
}
