using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MechTracker.Models;
using SQLite;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Linq;

namespace MechTracker.Data
{
    public class MechRepository
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly ILogger<MechRepository> _logger;

        public MechRepository(SQLiteAsyncConnection database, ILogger<MechRepository> logger)
        {
            _database = database;
            _logger = logger;
            _database.CreateTableAsync<Mech>().Wait();
            _database.CreateTableAsync<Weapon>().Wait();
            _database.CreateTableAsync<MechCriticalLocation>().Wait();
        }

        // Helper: Save Locations
        private async Task SaveMechLocationsAsync(int mechId, Critical[][] locations)
        {
            try
            {
                // Remove old locations
                await _database.Table<MechCriticalLocation>().Where(x => x.MechId == mechId).DeleteAsync();
                if (locations == null) return;
                var toInsert = new List<MechCriticalLocation>();
                for (int locIdx = 0; locIdx < locations.Length; locIdx++)
                {
                    var loc = locations[locIdx];
                    if (loc == null) continue;
                    for (int slotIdx = 0; slotIdx < loc.Length; slotIdx++)
                    {
                        var crit = loc[slotIdx];
                        if (crit == null) continue;
                        var mcl = new MechCriticalLocation
                        {
                            MechId = mechId,
                            LocationIndex = locIdx,
                            SlotIndex = slotIdx,
                            Name = crit.Name,
                            IsDestroyed = crit.IsDestroyed
                        };
                        if (crit is Weapon weapon)
                        {
                            mcl.Damage = weapon.Damage;
                            mcl.HeatGenerated = weapon.HeatGenerated;
                            mcl.ShortRange = weapon.ShortRange;
                            mcl.MediumRange = weapon.MediumRange;
                            mcl.LongRange = weapon.LongRange;
                            mcl.MinimumRange = weapon.MinimumRange;
                            mcl.CriticalSlots = weapon.CriticalSlots;
                        }
                        toInsert.Add(mcl);
                    }
                }
                if (toInsert.Count > 0)
                    await _database.InsertAllAsync(toInsert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving MechLocations for MechId {parameters}", mechId);
            }
        }

        // Helper: Load Locations
        private async Task<Critical[][]> LoadMechLocationsAsync(int mechId)
        {
            try
            {
                var locations = new Critical[8][];
                // 3 locations with 6 slots, rest with 12
                int[] slotCounts = [6, 12, 12, 12, 12, 12, 6, 6];
                for (int i = 0; i < 8; i++)
                    locations[i] = new Critical[slotCounts[i]];
                var dbLocs = await _database.Table<MechCriticalLocation>().Where(x => x.MechId == mechId).ToListAsync();
                foreach (var dbLoc in dbLocs)
                {
                    if (dbLoc.LocationIndex < 0 || dbLoc.LocationIndex >= 8) continue;
                    if (dbLoc.SlotIndex < 0 || dbLoc.SlotIndex >= locations[dbLoc.LocationIndex].Length) continue;
                    if (dbLoc.Damage.HasValue && dbLoc.HeatGenerated.HasValue && dbLoc.ShortRange.HasValue && dbLoc.MediumRange.HasValue && dbLoc.LongRange.HasValue && dbLoc.MinimumRange.HasValue && dbLoc.CriticalSlots.HasValue)
                    {
                        locations[dbLoc.LocationIndex][dbLoc.SlotIndex] = new Weapon
                        {
                            Name = dbLoc.Name,
                            IsDestroyed = dbLoc.IsDestroyed,
                            Damage = dbLoc.Damage.Value,
                            HeatGenerated = dbLoc.HeatGenerated.Value,
                            ShortRange = dbLoc.ShortRange.Value,
                            MediumRange = dbLoc.MediumRange.Value,
                            LongRange = dbLoc.LongRange.Value,
                            MinimumRange = dbLoc.MinimumRange.Value,
                            CriticalSlots = dbLoc.CriticalSlots.Value
                        };
                    }
                    else
                    {
                        locations[dbLoc.LocationIndex][dbLoc.SlotIndex] = new SimpleComponent
                        {
                            Name = dbLoc.Name,
                            IsDestroyed = dbLoc.IsDestroyed
                        };
                    }
                }
                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading MechLocations for MechId {parameters}", mechId);
                // Return empty structure on error
                int[] slotCounts = [6, 12, 12, 12, 12, 12, 6, 6];
                var locations = new Critical[8][];
                for (int i = 0; i < 8; i++)
                    locations[i] = new Critical[slotCounts[i]];
                return locations;
            }
        }

        // CRUD for Mech
        public async Task<Mech[]> GetMechsAsync(string filter = "")
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    return await _database.Table<Mech>().ToArrayAsync();
                }

                var mechs = await _database.Table<Mech>().Where(x => x.Name != null && x.Name.Contains(filter)).ToArrayAsync();

                foreach (var mech in mechs)
                {
                    mech.Locations = await LoadMechLocationsAsync(mech.Id);
                }
                return mechs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting mechs with filter {parameters}", filter);
                return [];
            }
        }

        public async Task<Mech[]> GetMechsByFullNameAsync(string nameFilter, string chassisFilter, string modelFilter)
        {
            try
            {
                if (string.IsNullOrEmpty(nameFilter) || string.IsNullOrEmpty(chassisFilter) || string.IsNullOrEmpty(modelFilter))
                {
                    return await _database.Table<Mech>().ToArrayAsync();
                }

                var mechs = await _database.Table<Mech>().Where(x => (x.Name != null && x.Name.Contains(nameFilter)) 
                && (x.Chassis!=null && x.Chassis.Contains(chassisFilter))
                && (x.Model!=null && x.Model.Contains(modelFilter))).ToArrayAsync();

                foreach (var mech in mechs)
                {
                    mech.Locations = await LoadMechLocationsAsync(mech.Id);
                }
                return mechs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting mechs with filter {parameters}", $"NameFilter: {nameFilter}, ChassisFilter: {chassisFilter}, ModelFilter:{modelFilter}");
                return [];
            }
        }

        public async Task<Mech?> GetMechAsync(int id)
        {
            try
            {
                var mech = await _database.Table<Mech>().Where(m => m.Id == id).FirstOrDefaultAsync();
                if (mech != null)
                {
                    mech.Locations = await LoadMechLocationsAsync(mech.Id);
                }
                return mech;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting mech with id {parameters}", id);
                return null;
            }
        }

        public async Task<int> SaveMechAsync(Mech mech)
        {
            try
            {
                int result;
                if (mech.Id != 0)
                    result = await _database.UpdateAsync(mech);
                else
                    result = await _database.InsertAsync(mech);
                await SaveMechLocationsAsync(mech.Id, mech.Locations);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving mech {parameters}", JsonSerializer.Serialize(mech));
                return 0;
            }
        }

        public async Task<int> DeleteMechAsync(Mech mech)
        {
            try
            {
                await _database.Table<MechCriticalLocation>().Where(x => x.MechId == mech.Id).DeleteAsync();
                return await _database.DeleteAsync(mech);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting mech {parameters}", JsonSerializer.Serialize(mech));
                return 0;
            }
        }

        // CRUD for Weapon
        public async Task<Weapon[]> GetWeaponsAsync(string filter = "")
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                    return await _database.Table<Weapon>().ToArrayAsync();
                else
                    return await _database.Table<Weapon>().Where(x => x.Name != null && x.Name.Contains(filter)).ToArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weapons {parameters}", filter);
                return [];
            }
        }

        public async Task<Weapon?> GetWeaponAsync(int id)
        {
            try
            {
                return await _database.Table<Weapon>().Where(w => w.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weapon with id {parameters}", id);
                return null;
            }
        }

        public async Task<int> SaveWeaponAsync(Weapon weapon)
        {
            try
            {
                if (weapon.Id != 0)
                    return await _database.UpdateAsync(weapon);
                else
                    return await _database.InsertAsync(weapon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving weapon {parameters}", JsonSerializer.Serialize(weapon));
                return 0;
            }
        }

        public async Task<int> DeleteWeaponAsync(Weapon weapon)
        {
            try
            {
                return await _database.DeleteAsync(weapon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting weapon {parameters}", JsonSerializer.Serialize(weapon));
                return 0;
            }
        }
    }
}
