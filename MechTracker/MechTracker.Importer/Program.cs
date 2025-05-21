using MechTracker.Constants;
using MechTracker.Data;
using MechTracker.Models;
using Microsoft.Extensions.Logging;
using SQLite;
using System.Text;

string basePath = "c:\\temp\\3039u\\";
string dbPath = Path.Combine("..", "..", "..", "..", "MechTracker", "Resources", "Database", "mechtracker.db3");
ILogger<MechRepository> logger = new LoggerFactory().CreateLogger<MechRepository>();
SQLiteAsyncConnection connection = new(dbPath, SQLiteOpenFlags.ReadWrite);
MechRepository repository = new(connection, logger);

foreach (var file in Directory.GetFiles(basePath, "*.MTF"))
{
    string[] lines = File.ReadAllLines(file, Encoding.ASCII);
    if (lines.Any(x => x.Contains("Config:Quad", StringComparison.InvariantCultureIgnoreCase)))
    {
        continue;
    }
    // Do something with 'text'
    Mech mech = new()
    {
        Chassis = lines[1],
        Model = lines[2].Split(' ')[0],
        Name = lines[2].Split(' ').Length > 1 ? lines[2].Split(' ')[1] : ""
    };
    var existing = repository.GetMechsByFullNameAsync(mech.Name, mech.Chassis, mech.Model).Result;
    if (existing != null && existing.Length>0)
    {
        mech.Id = existing[0].Id;
    }
    mech.Weight = int.Parse(lines.Where(x => x.StartsWith("Mass", StringComparison.InvariantCultureIgnoreCase)).First().Split(':')[1]);
    mech.WalkingSpeed = int.Parse(lines.Where(x => x.StartsWith("Walk MP", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    mech.RunningSpeed = (int)Math.Ceiling(mech.WalkingSpeed * 1.5);
    mech.JumpingSpeed = int.Parse(lines.Where(x => x.StartsWith("Jump MP", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    mech.HeatSinks = int.Parse(lines.Where(x => x.StartsWith("Heat Sinks", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1].Split(' ')[0]);
    // get armor
    //"Head", "Center Torso", "Left Torso", "Right Torso", "Left Arm", "Right Arm", "Left Leg", "Right Leg", "Center Torso (Rear)", "Left Torso (Rear)", "Right Torso (Rear)"
    int head = int.Parse(lines.Where(x => x.StartsWith("HD Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int centerTorso = int.Parse(lines.Where(x => x.StartsWith("CT Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int leftTorso = int.Parse(lines.Where(x => x.StartsWith("LT Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int rightTorso = int.Parse(lines.Where(x => x.StartsWith("RT Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int leftArm = int.Parse(lines.Where(x => x.StartsWith("LA Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int rightArm = int.Parse(lines.Where(x => x.StartsWith("RA Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int leftLeg = int.Parse(lines.Where(x => x.StartsWith("LL Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int rightLeg = int.Parse(lines.Where(x => x.StartsWith("RL Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int centerTorsoRear = int.Parse(lines.Where(x => x.StartsWith("RTC Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int leftTorsoRear = int.Parse(lines.Where(x => x.StartsWith("RTL Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int rightTorsoRear = int.Parse(lines.Where(x => x.StartsWith("RTR Armor", StringComparison.InvariantCultureIgnoreCase)).First().Split(":")[1]);
    int[] armor = [head, centerTorso, leftTorso, rightTorso, leftArm, rightArm, leftLeg, rightLeg, centerTorsoRear, leftTorsoRear, rightTorsoRear];
    mech.Armor = armor;
    // calc internals
    mech.Internals = MechConstants.GetInternalStructureByWeight(mech.Weight);
    // get crits
    int critIndex = 0;
    foreach (string location in MechConstants.InternalLabels)
    {
        int lineindex = Array.IndexOf(lines, lines.Where(x => x.StartsWith(location)).First());
        for (int i = 1; i <= 12; i++)
        {
            Critical locCrit;
            string critname = lines[lineindex + i].Trim();
            if (critname.Equals("-Empty-", StringComparison.InvariantCultureIgnoreCase))
            {
                locCrit = new SimpleComponent { Name = "Roll Again", IsDestroyed = true };
            }
            else
            {
                // look up weapon
                Weapon[] weapons = repository.GetWeaponsAsync(critname).Result;
                if (weapons != null && weapons.Length > 0)
                {
                    locCrit = weapons[0];
                }
                else
                {
                    locCrit = new SimpleComponent { Name = critname, IsDestroyed = false };
                }
            }
            mech.Locations[critIndex][i - 1] = locCrit;
            if (location.Equals("Head", StringComparison.InvariantCultureIgnoreCase) || location.EndsWith("Leg", StringComparison.InvariantCultureIgnoreCase) && i >= 5)
            {
                break;
            }
        }
        critIndex++;
    }

    repository.SaveMechAsync(mech).Wait();

    Console.WriteLine($"Read {file}: {lines.Length} lines");
}
