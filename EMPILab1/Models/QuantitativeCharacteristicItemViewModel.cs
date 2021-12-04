using System;
using Prism.Mvvm;

namespace EMPILab1.Models
{
    public class QuantitativeCharacteristicItemViewModel : BindableBase
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

        private Tuple<double, double> _interval;
        public Tuple<double, double> Interval
        {
            get => _interval;
            set => SetProperty(ref _interval, value);
        }
    }
}
