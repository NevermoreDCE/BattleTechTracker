using Microsoft.Maui.Controls;

namespace MechTracker.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCreateNewMechClicked(object sender, EventArgs e)
        {
            // navigate to Create New Mech page
            await Shell.Current.GoToAsync(nameof(CreateMechPage));
        }

        private async void OnLoadExistingMechClicked(object sender, EventArgs e)
        {
            // TODO: Navigate to Load Existing Mech page
            await Shell.Current.GoToAsync(nameof(LoadMechPage));
        }
    }
}
