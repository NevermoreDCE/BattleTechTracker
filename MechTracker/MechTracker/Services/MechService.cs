using MechTracker.Models;
using System.Collections.ObjectModel;

namespace MechTracker.Services
{
    public class MechService
    {
        private readonly ObservableCollection<Mech> _loadedMechs = new();
        public IReadOnlyCollection<Mech> LoadedMechs => _loadedMechs;

        public void AddMech(Mech mech)
        {
            if (!_loadedMechs.Any(m => m.Id == mech.Id))
                _loadedMechs.Add(mech);
        }

        public void RemoveMech(Mech mech)
        {
            _loadedMechs.Remove(mech);
        }

        public void ClearMechs()
        {
            _loadedMechs.Clear();
        }

        public Mech? GetMechById(int id)
        {
            return _loadedMechs.FirstOrDefault(m => m.Id == id);
        }
    }
}