using MechTracker.Constants;
using MechTracker.Models;
using MechTracker.Services;

namespace MechTracker.Views
{
    [QueryProperty(nameof(MechId), "mechId")]
    public partial class DamageInputPage : ContentPage
    {
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

        public DamageInputPage(MechService mechService)
        {
            InitializeComponent();
            _mechService = mechService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DamageStack.Children.Clear();
            for (int i = 0; i < MechConstants.ArmorLabels.Length; i++)
            {
                int currentArmor = (_mech?.CurrentArmor != null && _mech.CurrentArmor.Length > i) ? _mech.CurrentArmor[i] : 0;
                int maxArmor = (_mech?.Armor != null && _mech.Armor.Length > i) ? _mech.Armor[i] : 0;
                string internals = "";
                if (i < 8)
                {
                    int currentInternal = (_mech?.CurrentInternals != null && _mech.CurrentInternals.Length > i) ? _mech.CurrentInternals[i] : 0;
                    int maxInternal = (_mech?.Internals != null && _mech.Internals.Length > i) ? _mech.Internals[i] : 0;
                    internals = $" (Internal: {currentInternal}/{maxInternal})";
                }
                var label = new Label { Text = $"{MechConstants.ArmorLabels[i]}: {currentArmor}/{maxArmor}{internals}" };
                DamageStack.Children.Add(label);
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
        }

        private async void OnApplyDamageClicked(object sender, EventArgs e)
        {
            bool dealMoreDamage = true;
            while (dealMoreDamage)
            {
                string direction = await DisplayActionSheet(
                    "Which direction is the attacking coming from?",
                    "Cancel",
                    null,
                    "Front",
                    "Left",
                    "Right",
                    "Rear");

                if (direction == null || direction == "Cancel")
                    return;
                string[] hitLocations = [];
                switch (direction)
                {
                    case "Front":
                        hitLocations = MechConstants.DamageLocationFront;
                        break;
                    case "Left":
                        hitLocations = MechConstants.DamageLocationLeft;
                        break;
                    case "Right":
                        hitLocations = MechConstants.DamageLocationRight;
                        break;
                    case "Rear":
                        hitLocations = MechConstants.DamageLocationRear;
                        break;
                }

                // Ask for a number between 2 and 12
                string[] hitLocationOptions = Enumerable.Range(2, 11).Select(n => n.ToString()).ToArray();
                string hitLocationValue = await DisplayActionSheet(
                    "Pick a Hit Location between 2 and 12",
                    "Cancel",
                    null,
                    hitLocationOptions);

                if (hitLocationValue == null || hitLocationValue == "Cancel")
                    return;

                if (!int.TryParse(hitLocationValue, out int hitLocationValueInt) || hitLocationValueInt < 2 || hitLocationValueInt > 12)
                    return;

                hitLocationValueInt--;
                // Show DamagePickerModal as modal and get damage amount
                var modal = new DamagePickerModal();
                await Navigation.PushModalAsync(modal);
                int damageAmount = await modal.Result;

                // You can now use direction, hitLocation, and damageAmount as needed
                string hitLocationName = hitLocations[hitLocationValueInt];
                int armorIndex = Array.IndexOf(MechConstants.ArmorLabels, hitLocationName);
                _mech.CurrentArmor[armorIndex] -= damageAmount;
                if (_mech.CurrentArmor[armorIndex] < 0)
                {
                    // go internal
                    int internalDamage = -_mech.CurrentArmor[armorIndex];
                    _mech.CurrentArmor[armorIndex] = 0;
                    if (armorIndex > 7)
                    {
                        // get the hit location name without the word " (Rear)" at the end
                        string internalHitLocationName = hitLocationName.Substring(0, hitLocationName.Length - 7);
                        hitLocationName = internalHitLocationName;
                        armorIndex = Array.IndexOf(MechConstants.ArmorLabels, internalHitLocationName);
                    }
                    _mech.CurrentInternals[armorIndex] -= internalDamage;
                    if (_mech.CurrentInternals[armorIndex] < 0)
                    {
                        // alert to carry over to next location
                        int carryoverDamage = -_mech.CurrentInternals[armorIndex];
                        _mech.CurrentInternals[armorIndex] = 0;
                        await DisplayAlert("Carry Over Damage", $"Carry over {carryoverDamage} damage to the next location.", "OK");
                    }
                }
                bool continueDamage = await DisplayAlert("Damage Applied", $"Damage ({damageAmount}) has been applied to {hitLocationName}, apply more damage?", "Yes", "No");
                dealMoreDamage = continueDamage;
                               
            }
        }
    }
}
