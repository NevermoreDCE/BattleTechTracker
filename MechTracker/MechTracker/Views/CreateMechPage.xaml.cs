using Microsoft.Maui.Controls;
using MechTracker.Models;
using MechTracker.Services;
using System.Text.Json;

namespace MechTracker.Views
{
    [QueryProperty(nameof(MechId), "mechId")]
    public partial class CreateMechPage : ContentPage
    {
        private readonly MechService _mechService;
        private Mech _mech = new();
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

        public CreateMechPage(MechService mechService)
        {
            InitializeComponent();
            _mechService = mechService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Update UI fields with loaded values
            NameEntry.Text = "Sparky";//_mech.Name;
            ChassisEntry.Text = "Griffin";// _mech.Chassis ?? string.Empty;
            ModelEntry.Text = "GRF-1E";// _mech.Model ?? string.Empty;
            WeightEntry.Text = "55";// _mech.Weight.ToString();
            WalkingSpeedEntry.Text = "5";// _mech.WalkingSpeed.ToString();
            RunningSpeedEntry.Text = "8";// _mech.RunningSpeed.ToString();
            JumpingSpeedEntry.Text = "5";// _mech.JumpingSpeed.ToString();
            HeatSinksEntry.Text = "13";// _mech.HeatSinks.ToString();
            PilotSkillEntry.Text = "5"; // _mech.PilotSkill.ToString();
            GunnerySkillEntry.Text = "4"; // _mech.GunnerySkill.ToString();
        }

        private void OnWeightChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(WeightEntry.Text, out int weight))
                _mech.Weight = weight;
            if (!int.TryParse(WeightEntry.Text, out weight) || weight < 25 || weight > 100)
            {
                WeightValidationLabel.Text = "Weight must be a number between 25 and 100.";
                WeightValidationLabel.IsVisible = true;
            }
            else
            {
                WeightValidationLabel.IsVisible = false;
            }
            ValidateNextButton();
        }

        private void OnWalkingSpeedChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(WalkingSpeedEntry.Text, out int walkingSpeed))
                _mech.WalkingSpeed = walkingSpeed;
            WalkingSpeedValidationLabel.IsVisible = false;
            ValidateNextButton();
            // Running speed may depend on this value
            OnRunningSpeedChanged(this, e);
        }

        private void OnRunningSpeedChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RunningSpeedEntry.Text, out int runningSpeed))
                _mech.RunningSpeed = runningSpeed;
            if (int.TryParse(WalkingSpeedEntry.Text, out int walkingSpeed) &&
                int.TryParse(RunningSpeedEntry.Text, out runningSpeed))
            {
                if (runningSpeed <= walkingSpeed)
                {
                    RunningSpeedValidationLabel.Text = "Running Speed must be greater than Walking Speed.";
                    RunningSpeedValidationLabel.IsVisible = true;
                }
                else
                {
                    RunningSpeedValidationLabel.IsVisible = false;
                }
            }
            else
            {
                RunningSpeedValidationLabel.IsVisible = false;
            }
            ValidateNextButton();
        }

        private void OnJumpingSpeedChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(JumpingSpeedEntry.Text, out int jumpingSpeed))
                _mech.JumpingSpeed = jumpingSpeed;
        }

        private void OnHeatSinksChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(HeatSinksEntry.Text, out int heatSinks))
                _mech.HeatSinks = heatSinks;
        }

        private void OnNameChanged(object sender, TextChangedEventArgs e)
        {
            _mech.Name = NameEntry.Text;
        }

        private void OnPilotSkillChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(PilotSkillEntry.Text, out int pilotSkill))
                _mech.PilotSkill = pilotSkill;
        }

        private void OnGunnerySkillChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(GunnerySkillEntry.Text, out int gunnerySkill))
                _mech.GunnerySkill = gunnerySkill;
        }

        private void OnChassisChanged(object sender, TextChangedEventArgs e)
        {
            _mech.Chassis = ChassisEntry.Text;
        }

        private void OnModelChanged(object sender, TextChangedEventArgs e)
        {
            _mech.Model = ModelEntry.Text;
        }

        private void ValidateNextButton()
        {
            bool valid = !WeightValidationLabel.IsVisible && !RunningSpeedValidationLabel.IsVisible;
            NextButton.IsEnabled = valid;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            // Discard the Mech and go back
            _mech = new Mech();
            await Shell.Current.GoToAsync("..", true);
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            // Add the mech to the service if not already tracked
            if (_mech.Id == 0)
                _mech.Id = _mechService.LoadedMechs.Count > 0 ? _mechService.LoadedMechs.Max(m => m.Id) + 1 : 1;
            UpdateMech();
            _mechService.AddMech(_mech);
            await Shell.Current.GoToAsync($"SetArmorPage?mechId={_mech.Id}");
        }

        private void UpdateMech()
        {
            _mech.Name = NameEntry.Text;
            _mech.Chassis = ChassisEntry.Text;
            _mech.Model = ModelEntry.Text;
            if (int.TryParse(WeightEntry.Text, out int weight))
                _mech.Weight = weight;
            if (int.TryParse(WalkingSpeedEntry.Text, out int walkingSpeed))
                _mech.WalkingSpeed = walkingSpeed;
            if (int.TryParse(RunningSpeedEntry.Text, out int runningSpeed))
                _mech.RunningSpeed = runningSpeed;
            if (int.TryParse(JumpingSpeedEntry.Text, out int jumpingSpeed))
                _mech.JumpingSpeed = jumpingSpeed;
            if (int.TryParse(HeatSinksEntry.Text, out int heatSinks))
                _mech.HeatSinks = heatSinks;
            if (int.TryParse(PilotSkillEntry.Text, out int pilotSkill))
                _mech.PilotSkill = pilotSkill;
            if (int.TryParse(GunnerySkillEntry.Text, out int gunnerySkill))
                _mech.GunnerySkill = gunnerySkill;
        }
    }
}
