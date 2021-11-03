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
                    FileContent = System.IO.File.ReadAllText(pickedFile.FullPath),
                };

                IsFileSelected = true;
            }
        }

        #endregion
    }
}
