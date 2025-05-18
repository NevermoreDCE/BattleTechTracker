using MechTracker.Constants;
using MechTracker.Models;
using MechTracker.Services;
using MechTracker.ViewModels;
using Microsoft.Extensions.Logging;

namespace MechTracker.Views
{
    [QueryProperty(nameof(MechId), "mechId")]
    public partial class WeaponDamageInputPage : ContentPage
    {
        private WeaponDamageInputViewModel _viewModel;
        private readonly MechService _mechService;
        private readonly IUserPromptService _userPromptService;
        private readonly ILogger<WeaponDamageInputViewModel> _logger;
        private int _mechId;
        public int MechId
        {
            get => _mechId;
            set
            {
                _mechId = value;
                // ViewModel will be created in OnAppearing
            }
        }

        public WeaponDamageInputPage(MechService mechService, IUserPromptService userPromptService, ILogger<WeaponDamageInputViewModel> logger)
        {
            InitializeComponent();
            _mechService = mechService;
            _userPromptService = userPromptService;
            _logger = logger;
            _viewModel = new WeaponDamageInputViewModel(_mechService, _userPromptService, _logger, 0); // MechId will be set in OnAppearing
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // If MechId was set by navigation, update ViewModel
            if (_mechId != 0 && _viewModel.Mech.Id != _mechId)
            {
                _viewModel = new WeaponDamageInputViewModel(_mechService, _userPromptService, _logger, _mechId);
                BindingContext = _viewModel;
            }
            DamageStack.Children.Clear();
            var mech = _viewModel.Mech;
            for (int i = 0; i < MechConstants.ArmorLabels.Length; i++)
            {
                int currentArmor = (mech?.CurrentArmor != null && mech.CurrentArmor.Length > i) ? mech.CurrentArmor[i] : 0;
                int maxArmor = (mech?.Armor != null && mech.Armor.Length > i) ? mech.Armor[i] : 0;
                string internals = "";
                if (i < 8)
                {
                    int currentInternal = (mech?.CurrentInternals != null && mech.CurrentInternals.Length > i) ? mech.CurrentInternals[i] : 0;
                    int maxInternal = (mech?.Internals != null && mech.Internals.Length > i) ? mech.Internals[i] : 0;
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
            await _viewModel.ApplyDamageLoopAsync();
            // Refresh the display after applying damage
            OnAppearing();
        }
    }
}
