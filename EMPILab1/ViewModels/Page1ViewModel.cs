using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EMPILab1.Models;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class Page1ViewModel : BaseViewModel
    {
        public Page1ViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        #region -- Public properties --

        private ObservableCollection<VariantItemViewModel> _variants;
        public ObservableCollection<VariantItemViewModel> Variants
        {
            get => _variants;
            set => SetProperty(ref _variants, value);
        }

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(Variants), out IEnumerable<VariantItemViewModel> variants))
            {
                Variants = new ObservableCollection<VariantItemViewModel>(variants);
            }

            var optimalClassCount = GetOptimalClassCount();
            SplitOnClasses(optimalClassCount);
        }

        #endregion

        #region -- Private helpers --

        private void SplitOnClasses(int classCount)
        {

        }

        private int GetOptimalClassCount()
        {
            return (int)Math.Round(1 + 3.32 * Math.Log10(Variants.Count), 0);
        }

        #endregion
    }
}
