using SQLite;

namespace MechTracker.Models
{
    [Table("Mechs")]
    public class Mech
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Weight { get; set; }
        public int WalkingSpeed { get; set; }
        public int RunningSpeed { get; set; }
        public int JumpingSpeed { get; set; }
        public int Heat { get; set; } = 0;
        public int HeatSinks { get; set; } = 0;
        [Ignore]
        public Critical[][] Locations { get; set; } =
           [
               [
                    new SimpleComponent{Name="Life Support"},
                    new SimpleComponent{Name="Sensors"},
                    new SimpleComponent{Name="Cockpit"},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Sensors"},
                    new SimpleComponent{Name="Life Support"}
                ], // Head
                [
                    new SimpleComponent{Name="Shoulder"},
                    new SimpleComponent{Name="Upper Arm Actuator"},
                    new SimpleComponent{Name="Lower Arm Actuator"},
                    new SimpleComponent{Name="Hand Actuator"},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},

                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                ], // Left Arm
                [
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},

                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                ], // Left Torso
                [
                    new SimpleComponent{Name="Engine"},
                    new SimpleComponent{Name="Engine"},
                    new SimpleComponent{Name="Engine"},
                    new SimpleComponent{Name="Gyro"},
                    new SimpleComponent{Name="Gyro"},
                    new SimpleComponent{Name="Gyro"},
                    new SimpleComponent{Name="Gyro"},
                    new SimpleComponent{Name="Engine"},
                    new SimpleComponent{Name="Engine"},
                    new SimpleComponent{Name="Engine"},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true }
                ], // Center Torso
                [
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},

                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},

                ], // Right Torso
                [
                    new SimpleComponent{Name="Shoulder"},
                    new SimpleComponent{Name="Upper Arm Actuator"},
                    new SimpleComponent{Name="Lower Arm Actuator"},
                    new SimpleComponent{Name="Hand Actuator"},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},

                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                ], // Right Arm
                [
                    new SimpleComponent{Name="Hip"},
                    new SimpleComponent{Name="Upper Leg Actuator"},
                    new SimpleComponent{Name="Lower Leg Actuator"},
                    new SimpleComponent{Name="Foot Actuator"},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true}
                ], // Left Leg
                [
                    new SimpleComponent{Name="Hip"},
                    new SimpleComponent{Name="Upper Leg Actuator"},
                    new SimpleComponent{Name="Lower Leg Actuator"},
                    new SimpleComponent{Name="Foot Actuator"},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true},
                    new SimpleComponent{Name="Roll Again", IsDestroyed=true}
                ]  // Right Leg
           ];

        [Ignore]
        public int[] Armor
        {
            get => ArmorString?.Split(',').Select(int.Parse).ToArray() ?? new int[11];
            set
            {
                ArmorString = string.Join(",", value ?? new int[11]);
                CurrentArmor = [.. (value ?? new int[11])];
            }
        }

        [Ignore]
        public int[] CurrentArmor { get; set; } = new int[11];

        public string? ArmorString { get; set; } = "9,24,20,20,18,18,20,20,7,6,6"; // mapped to DB

        [Ignore]
        public int[] Internals
        {
            get => InternalsString?.Split(',').Select(int.Parse).ToArray() ?? new int[8];
            set
            {
                InternalsString = string.Join(",", value ?? new int[8]);
                CurrentInternals = [.. (value ?? new int[8])];
            }
        }

        [Ignore]
        public int[] CurrentInternals { get; set; } = new int[8];

        public string? InternalsString { get; set; } = "3,18,13,13,9,9,13,13"; // mapped to DB
    }
}
