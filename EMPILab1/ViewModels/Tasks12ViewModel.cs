using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using EMPILab1.Events;
using EMPILab1.Extensions;
using EMPILab1.Models;
using EMPILab1.Pages;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Xamarin.Essentials;

namespace EMPILab1.ViewModels
{
    public class Tasks12ViewModel : BaseViewModel
    {
        public Tasks12ViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator)
            : base(navigationService)
        {
            eventAggregator.GetEvent<InitialDatasetChanged>().Subscribe(OnInitialDatasetChanged);
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

        private List<double> _initialDataset;
        public List<double> InitialDataset
        {
            get => _initialDataset;
            set => SetProperty(ref _initialDataset, value);
        }

        private ICommand _loadFileCommand;
        public ICommand LoadFileCommand => _loadFileCommand ??= new DelegateCommand(async () => await OnLoadFileCommandAsync());

        private ICommand _continueCommand;
        public ICommand ContinueCommand => _continueCommand ??= new DelegateCommand(async () => await OnContinueCommandAsync());

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(InitialDataset))
            {
                Variants = new(InitialDataset.ToVariantsList());
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnContinueCommandAsync()
        {
            var prms = new NavigationParameters
            {
                { nameof(Variants), Variants },
                { nameof(InitialDataset), InitialDataset },
            };

            return NavigationService.NavigateAsync(nameof(Tasks345), prms);
        }

        private async Task OnLoadFileCommandAsync()
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
            var valuesList = InitialDataset = GetParsedListOfData(SelectedFile.FileContent);

            // test
            // valuesList = new List<double> { 0.5, 1.2, 1.2, 3, 4, 5, 5, 5, 7.3, 8 };

            Variants = new(valuesList.ToVariantsList());
        }

        private List<double> GetParsedListOfData(string[] fileContent)
        {
            var valuesList = new List<double>();

            foreach (var str in fileContent)
            {
                var preparedStr = str.Trim();

                if (preparedStr.Contains(" "))
                {
                    var strs = preparedStr.Split(" ".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

                    foreach (var s in strs)
                    {
                        if (double.TryParse(s, out double num))
                        {
                            valuesList.Add(num);
                        }
                    }
                }
                else if (double.TryParse(str, out var num))
                {
                    valuesList.Add(num);
                }
            }

            return valuesList;
        }

        private void OnInitialDatasetChanged(List<double> newDataset)
        {
            InitialDataset = new List<double>(newDataset);
        }

        #endregion
    }
}
