using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EMPILab1.Extensions;
using EMPILab1.Pages;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Task8ViewModel : BaseViewModel
    {
        public Task8ViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        private List<double> _initialDataset = new();
        public List<double> InitialDataset
        {
            get => _initialDataset;
            set => SetProperty(ref _initialDataset, value);
        }

        private ObservableCollection<Tuple<double, double>> _newCoordinates;
        public ObservableCollection<Tuple<double, double>> NewCoordinates
        {
            get => _newCoordinates;
            set => SetProperty(ref _newCoordinates, value);
        }

        private PlotModel _propbabilityPapperModel;
        public PlotModel PropbabilityPaperModel
        {
            get => _propbabilityPapperModel;
            set => SetProperty(ref _propbabilityPapperModel, value);
        }

        private ICommand _continueCommand;
        public ICommand ContinueCommand => _continueCommand ??= new DelegateCommand(async () => await OnContinueCommandAsync());

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(List<double>), out List<double> dataset))
            {
                InitialDataset = new List<double>(dataset);
            }

            CalculateNewCoordinates();

            PropbabilityPaperModel = GetProbabilityPaperModel();
        }

        #endregion

        #region -- Private helpers --

        private void CalculateNewCoordinates()
        {
            NewCoordinates = new();

            var variants = InitialDataset.ToVariantsList();

            foreach (var x in InitialDataset)
            {
                var t = x;
                var fx = variants.Find(v => v.Value == x).EmpiricalDistrFuncValue;
                var diff = 1 - fx;

                if (diff > 0)
                {
                    var z = -Math.Log(diff);

                    NewCoordinates.Add(new Tuple<double, double>(t, z));
                }
            }
        }

        private PlotModel GetProbabilityPaperModel()
        {
            var plotModel = new PlotModel
            {
                Title = "Вероятностная бумага (экспоненц. распр.)"
            };

            var xAxis = new LinearAxis
            {
                Title = "t",
                Position = AxisPosition.Bottom,
                LabelFormatter = (param) => Math.Round(param, 3).ToString(),
                Minimum = NewCoordinates.Select(u => u.Item1).Min(),
                Maximum = NewCoordinates.Select(u => u.Item1).Max(),
            };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = "z",
                Position = AxisPosition.Left,
                Minimum = NewCoordinates.Select(u => u.Item2).Min(),
                Maximum = NewCoordinates.Select(u => u.Item2).Max(),
            };
            plotModel.Axes.Add(yAxis);

            var scatterSeries = new ScatterSeries();

            foreach (var point in NewCoordinates)
            {
                scatterSeries.Points.Add(new ScatterPoint(point.Item1, point.Item2, 1));
            }

            plotModel.Series.Add(scatterSeries);

            return plotModel;
        }

        private Task OnContinueCommandAsync()
        {
            var prms = new NavigationParameters
            {
                { nameof(List<double>), InitialDataset },
            };

            return NavigationService.NavigateAsync(nameof(Task8Subtasks), prms);
        }

        #endregion
    }
}
