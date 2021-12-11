using System;
using Prism.Mvvm;

namespace EMPILab1.Models
{
    public class ParameterEstimationViewModel : BindableBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private double _value;
        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private double _standardDeviation;
        public double StandardDeviation
        {
            get => _standardDeviation;
            set => SetProperty(ref _standardDeviation, value);
        }

        private Tuple<double, double> _confidenceInterval;
        public Tuple<double, double> ConfidenceInterval
        {
            get => _confidenceInterval;
            set => SetProperty(ref _confidenceInterval, value);
        }
    }
}
