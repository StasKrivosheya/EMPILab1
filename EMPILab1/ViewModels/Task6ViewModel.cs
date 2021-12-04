using System.Collections.ObjectModel;
using EMPILab1.Models;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Task6ViewModel : BaseViewModel
    {
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

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            Characteristics = new ObservableCollection<QuantitativeCharacteristicItemViewModel>
            {
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Сред. арифметич.",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Медиана",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Среднеквадр.",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. асиметр.",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. эксцеса",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Коэф. контрэксцеса",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Мин",
                },
                new QuantitativeCharacteristicItemViewModel
                {
                    Name = "Макс",
                },
            };
        }

        #endregion
    }
}
