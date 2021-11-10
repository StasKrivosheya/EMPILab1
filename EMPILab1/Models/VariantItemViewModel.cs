using Prism.Mvvm;

namespace EMPILab1.Models
{
    public class VariantItemViewModel : BindableBase
    {
        #region -- Public properties --

        private int _index;
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        private double _value;
        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private int _frequency;
        public int Frequency
        {
            get => _frequency;
            set => SetProperty(ref _frequency, value);
        }

        private double _relativeFrequency;
        public double RelativeFrequency
        {
            get => _relativeFrequency;
            set => SetProperty(ref _relativeFrequency, value);
        }

        private double _empiricalDistrFuncValue;
        public double EmpiricalDistrFuncValue
        {
            get => _empiricalDistrFuncValue;
            set => SetProperty(ref _empiricalDistrFuncValue, value);
        }

        #endregion
    }
}
