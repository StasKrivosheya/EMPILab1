using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EMPILab1.Helpers;
using EMPILab1.Models;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Task6ViewModel : BaseViewModel
    {
        private const double ALPHA = 0.05;

        public Task6ViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        private ObservableCollection<QuantitativeCharacteristicItemViewModel> _characteristics;
        public ObservableCollection<QuantitativeCharacteristicItemViewModel> Characteristics
        {
            get => _characteristics;
            set => SetProperty(ref _characteristics, value);
        }

        private List<double> _sortedInitialDataset;
        public List<double> SortedInitialDataset
        {
            get => _sortedInitialDataset;
            set => SetProperty(ref _sortedInitialDataset, value);
        }

        private List<VariantItemViewModel> _variants;
        public List<VariantItemViewModel> Variants
        {
            get => _variants;
            set => SetProperty(ref _variants, value);
        }

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(List<double>), out List<double> initialDataset))
            {
                initialDataset.Sort();
                SortedInitialDataset = new List<double>(initialDataset);
            }

            if (parameters.TryGetValue(nameof(Variants), out IEnumerable<VariantItemViewModel> variants))
            {
                Variants = new List<VariantItemViewModel>(variants);
            }

            Characteristics = new ObservableCollection<QuantitativeCharacteristicItemViewModel>
            {
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Среднее арифметическое",
                    Value = MathHelpers.Mean(SortedInitialDataset).ToString(),
                    StandardDeviation = MathHelpers.MeanStandardError(SortedInitialDataset).ToString(),
                    ConfidenceInterval = MathHelpers.MeanConfidenceInterval(ALPHA, SortedInitialDataset).ToString(), 
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Медиана",
                    Value = MathHelpers.Median(SortedInitialDataset).ToString(),
                    StandardDeviation = "-",
                    ConfidenceInterval = MathHelpers.MedianConfidenceInterval(ALPHA, SortedInitialDataset).ToString(),
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Среднеквадр. отклон.",
                    Value = MathHelpers.StandardDeviation(SortedInitialDataset).ToString(),
                    StandardDeviation = MathHelpers.StandardDeviationStandardError(SortedInitialDataset).ToString(),
                    ConfidenceInterval = MathHelpers.StandardDeviationConfidenceInterval(ALPHA, SortedInitialDataset).ToString(),
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. асиметр.",
                    Value = MathHelpers.Skewness(SortedInitialDataset).ToString(),
                    StandardDeviation = MathHelpers.SkewnessStandardError(SortedInitialDataset).ToString(),
                    ConfidenceInterval = MathHelpers.SkewnessConfidenceInterval(ALPHA, SortedInitialDataset).ToString(),
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. эксцеса",
                    Value = MathHelpers.Kurtosis(SortedInitialDataset).ToString(),
                    StandardDeviation = MathHelpers.KurtosisStandardError(SortedInitialDataset).ToString(),
                    ConfidenceInterval = MathHelpers.KurtosisConfidenceInterval(ALPHA, SortedInitialDataset).ToString(),
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. контрэксцеса",
                    Value = MathHelpers.AntiKurtosis(SortedInitialDataset).ToString(),
                    StandardDeviation = "...",
                    ConfidenceInterval = "...",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Мин",
                    Value = SortedInitialDataset.FirstOrDefault().ToString(),
                    StandardDeviation = "-",
                    ConfidenceInterval = "-",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Макс",
                    Value = SortedInitialDataset.LastOrDefault().ToString(),
                    StandardDeviation = "-",
                    ConfidenceInterval = "-",
                },
            };
        }

        #endregion
    }
}
