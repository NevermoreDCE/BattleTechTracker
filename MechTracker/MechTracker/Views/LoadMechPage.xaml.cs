using MechTracker.Models;
using MechTracker.Data;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace MechTracker.Views
{
    public partial class LoadMechPage : ContentPage
    {
        private readonly MechRepository _mechRepository;
        public ObservableCollection<Mech> Mechs { get; set; } = [];

        public LoadMechPage(MechRepository mechRepository)
        {
            InitializeComponent();
            _mechRepository = mechRepository;
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Mechs.Clear();
            var mechs = await _mechRepository.GetMechsAsync();
            foreach (var mech in mechs)
            {
                mech.InitMech();
                Mechs.Add(mech);
            }
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

        private void OnMechSelected(object? sender, SelectionChangedEventArgs e)
        {
            // TODO: Handle mech selection for loading
        }
    }
}
