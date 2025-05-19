using Microsoft.Maui.Controls;
using MechTracker.Models;
using MechTracker.Services;
using MechTracker.Constants;

namespace MechTracker.Views
{
    [QueryProperty(nameof(MechId), "mechId")]
    public partial class SetArmorPage : ContentPage
    {
        private readonly Entry[] _armorEntries = new Entry[11];
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

        public SetArmorPage(MechService mechService)
        {
            InitializeComponent();
            _mechService = mechService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ArmorGrid.Children.Clear();
            int numRows = (MechConstants.ArmorLabels.Length + 1) / 2;
            for (int i = 0; i < MechConstants.ArmorLabels.Length; i++)
            {
                int row = i % numRows;
                int col = i / numRows;
                var label = new Label { Text = MechConstants.ArmorLabels[i], VerticalOptions = LayoutOptions.Center };
                var entry = new Entry { Keyboard = Keyboard.Numeric, Text = (_mech?.Armor != null && _mech.Armor.Length > i) ? _mech.Armor[i].ToString() : "0" };
                _armorEntries[i] = entry;
                ArmorGrid.Add(label, col, row * 2);
                ArmorGrid.Add(entry, col, row * 2 + 1);
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            UpdateMechArmor();
            await Shell.Current.GoToAsync("..", true);
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            UpdateMechArmor();
            await Shell.Current.GoToAsync($"SetInternalsPage?mechId={_mech.Id}");
        }

        private void UpdateMechArmor()
        {
            if (_mech == null) return;
            if (_mech.Armor == null || _mech.Armor.Length != MechConstants.ArmorLabels.Length)
                _mech.Armor = new int[MechConstants.ArmorLabels.Length];
            for (int i = 0; i < MechConstants.ArmorLabels.Length; i++)
            {
                if (int.TryParse(_armorEntries[i].Text, out int value))
                {
                    var armor = _mech.Armor;
                    armor[i] = value;
                    _mech.Armor = armor;
                }
            }
        }
    }
}
