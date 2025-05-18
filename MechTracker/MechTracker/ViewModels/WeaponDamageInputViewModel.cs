using MechTracker.Constants;
using MechTracker.Models;
using MechTracker.Services;
using Microsoft.Extensions.Logging;

namespace MechTracker.ViewModels
{
    public class WeaponDamageInputViewModel
    {
        private readonly MechService _mechService;
        private readonly IUserPromptService _userPromptService;
        private readonly ILogger<WeaponDamageInputViewModel> _logger;
        public Mech Mech { get; private set; }

        public WeaponDamageInputViewModel(MechService mechService, IUserPromptService userPromptService, ILogger<WeaponDamageInputViewModel> logger, int mechId)
        {
            _mechService = mechService;
            _userPromptService = userPromptService;
            _logger = logger;
            try
            {
                Mech = _mechService.GetMechById(mechId) ?? new Mech();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Mech by id {MechId}", mechId);
                Mech = new Mech();
            }
        }

        public async Task ApplyDamageLoopAsync()
        {
            bool dealMoreDamage = true;
            while (dealMoreDamage)
            {
                try
                {
                    var hitLocations = await GetHitLocationsArray();
                    if (hitLocations == null)
                        return;

                    int hitLocationValue = await GetHitLocationValue();
                    if (hitLocationValue < 0)
                        return;

                    int damageAmount = await GetDamageAmount();
                    if (damageAmount < 1)
                        return;

                    int armorIndex = GetArmorIndex(hitLocations, hitLocationValue);
                    string hitLocationName = hitLocations[hitLocationValue];

                    string result = ApplyDamage(damageAmount, armorIndex, hitLocationName, out int carryOver);
                    if (carryOver > 0)
                    {
                        result += "\n";
                        int carryoverIndex = GetCarryOverIndex(hitLocations, hitLocationName);
                        result += ApplyDamage(carryOver, carryoverIndex, MechConstants.ArmorLabels[carryoverIndex], out int secondCarryOver);
                        if (secondCarryOver > 0)
                        {
                            result += "\n";
                            int secondCarryoverIndex = GetCarryOverIndex(hitLocations, MechConstants.ArmorLabels[carryoverIndex]);
                            result += ApplyDamage(secondCarryOver, secondCarryoverIndex, MechConstants.ArmorLabels[secondCarryoverIndex], out _);
                        }
                    }

                    bool continueDamage = await _userPromptService.ShowAlert("Damage Applied", $"{result}\n\nApply more damage?", "Yes", "No");
                    dealMoreDamage = continueDamage;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during damage application loop");
                    await _userPromptService.ShowAlert("Error", "An error occurred while applying damage. Please try again.", "OK", "");
                    return;
                }
            }
        }

        public string ApplyDamage(int damageAmount, int armorIndex, string hitLocationName, out int carryoverDamage)
        {
            carryoverDamage = 0;
            try
            {
                bool zeroArmor = Mech.CurrentArmor[armorIndex] == 0;
                bool zeroInternals = Mech.CurrentInternals[armorIndex] == 0;
                string result;
                if (zeroArmor && zeroInternals)
                {
                    result = $"There is no armor or internals left in the {hitLocationName}.";
                    carryoverDamage = damageAmount;
                    return result;
                }
                Mech.CurrentArmor[armorIndex] -= damageAmount;
                result = $"{damageAmount} damage has been applied to {hitLocationName}";
                if (zeroArmor)
                {
                    result += " but there was no armor there. ";
                }
                if (Mech.CurrentArmor[armorIndex] < 0)
                {
                    int internalDamage = -Mech.CurrentArmor[armorIndex];
                    Mech.CurrentArmor[armorIndex] = 0;
                    if (!zeroArmor)
                    {
                        result += " and has breached the armor.";
                    }
                    result += $" {internalDamage} damage is now internal";
                    if (armorIndex > 7)
                    {
                        string internalHitLocationName = hitLocationName[..^7];
                        hitLocationName = internalHitLocationName;
                        armorIndex = Array.IndexOf(MechConstants.ArmorLabels, internalHitLocationName);
                    }
                    result += $" to the {hitLocationName}";
                    Mech.CurrentInternals[armorIndex] -= internalDamage;
                    if (Mech.CurrentInternals[armorIndex] <= 0)
                    {
                        if (hitLocationName == "Head" || hitLocationName == "Center Torso")
                        {
                            result += " and has destroyed the mech!";
                            return result;
                        }
                        carryoverDamage = -Mech.CurrentInternals[armorIndex];
                        Mech.CurrentInternals[armorIndex] = 0;
                        result += $" and has destroyed the internals. Check for criticals (8-9=1, 10-11=2, 12=3). {carryoverDamage} damage is now carry over to the next location. ";
                    }
                    else
                    {
                        result += ". Check for criticals (8-9=1, 10-11=2, 12=3).";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying damage: {DamageAmount} to {HitLocationName} (armorIndex={ArmorIndex})", damageAmount, hitLocationName, armorIndex);
                carryoverDamage = 0;
                return "An error occurred while applying damage.";
            }
        }

        public static int GetCarryOverIndex(string[] hitLocations, string? carryOverFromName)
        {
            try
            {
                if (string.IsNullOrEmpty(carryOverFromName))
                {
                    return -1;
                }
                string nextLocationName = string.Empty;
                if (carryOverFromName.EndsWith("Leg"))
                {
                    nextLocationName = carryOverFromName[..^4] + " Torso";
                }
                else if (carryOverFromName.EndsWith("Arm"))
                {
                    nextLocationName = carryOverFromName[..^4] + " Torso";
                }
                else if (carryOverFromName.EndsWith("Torso"))
                {
                    nextLocationName = "Center Torso";
                }
                if (hitLocations.Any(x => x.EndsWith(" (Rear)")))
                {
                    nextLocationName += " (Rear)";
                }
                return Array.IndexOf(MechConstants.ArmorLabels, nextLocationName);
            }
            catch
            {
                // Static method, can't log directly
                return -1;
            }
        }

        public static int GetArmorIndex(string[] hitLocations, int hitLocationValue)
        {
            try
            {
                string hitLocationName = hitLocations[hitLocationValue];
                return Array.IndexOf(MechConstants.ArmorLabels, hitLocationName);
            }
            catch (Exception)
            {
                // Static method, can't log directly
                return -1;
            }
        }

        public async Task<int> GetDamageAmount()
        {
            try
            {
                return await _userPromptService.ShowDamagePickerModal();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting damage amount");
                return -1;
            }
        }

        public async Task<int> GetHitLocationValue()
        {
            try
            {
                string[] hitLocationOptions = [.. Enumerable.Range(2, 11).Select(n => n.ToString())];
                string hitLocationValue = await _userPromptService.ShowActionSheet(
                    "Input hit location roll between 2 and 12",
                    "Cancel",
                    null,
                    hitLocationOptions);
                if (hitLocationValue == null || hitLocationValue == "Cancel")
                    return -1;
                if (!int.TryParse(hitLocationValue, out int hitLocationValueInt) || hitLocationValueInt < 2 || hitLocationValueInt > 12)
                    return -1;
                hitLocationValueInt -= 2;
                return hitLocationValueInt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hit location value");
                return -1;
            }
        }

        public async Task<string[]?> GetHitLocationsArray()
        {
            try
            {
                string direction = await _userPromptService.ShowActionSheet(
                    "Which direction is the attacking coming from?",
                    "Cancel",
                    null,
                    "Front",
                    "Left",
                    "Right",
                    "Rear");
                if (direction == null || direction == "Cancel")
                    return null;
                return direction switch
                {
                    "Front" => MechConstants.DamageLocationFront,
                    "Left" => MechConstants.DamageLocationLeft,
                    "Right" => MechConstants.DamageLocationRight,
                    "Rear" => MechConstants.DamageLocationRear,
                    _ => null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hit locations array");
                return null;
            }
        }
    }
}
