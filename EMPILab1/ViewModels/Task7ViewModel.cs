﻿using System.Collections.Generic;
using System.Linq;
using EMPILab1.Helpers;
using EMPILab1.Models;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Task7ViewModel : BaseViewModel
    {
        private readonly double _errorForAnomalies = 0.01;
        private double _lowBorder;
        private double _upBorder;

        public Task7ViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        private PlotModel _scatterPlotModel;
        public PlotModel ScatterPlotModel
        {
            get => _scatterPlotModel;
            set => SetProperty(ref _scatterPlotModel, value);
        }

        private List<double> _initialDataset = new();
        public List<double> InitialDataset
        {
            get => _initialDataset;
            set => SetProperty(ref _initialDataset, value);
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

            var anomalies = CalculateAnomalies(InitialDataset, _errorForAnomalies);

            var normalPoints = new List<ScatterPointViewModel>();
            for (int i = 0; i < InitialDataset.Count(); i++)
            {
                if (!anomalies.Any(a => a.Value == InitialDataset[i]))
                {
                    normalPoints.Add(new ScatterPointViewModel
                    {
                        Index = i,
                        Value = InitialDataset[i],
                    });
                }
            }

            if (anomalies.Any())
            {
                ScatterPlotModel = CalculateScatterModel(normalPoints, anomalies);
            }
            else
            {
                ScatterPlotModel = CalculateScatterModel(normalPoints);
            }
        }

        #endregion

        #region -- Private helpers --

        private List<ScatterPointViewModel> CalculateAnomalies(List<double> initialDataset, double error)
        {
            var quantil = MathHelpers.QuantileU(1 - (error / 2.0));

            var avgValue = InitialDataset.Average();

            _lowBorder = avgValue - quantil * MathHelpers.StandardDeviation(initialDataset);
            _upBorder = avgValue + quantil * MathHelpers.StandardDeviation(initialDataset);

            var anomalies = new List<ScatterPointViewModel>();

            for (int i = 0; i < initialDataset.Count(); i++)
            {
                if (initialDataset[i] > _upBorder
                    || initialDataset[i] < _lowBorder)
                {
                    anomalies.Add(new ScatterPointViewModel
                    {
                        Index = i,
                        Value = initialDataset[i]
                    });
                }
            }

            return anomalies;
        }

        private PlotModel CalculateScatterModel(List<ScatterPointViewModel> normalPoints, List<ScatterPointViewModel> anomalies = null)
        {
            PlotModel scatterModel = new();

            var xAxis = new LinearAxis
            {
                Title = "index",
                Position = AxisPosition.Bottom,
                LabelFormatter = (param) => param.ToString(),
            };
            scatterModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = "value",
                Position = AxisPosition.Left,
            };
            scatterModel.Axes.Add(yAxis);

            ScatterSeries normalScatterSeries = new()
            {
                MarkerFill = OxyColor.Parse("#0000FF"),
            };

            foreach(var point in normalPoints)
            {
                normalScatterSeries.Points.Add(new ScatterPoint(point.Index, point.Value, size: 2));
            }

            scatterModel.Series.Add(normalScatterSeries);

            if (anomalies != null)
            {
                ScatterSeries anomaliesScatterSeries = new()
                {
                    MarkerFill = OxyColor.Parse("#FF0000"),
                };

                foreach (var point in anomalies)
                {
                    anomaliesScatterSeries.Points.Add(new ScatterPoint(point.Index, point.Value, size: 2));
                }

                scatterModel.Series.Add(anomaliesScatterSeries);
            }

            LineAnnotation LineUpper = new LineAnnotation()
            {
                Type = LineAnnotationType.Horizontal,
                StrokeThickness = 1,
                Color = OxyColors.Green,
                Y = _upBorder
            };

            LineAnnotation LineLow = new LineAnnotation()
            {
                Type = LineAnnotationType.Horizontal,
                StrokeThickness = 1,
                Color = OxyColors.Green,
                Y = _lowBorder
            };

            scatterModel.Annotations.Add(LineUpper);
            scatterModel.Annotations.Add(LineLow);

            return scatterModel;
        }

        #endregion
    }
}
