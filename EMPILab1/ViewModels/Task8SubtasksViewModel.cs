using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EMPILab1.Extensions;
using EMPILab1.Helpers;
using EMPILab1.Models;
using EMPILab1.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Task8SubtasksViewModel : BaseViewModel
    {
        private const double ALPHA = 0.05;

        private double _lambdaPointEstimate;

        private readonly IRuntimeStorage _runtimeStorage;

        public Task8SubtasksViewModel(
            INavigationService navigationService,
            IRuntimeStorage runtimeStorage)
            : base(navigationService)
        {
            _runtimeStorage = runtimeStorage;
        }

        #region -- Public properties --

        private PlotModel _densityModel;
        public PlotModel DensityModel
        {
            get => _densityModel;
            set => SetProperty(ref _densityModel, value);
        }

        private bool _isDensityModelVisible = true;
        public bool IsDensityModelVisible
        {
            get => _isDensityModelVisible;
            set => SetProperty(ref _isDensityModelVisible, value);
        }

        private PlotModel _distributionModel;
        public PlotModel DistributionModel
        {
            get => _distributionModel;
            set => SetProperty(ref _distributionModel, value);
        }

        private bool _isDistributionModelVisible;
        public bool IsDistributionModelVisible
        {
            get => _isDistributionModelVisible;
            set => SetProperty(ref _isDistributionModelVisible, value);
        }

        private PlotModel _linearModel;
        public PlotModel LinearModel
        {
            get => _linearModel;
            set => SetProperty(ref _linearModel, value);
        }

        private bool _isLinearModelVisible;
        public bool IsLinearModelVisible
        {
            get => _isLinearModelVisible;
            set => SetProperty(ref _isLinearModelVisible, value);
        }

        //

        private bool _isReliabilityVisible;
        public bool IsReliabilityVisible
        {
            get => _isReliabilityVisible;
            set => SetProperty(ref _isReliabilityVisible, value);
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

        private ICommand _ShowDensityCommand;
        public ICommand ShowDensityCommand => _ShowDensityCommand ??= new DelegateCommand(OnShowDensityCommand);

        private ICommand _ShowDistributionCommand;
        public ICommand ShowDistributionCommand => _ShowDistributionCommand ??= new DelegateCommand(OnShowDistributionCommand);

        private ICommand _ShowLinearCommand;
        public ICommand ShowLinearCommand => _ShowLinearCommand ??= new DelegateCommand(OnShowLinearCommand);

        private ICommand _ShowReliabilityCommand;
        public ICommand ShowReliabilityCommand => _ShowReliabilityCommand ??= new DelegateCommand(OnShowReliabilityCommand);

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

            DensityModel = GetModelForRenewedDensityFunction(_runtimeStorage.HistogramModel);

            DistributionModel = GetModelForRenewedDistributionFunction();

            LinearModel = GetProbabilityPaperModel();
            AddLinearDistrFuncToProbabilityPaperModel();
        }

        #endregion

        #region -- Private helpers --

        private void EstimateParameters(double alpha)
        {
            // по условию варианта 11, закон распределения - экспоненциальный
            // рассчеты см в ../FormulasAndReferences/Calculations.jpeg

            // согласно рассчетам, имеем точечную оценку лямбда и ее сред.кв.отклон:
            var lambda_ = _lambdaPointEstimate = 1 / InitialDataset.Average();
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

        private PlotModel GetModelForRenewedDensityFunction(PlotModel histogramModel)
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

            var xs = MathHelpers.GetXs(InitialDataset, 1000);

            foreach (var x in xs)
            {
                lineSeries.Points.Add(
                    new DataPoint(x, MathHelpers.DensityFuncForExponential(x, ParametersEstimations.FirstOrDefault().Value) * _runtimeStorage.ClassWidth));
            }

            plotModel.Series.Add(lineSeries);

            return plotModel;
        }

        private PlotModel GetModelForRenewedDistributionFunction()
        {
            var plotModel = new PlotModel
            {
                Title = "Restored distribution function"
            };

            var xAxis = new LinearAxis
            {
                Title = "value",
                Position = AxisPosition.Bottom,
                LabelFormatter = (param) => Math.Round(param, 3).ToString()
            };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = "EmpFunctionResult",
                Position = AxisPosition.Left
            };
            plotModel.Axes.Add(yAxis);

            var variants = InitialDataset.ToVariantsList();
            var stepSeries = new StairStepSeries();
            foreach (var v in variants)
            {
                stepSeries.Points.Add(new DataPoint(v.Value, v.EmpiricalDistrFuncValue));
            }

            var xs = MathHelpers.GetXs(InitialDataset, 1000);
            var distributionFunc = new LineSeries();
            foreach (var x in xs)
            {
                distributionFunc.Points.Add(new DataPoint(x, MathHelpers.ImplericalFuncExponential(x, ParametersEstimations.FirstOrDefault().Value)));
            }

            plotModel.Series.Add(stepSeries);
            plotModel.Series.Add(distributionFunc);

            return plotModel;
        }

        private void AddLinearDistrFuncToProbabilityPaperModel()
        {
            var linearSeries = new LineSeries
            {
                Title = "Restored distribution function"
            };

            var xs = MathHelpers.GetXs(InitialDataset, 1000);

            foreach (var x in xs)
            {
                linearSeries.Points
                    .Add(new DataPoint(x,
                        Math.Log(1 / (1 - MathHelpers.ImplericalFuncExponential(x, _lambdaPointEstimate)))));
            }

            LinearModel.Series.Add(linearSeries);
        }

        // hack: copy pase from previous viewmodel
        private List<Tuple<double, double>> GetNewCoordinates()
        {
            var newCoordinates = new List<Tuple<double, double>>();

            var variants = InitialDataset.ToVariantsList();

            foreach (var x in InitialDataset)
            {
                var t = x;
                var fx = variants.Find(v => v.Value == x).EmpiricalDistrFuncValue;
                var diff = 1 - fx;

                if (diff > 0)
                {
                    var z = -Math.Log(diff);

                    newCoordinates.Add(new Tuple<double, double>(t, z));
                }
            }

            return newCoordinates;
        }

        private PlotModel GetProbabilityPaperModel()
        {
            var newCoordinates = GetNewCoordinates();

            var plotModel = new PlotModel
            {
                Title = "Вероятностная бумага (экспоненц. распр.)"
            };

            var xAxis = new LinearAxis
            {
                Title = "t",
                Position = AxisPosition.Bottom,
                LabelFormatter = (param) => Math.Round(param, 3).ToString(),
                Minimum = newCoordinates.Select(u => u.Item1).Min(),
                Maximum = newCoordinates.Select(u => u.Item1).Max(),
            };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = "z",
                Position = AxisPosition.Left,
                Minimum = newCoordinates.Select(u => u.Item2).Min(),
                Maximum = newCoordinates.Select(u => u.Item2).Max(),
            };
            plotModel.Axes.Add(yAxis);

            var scatterSeries = new ScatterSeries();

            foreach (var point in newCoordinates)
            {
                scatterSeries.Points.Add(new ScatterPoint(point.Item1, point.Item2, 1));
            }

            plotModel.Series.Add(scatterSeries);

            return plotModel;
        }

        // hack: quick solution
        private void OnShowDensityCommand()
        {
            IsDensityModelVisible = true;
            IsDistributionModelVisible = false;
            IsLinearModelVisible = false;
            IsReliabilityVisible = false;
        }

        private void OnShowDistributionCommand()
        {
            IsDensityModelVisible = false;
            IsDistributionModelVisible = true;
            IsLinearModelVisible = false;
            IsReliabilityVisible = false;
        }

        private void OnShowLinearCommand()
        {
            IsDensityModelVisible = false;
            IsDistributionModelVisible = false;
            IsLinearModelVisible = true;
            IsReliabilityVisible = false;
        }

        private void OnShowReliabilityCommand()
        {
            IsDensityModelVisible = false;
            IsDistributionModelVisible = false;
            IsLinearModelVisible = false;
            IsReliabilityVisible = true;
        }

        #endregion
    }
}
