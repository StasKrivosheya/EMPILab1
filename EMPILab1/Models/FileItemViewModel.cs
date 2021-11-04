using Prism.Mvvm;

namespace EMPILab1.Models
{
    public class FileItemViewModel : BindableBase
    {
        #region -- Public properties --

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        private string[] _fileContent;
        public string[] FileContent
        {
            get => _fileContent;
            set => SetProperty(ref _fileContent, value);
        }

        #endregion
    }
}
