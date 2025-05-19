using MechTracker.Models;
using MechTracker.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace MechTracker.Views
{
    [QueryProperty(nameof(NextPage), "nextPage")]
    public partial class SelectActiveMechPage : ContentPage
    {
        private readonly MechService _mechService;
        public ObservableCollection<Mech> Mechs { get; set; } = [];
        private string? _nextPage;
        public string? NextPage
        {
            get => _nextPage;
            set => _nextPage = value;
        }

        public SelectActiveMechPage(MechService mechService)
        {
            InitializeComponent();
            _mechService = mechService;
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Mechs.Clear();
            foreach (var mech in _mechService.GetAllMechs())
                Mechs.Add(mech);
            AdaptGridSpan();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            AdaptGridSpan();
        }

        private void AdaptGridSpan()
        {
            int span = 2; // Default for portrait phone
            var idiom = DeviceInfo.Current.Idiom;
            bool isPortrait = Width < Height;
            if (idiom == DeviceIdiom.Tablet || idiom == DeviceIdiom.Desktop)
            {
                span = isPortrait ? 4 : 6;
            }
            else // Phone or other
            {
                span = isPortrait ? 2 : 3;
            }
            if (MechCollectionView.ItemsLayout is GridItemsLayout gridLayout)
                gridLayout.Span = span;
        }

        private async void OnMechSelected(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is Mech selectedMech)
            {
                string nextPage = !string.IsNullOrEmpty(NextPage) ? NextPage : nameof(SelectPhasePage);
                await Shell.Current.GoToAsync($"{nextPage}?mechId={selectedMech.InstanceId}");
            }
        }
    }
}
