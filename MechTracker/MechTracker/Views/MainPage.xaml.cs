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
            // TODO: Navigate to Create New Mech page
            await DisplayAlert("Stub", "Navigate to Create New Mech view", "OK");
        }

        private async void OnLoadExistingMechClicked(object sender, EventArgs e)
        {
            // TODO: Navigate to Load Existing Mech page
            await DisplayAlert("Stub", "Navigate to Load Existing Mech view", "OK");
        }
    }
}
