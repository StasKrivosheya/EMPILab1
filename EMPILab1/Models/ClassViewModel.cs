using System;
using Prism.Mvvm;

namespace EMPILab1.Models
{
    public class ClassViewModel : BindableBase
    {
        private int _index;
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        private string _className;
        public string ClassName
        {
            get => _className;
            set => SetProperty(ref _className, value);
        }

        private double _classWidth;
        public double ClassWidth
        {
            get => _classWidth;
            set => SetProperty(ref _classWidth, value);
        }

        private Tuple<double, double> _bounds;
        public Tuple<double, double> Bounds
        {
            get => _bounds;
            set => SetProperty(ref _bounds, value);
        }

        private double _frequency;
        public double Frequency
        {
            get => _frequency;
            set => SetProperty(ref _frequency, value);
        }

        private double _theoreticalFrequency;
        public double TheoreticalFrequency
        {
            get => _theoreticalFrequency;
            set => SetProperty(ref _theoreticalFrequency, value);
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
    }
}
