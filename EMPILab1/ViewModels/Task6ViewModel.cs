using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EMPILab1.Helpers;
using EMPILab1.Models;
using EMPILab1.Pages;
using Prism.Commands;
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

        private List<double> _initialDataset;
        public List<double> InitialDataset
        {
            get => _initialDataset;
            set => SetProperty(ref _initialDataset, value);
        }

        private List<VariantItemViewModel> _variants;
        public List<VariantItemViewModel> Variants
        {
            get => _variants;
            set => SetProperty(ref _variants, value);
        }

        private ICommand _continueCommand;
        public ICommand ContinueCommand => _continueCommand ??= new DelegateCommand(async () => await OnContinueCommandAsync());

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(List<double>), out List<double> initialDataset))
            {
                InitialDataset = new List<double>(initialDataset);
            }

            if (parameters.TryGetValue(nameof(Variants), out IEnumerable<VariantItemViewModel> variants))
            {
                Variants = new List<VariantItemViewModel>(variants);
            }

            var sortedDataset = InitialDataset.OrderBy(u => u).ToList();

            Characteristics = new ObservableCollection<QuantitativeCharacteristicItemViewModel>
            {
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Среднее арифметическое",
                    Value = MathHelpers.Mean(sortedDataset).ToString(),
                    StandardDeviation = MathHelpers.MeanStandardError(sortedDataset).ToString(),
                    ConfidenceInterval = MathHelpers.MeanConfidenceInterval(ALPHA, sortedDataset).ToString(), 
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Медиана",
                    Value = MathHelpers.Median(sortedDataset).ToString(),
                    StandardDeviation = "-",
                    ConfidenceInterval = MathHelpers.MedianConfidenceInterval(ALPHA, sortedDataset).ToString(),
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Среднеквадр. отклон.",
                    Value = MathHelpers.StandardDeviation(sortedDataset).ToString(),
                    StandardDeviation = MathHelpers.StandardDeviationStandardError(sortedDataset).ToString(),
                    ConfidenceInterval = MathHelpers.StandardDeviationConfidenceInterval(ALPHA, sortedDataset).ToString(),
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. асиметр.",
                    Value = MathHelpers.Skewness(sortedDataset).ToString(),
                    StandardDeviation = MathHelpers.SkewnessStandardError(sortedDataset).ToString(),
                    ConfidenceInterval = MathHelpers.SkewnessConfidenceInterval(ALPHA, sortedDataset).ToString(),
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. эксцеса",
                    Value = MathHelpers.Kurtosis(sortedDataset).ToString(),
                    StandardDeviation = MathHelpers.KurtosisStandardError(sortedDataset).ToString(),
                    ConfidenceInterval = MathHelpers.KurtosisConfidenceInterval(ALPHA, sortedDataset).ToString(),
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. контрэксцеса",
                    Value = MathHelpers.AntiKurtosis(sortedDataset).ToString(),
                    StandardDeviation = "...",
                    ConfidenceInterval = "...",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Мин",
                    Value = sortedDataset.FirstOrDefault().ToString(),
                    StandardDeviation = "-",
                    ConfidenceInterval = "-",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Макс",
                    Value = sortedDataset.LastOrDefault().ToString(),
                    StandardDeviation = "-",
                    ConfidenceInterval = "-",
                },
            };
        }

        private Task OnContinueCommandAsync()
        {
            var prms = new NavigationParameters
            {
                { nameof(List<double>), InitialDataset },
                { nameof(Variants), Variants },
            };

            return NavigationService.NavigateAsync(nameof(Task7), prms);
        }

        #endregion
    }
}
