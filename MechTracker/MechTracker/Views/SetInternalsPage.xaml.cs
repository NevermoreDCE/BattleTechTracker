using MechTracker.Constants;
using MechTracker.Models;
using MechTracker.Services;

namespace MechTracker.Views
{
    [QueryProperty(nameof(MechId), "mechId")]
    public partial class SetInternalsPage : ContentPage
    {
        private readonly Entry[] _internalEntries = new Entry[MechConstants.InternalLabels.Length];
        private Mech _mech = new();
        private readonly MechService _mechService;
        private int _mechId;
        public int MechId
        {
            get => _mechId;
            set
            {
                _mechId = value;
                _mech = _mechService.GetMechById(_mechId) ?? new Mech();
            }
        }

        public SetInternalsPage(MechService mechService)
        {
            InitializeComponent();
            _mechService = mechService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InternalsGrid.Children.Clear();
            int numRows = (MechConstants.InternalLabels.Length + 1) / 2;
            for (int i = 0; i < MechConstants.InternalLabels.Length; i++)
            {
                int row = i % numRows;
                int col = i / numRows;
                var label = new Label { Text = MechConstants.InternalLabels[i], VerticalOptions = LayoutOptions.Center };
                var entry = new Entry { Keyboard = Keyboard.Numeric, Text = (_mech?.Internals != null && _mech.Internals.Length > i) ? _mech.Internals[i].ToString() : "0" };
                _internalEntries[i] = entry;
                InternalsGrid.Add(label, col, row * 2);
                InternalsGrid.Add(entry, col, row * 2 + 1);
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            UpdateMechInternals();
            await Shell.Current.GoToAsync("..", true);
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            UpdateMechInternals();
            await Shell.Current.GoToAsync(nameof(SelectPhasePage));
        }

        private void UpdateMechInternals()
        {
            if (_mech == null) return;
            if (_mech.Internals == null || _mech.Internals.Length != MechConstants.InternalLabels.Length)
                _mech.Internals = new int[MechConstants.InternalLabels.Length];
            for (int i = 0; i < MechConstants.InternalLabels.Length; i++)
            {
                if (int.TryParse(_internalEntries[i].Text, out int value))
                {
                    var internals = _mech.Internals;
                    internals[i] = value;
                    _mech.Internals = internals;
                }
            }
        }
    }
}
