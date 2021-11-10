using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EMPILab1.Models;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Essentials;

namespace EMPILab1.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel(
            INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        private FileItemViewModel _selectedFile;
        public FileItemViewModel SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        private bool _isFileSelected;
        public bool IsFileSelected
        {
            get => _isFileSelected;
            set => SetProperty(ref _isFileSelected, value);
        }

        private ObservableCollection<VariantItemViewModel> _variants;
        public ObservableCollection<VariantItemViewModel> Variants
        {
            get => _variants;
            set => SetProperty(ref _variants, value);
        }

        private ICommand _LoadFileCommand;
        public ICommand LoadFileCommand => _LoadFileCommand ??= new DelegateCommand(async () => await OnLoadFileCommandAync());

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
        }

        #endregion

        #region -- Private helpers --

        private async Task OnLoadFileCommandAync()
        {
            var options = new PickOptions
            {
                PickerTitle = "Выберите .dat или .txt файл",
            };

            var pickedFile = await FilePicker.PickAsync(options);

            if (pickedFile != null)
            {
                SelectedFile = new FileItemViewModel
                {
                    FileName = pickedFile.FileName,
                    FileContent = System.IO.File.ReadAllLines(pickedFile.FullPath),
                };

                CalculateModels();

                IsFileSelected = true;
            }
        }

        private void CalculateModels()
        {
            var valuesList = new List<double>();

            foreach (var str in SelectedFile.FileContent)
            {
                if (double.TryParse(str, out var num))
                {
                    valuesList.Add(num);
                }
            }

            //valuesList = new List<double> { 0.5, 1.2, 1.2, 3, 4, 5, 5, 5, 7.3, 8 };

            valuesList.Sort();

            var uniqueValues = valuesList.GroupBy(v => v);

            var variantsList = new List<VariantItemViewModel>();

            var i = 1;
            var empiricalDistrFuncValue = 0d;
            foreach (var group in uniqueValues)
            {
                var variant = new VariantItemViewModel
                {
                    Index = i,
                    Value = group.Key,
                    Frequency = group.Count(),
                    RelativeFrequency = (double)group.Count() / uniqueValues.Count(),
                    EmpiricalDistrFuncValue = empiricalDistrFuncValue += (double)group.Count() / uniqueValues.Count(),
                };

                variantsList.Add(variant);

                ++i;
            }
            
            Variants = new ObservableCollection<VariantItemViewModel>(variantsList);
        }

        #endregion
    }
}
