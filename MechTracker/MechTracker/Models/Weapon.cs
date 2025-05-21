using System;
using SQLite;

namespace MechTracker.Models
{
    [Table("Weapons")]
    public class Weapon : Critical
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Damage { get; set; }
        public int HeatGenerated { get; set; }
        public int ShortRange { get; set; }
        public int MediumRange { get; set; }
        public int LongRange { get; set; }
        public int MinimumRange { get; set; }
        public int CriticalSlots { get; set; }
    }
}
