using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace MechTracker.Views
{
    public partial class DamagePickerModal : ContentPage
    {
        private readonly TaskCompletionSource<int> _tcs = new();
        public Task<int> Result => _tcs.Task;
        public int SelectedValue { get; private set; } = 8;
        public DamagePickerModal()
        {
            InitializeComponent();
            NumberEntry.Text = "8";
        }

        private async void OnQuickPickClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Text, out int value))
            {
                _tcs.TrySetResult(value);
                await Shell.Current.Navigation.PopModalAsync();
            }
        }

        private async void OnApplyClicked(object sender, EventArgs e)
        {
            if (int.TryParse(NumberEntry.Text, out int value))
            {
                SelectedValue = value;
                _tcs.TrySetResult(value);
                await Shell.Current.Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number.", "OK");
            }
        }
    }
}
