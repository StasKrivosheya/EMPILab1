using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EMPILab1.Models;
using Prism.Commands;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Page1ViewModel : BaseViewModel
    {
        public Page1ViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        #region -- Public properties --

        private string _classesAmount;
        public string ClassesAmount
        {
            get => _classesAmount;
            set => SetProperty(ref _classesAmount, value);
        }

        private double _classWidth;
        public double ClassWidth
        {
            get => _classWidth;
            set => SetProperty(ref _classWidth, value);
        }

        private double _minValue;
        public double MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value);
        }

        private double _maxValue;
        public double MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }

        private ObservableCollection<VariantItemViewModel> _variants;
        public ObservableCollection<VariantItemViewModel> Variants
        {
            get => _variants;
            set => SetProperty(ref _variants, value);
        }

        private ObservableCollection<ClassViewModel> _classes;
        public ObservableCollection<ClassViewModel> Classes
        {
            get => _classes;
            set => SetProperty(ref _classes, value);
        }

        private ICommand _recalculateCommand;
        public ICommand RecalculateCommand => _recalculateCommand ??= new DelegateCommand(OnRecalculateCommandAsync);

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(Variants), out IEnumerable<VariantItemViewModel> variants))
            {
                Variants = new(variants);
            }

            var optimalClassCount = GetOptimalClassCount();
            var classes = SplitOnClasses(optimalClassCount);

            ClassesAmount = optimalClassCount.ToString();
            Classes = new(classes);
        }

        #endregion

        #region -- Private helpers --

        private void OnRecalculateCommandAsync()
        {
            if (int.TryParse(ClassesAmount, out var number))
            {
                var classes = SplitOnClasses(number);

                Classes = new(classes);
            }
        }

        private IEnumerable<ClassViewModel> SplitOnClasses(int classCount)
        {
            var minVal = MinValue = Variants.AsQueryable().Min(v => v.Value);
            var maxVal = MaxValue = Variants.AsQueryable().Max(v => v.Value);
            var h = ClassWidth = Math.Round((maxVal - minVal) / classCount, 4, MidpointRounding.AwayFromZero);

            var classes = new List<ClassViewModel>();

            var empiricalDistrFuncValue = 0d;
            for (int i = 1; i <= classCount; i++)
            {
                var leftBound = minVal;
                var rightBound = i == classCount
                    ? maxVal
                    : minVal + h;

                classes.Add(new ClassViewModel
                {
                    Index = i,
                    ClassName = $"p{i}",
                    ClassWidth = h,
                    Bounds = new Tuple<double, double>(leftBound, rightBound),
                });
                
                classes[i - 1].Frequency = Variants
                    .Where(v => v.Value >= leftBound && v.Value <= rightBound)
                    .Count();
                classes[i - 1].RelativeFrequency = classes[i - 1].Frequency / Variants.Count;
                classes[i - 1].EmpiricalDistrFuncValue = empiricalDistrFuncValue += classes[i - 1].RelativeFrequency;

                minVal += h;
            }

            return classes;
        }

        private int GetOptimalClassCount()
        {
            return (int)Math.Round(1 + 3.32 * Math.Log10(Variants.Count), 0);
        }

        #endregion
    }
}
