using MechTracker.Views;

namespace MechTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(CreateMechPage), typeof(CreateMechPage));
            Routing.RegisterRoute(nameof(SetArmorPage), typeof(SetArmorPage));
            Routing.RegisterRoute(nameof(SetInternalsPage), typeof(SetInternalsPage));
            Routing.RegisterRoute(nameof(WeaponDamageInputPage), typeof(WeaponDamageInputPage));
            Routing.RegisterRoute(nameof(SelectPhasePage), typeof(SelectPhasePage));
            Routing.RegisterRoute(nameof(SelectActiveMechPage), typeof(SelectActiveMechPage));
            Routing.RegisterRoute(nameof(LoadMechPage), typeof(LoadMechPage));
        }
    }
}
