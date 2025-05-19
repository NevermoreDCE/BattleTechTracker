using Microsoft.Maui.Controls;

namespace MechTracker.Views
{
    public partial class SelectPhasePage : ContentPage
    {
        public SelectPhasePage()
        {
            InitializeComponent();
        }

        private void OnMovementClicked(object sender, EventArgs e)
        {
            // TODO: Implement Movement phase logic
        }

        private void OnWeaponShootingClicked(object sender, EventArgs e)
        {
            // TODO: Implement Weapon Shooting phase logic
        }

        private async void OnWeaponDamageClicked(object sender, EventArgs e)
        {
            // Navigate to SelectActiveMechPage and pass WeaponDamageInputPage as the next page parameter
            await Shell.Current.GoToAsync($"{nameof(SelectActiveMechPage)}?nextPage={nameof(WeaponDamageInputPage)}");
        }

        private void OnPhysicalAttacksClicked(object sender, EventArgs e)
        {
            // TODO: Implement Physical Attacks phase logic
        }

        private void OnPhysicalDamageClicked(object sender, EventArgs e)
        {
            // TODO: Implement Physical Damage phase logic
        }

        private void OnHeatResolutionClicked(object sender, EventArgs e)
        {
            // TODO: Implement Heat Resolution phase logic
        }

        private void OnEndPhaseClicked(object sender, EventArgs e)
        {
            // TODO: Implement End Phase logic
        }
    }
}
