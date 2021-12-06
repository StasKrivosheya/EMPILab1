using Prism.Mvvm;

namespace EMPILab1.Models
{
    public class ScatterPointViewModel : BindableBase
    {
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
    }
}
