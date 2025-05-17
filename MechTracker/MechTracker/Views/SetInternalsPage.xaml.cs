using MechTracker.Constants;
using MechTracker.Models;
using MechTracker.Services;

namespace MechTracker.Views
{
    [QueryProperty(nameof(MechId), "mechId")]
    public partial class SetInternalsPage : ContentPage
    {
        private Entry[] _internalEntries = new Entry[MechConstants.InternalLabels.Length];
        private Mech _mech;
        private readonly MechService _mechService;
        private int _mechId;
        public int MechId
        {
            get => _mechId;
            set
            {
                _mechId = value;
                _mech = _mechService.GetMechById(_mechId);
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
            InternalsStack.Children.Clear();
            for (int i = 0; i < MechConstants.InternalLabels.Length; i++)
            {
                var label = new Label { Text = MechConstants.InternalLabels[i] };
                var entry = new Entry { Keyboard = Keyboard.Numeric, Text = (_mech?.Internals != null && _mech.Internals.Length > i) ? _mech.Internals[i].ToString() : "0" };
                _internalEntries[i] = entry;
                InternalsStack.Children.Add(label);
                InternalsStack.Children.Add(entry);
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
            await Shell.Current.GoToAsync($"DamageInputPage?mechId={_mech.Id}");
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
