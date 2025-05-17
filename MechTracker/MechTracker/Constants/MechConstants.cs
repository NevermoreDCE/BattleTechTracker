namespace MechTracker.Constants
{
    public static class MechConstants
    {
        public static readonly string[] ArmorLabels = new[]
        {
            "Head", "Center Torso", "Left Torso", "Right Torso", "Left Arm", "Right Arm", "Left Leg", "Right Leg", "Center Torso (Rear)", "Left Torso (Rear)", "Right Torso (Rear)"
        };

        public static readonly string[] InternalLabels = new[]
        {
            "Head", "Center Torso", "Left Torso", "Right Torso", "Left Arm", "Right Arm", "Left Leg", "Right Leg"
        };

        public static readonly string[] DamageLocationFront = new[]
        {
            "Center Torso", "Right Arm", "Right Arm", "Right Leg", "Right Torso", "Center Torso", "Left Torso", "Left Leg", "Left Arm", "Left Arm", "Head"
        };
        public static readonly string[] DamageLocationRear = new[]
        {
            "Center Torso", "Right Arm", "Right Arm", "Right Leg", "Right Torso (Rear)", "Center Torso (Rear)", "Left Torso (Rear)", "Left Leg", "Left Arm", "Left Arm", "Head"
        };
        public static readonly string[] DamageLocationRight = new[]
        {
            "Right Torso", "Right Leg", "Right Arm", "Right Arm", "Right Leg", "Right Torso", "Center Torso", "Left Torso", "Left Arm", "Left Leg", "Head"
        };
        public static readonly string[] DamageLocationLeft = new[]
        {
            "Left Torso", "Left Leg", "Left Arm", "Left Arm", "Left Leg", "Left Torso", "Center Torso", "Right Torso", "Right Arm", "Right Leg", "Head"
        };
    }
}
