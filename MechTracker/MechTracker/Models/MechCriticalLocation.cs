using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechTracker.Models
{
    public class MechCriticalLocation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MechId { get; set; }
        public int LocationIndex { get; set; }
        public int SlotIndex { get; set; }
        public string? Name { get; set; }
        public bool IsDestroyed { get; set; }
        // Weapon properties
        public int? Damage { get; set; }
        public int? HeatGenerated { get; set; }
        public int? ShortRange { get; set; }
        public int? MediumRange { get; set; }
        public int? LongRange { get; set; }
        public int? MinimumRange { get; set; }
    }
}
