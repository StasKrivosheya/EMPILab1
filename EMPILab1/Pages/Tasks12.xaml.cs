using EMPILab1.ViewModels;
using Xamarin.Forms;

namespace EMPILab1.Pages
{
    public partial class Tasks12 : ContentPage
    {
        public Tasks12()
        {
            InitializeComponent();
        }

        // HACK: MVVM violation
        void Button_Clicked(object sender, System.EventArgs e)
        {
            var lastIndex = ((Tasks12ViewModel)BindingContext).Variants.Count - 1;
            Collection.ScrollTo(lastIndex);
        }
    }
}
