namespace MechTracker.Constants
{
    public static class MechConstants
    {
        public static readonly string[] ArmorLabels =
        [
            "Head", "Center Torso", "Left Torso", "Right Torso", "Left Arm", "Right Arm", "Left Leg", "Right Leg", "Center Torso (Rear)", "Left Torso (Rear)", "Right Torso (Rear)"
        ];

        public static readonly string[] InternalLabels =
        [
            "Head", "Center Torso", "Left Torso", "Right Torso", "Left Arm", "Right Arm", "Left Leg", "Right Leg"
        ];

        public static readonly string[] DamageLocationFront =
        [
            "Center Torso", "Right Arm", "Right Arm", "Right Leg", "Right Torso", "Center Torso", "Left Torso", "Left Leg", "Left Arm", "Left Arm", "Head"
        ];
        public static readonly string[] DamageLocationRear =
        [
            "Center Torso", "Right Arm", "Right Arm", "Right Leg", "Right Torso (Rear)", "Center Torso (Rear)", "Left Torso (Rear)", "Left Leg", "Left Arm", "Left Arm", "Head"
        ];
        public static readonly string[] DamageLocationRight =
        [
            "Right Torso", "Right Leg", "Right Arm", "Right Arm", "Right Leg", "Right Torso", "Center Torso", "Left Torso", "Left Arm", "Left Leg", "Head"
        ];
        public static readonly string[] DamageLocationLeft =
        [
            "Left Torso", "Left Leg", "Left Arm", "Left Arm", "Left Leg", "Left Torso", "Center Torso", "Right Torso", "Right Arm", "Right Leg", "Head"
        ];

        public static int[] GetInternalStructureByWeight(int weight)
        {
            return weight switch
            {
                20 => [3, 6, 5, 5, 3, 3, 4, 4],
                25 => [3, 8, 6, 6, 4, 4, 6, 6],
                30 => [3, 10, 7, 7, 5, 5, 7, 7],
                35 => [3, 11, 8, 8, 6, 6, 8, 8],
                40 => [3, 12, 10, 10, 6, 6, 10, 10],
                45 => [3, 14, 11, 11, 7, 7, 11, 11],
                50 => [3, 16, 12, 12, 8, 8, 12, 12],
                55 => [3, 18, 13, 13, 9, 9, 13, 13],
                60 => [3, 20, 14, 14, 10, 10, 14, 14],
                65 => [3, 21, 15, 15, 10, 10, 15, 15],
                70 => [3, 22, 15, 15, 11, 11, 15, 15],
                75 => [3, 23, 16, 16, 12, 12, 16, 16],
                80 => [3, 25, 17, 17, 13, 13, 17, 17],
                85 => [3, 27, 18, 18, 14, 14, 18, 18],
                90 => [3, 29, 19, 19, 15, 15, 19, 19],
                95 => [3, 30, 20, 20, 16, 16, 20, 20],
                100 => [3, 31, 21, 21, 17, 17, 21, 21],
                _ => [],
            };
        }

        public static string GetDatabasePath()
        {
            string dbName = "mechtracker.db3";
            string targetPath = Path.Combine(FileSystem.AppDataDirectory, dbName);

            if (!File.Exists(targetPath))
            {
                // Adjust the path if you placed the file elsewhere in your project
                using var stream = FileSystem.OpenAppPackageFileAsync($"Resources/Database/{dbName}").Result;
                using var fileStream = File.Create(targetPath);
                stream.CopyTo(fileStream);
            }

            return targetPath;
        }
    }
}
