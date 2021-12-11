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

        // double
        private string _value;
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        // double
        private string _standardDeviation;
        public string StandardDeviation
        {
            get => _standardDeviation;
            set => SetProperty(ref _standardDeviation, value);
        }

        // Tuple<double, double>
        private string _confidenceInterval;
        public string ConfidenceInterval
        {
            get => _confidenceInterval;
            set => SetProperty(ref _confidenceInterval, value);
        }
    }
}
