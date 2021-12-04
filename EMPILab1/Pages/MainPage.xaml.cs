using EMPILab1.ViewModels;
using Xamarin.Forms;

namespace EMPILab1.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // HACK: MVVM violation
        void Button_Clicked(object sender, System.EventArgs e)
        {
            var lastIndex = ((MainPageViewModel)BindingContext).Variants.Count - 1;
            Collection.ScrollTo(lastIndex);
        }
    }
}
