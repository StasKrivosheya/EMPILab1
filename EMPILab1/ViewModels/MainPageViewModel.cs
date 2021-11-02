using System.Collections.ObjectModel;
using EMPILab1.Models;
using Prism.Navigation;

namespace EMPILab1.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            AreaData = new ObservableCollection<TestModel>()
            {
                new TestModel("Jan", 50),
                new TestModel("Feb", 70),
                new TestModel("Mar", 65),
                new TestModel("Apr", 57),
                new TestModel("May", 48),
            };

            ColumnData = new ObservableCollection<TestModel>()
            {
                new TestModel("Jan", 20),
                new TestModel("Feb", 30),
                new TestModel("Mar", 25),
                new TestModel("Apr", 47),
                new TestModel("May", 38),
            };
        }

        public ObservableCollection<TestModel> AreaData { get; set; }

        public ObservableCollection<TestModel> ColumnData { get; set; }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
        }
    }
}
