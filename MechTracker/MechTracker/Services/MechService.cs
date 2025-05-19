using MechTracker.Models;
using System.Collections.ObjectModel;

namespace MechTracker.Services
{
    public class MechService
    {
        private readonly ObservableCollection<Mech> _loadedMechs = [];
        public IReadOnlyCollection<Mech> LoadedMechs => _loadedMechs;

        public virtual int AddMech(Mech mech)
        {
            if(mech.InstanceId == 0)
            {
                mech.InstanceId = _loadedMechs.Count + 1;
            }

            if (!_loadedMechs.Any(m => m.InstanceId == mech.InstanceId))
                _loadedMechs.Add(mech);

            return mech.InstanceId;
        }

        public virtual void RemoveMech(Mech mech)
        {
            _loadedMechs.Remove(mech);
        }

        public virtual void ClearMechs()
        {
            _loadedMechs.Clear();
        }

        public virtual Mech? GetMechById(int id)
        {
            return _loadedMechs.FirstOrDefault(m => m.InstanceId == id);
        }

        public IReadOnlyCollection<Mech> GetAllMechs()
        {
            return LoadedMechs;
        }
    }
}