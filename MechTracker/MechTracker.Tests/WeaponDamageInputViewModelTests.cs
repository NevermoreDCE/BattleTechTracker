using Xunit;
using Moq;
using FluentAssertions;
using MechTracker.ViewModels;
using MechTracker.Models;
using MechTracker.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MechTracker.Tests
{
    public class WeaponDamageInputViewModelTests
    {

        [Fact]
        public void Constructor_ShouldInitializeMech()
        {
            Mech _mech = new Mech { Id = 1, Name = "TestMech", Armor = new int[11], Internals = new int[8] };
            Mock<MechService> _mechServiceMock = new Mock<MechService>();
            _mechServiceMock.Setup(m => m.GetMechById(1)).Returns(_mech);
            Mock<IUserPromptService> _promptMock = new Mock<IUserPromptService>();
            Mock<ILogger<WeaponDamageInputViewModel>> _loggerMock = new Mock<ILogger<WeaponDamageInputViewModel>>();
            var vm = new WeaponDamageInputViewModel(_mechServiceMock.Object, _promptMock.Object, _loggerMock.Object, 1);
            vm.Mech.Should().NotBeNull();
            vm.Mech.Id.Should().Be(1);
        }

        [Fact]
        public void ApplyDamage_ShouldReduceArmor()
        {
            Mech _mech = new Mech { Id = 1, Name = "TestMech", Armor = new int[11], Internals = new int[8] };
            Mock<MechService> _mechServiceMock = new Mock<MechService>();
            _mechServiceMock.Setup(m => m.GetMechById(1)).Returns(_mech);
            Mock<IUserPromptService> _promptMock = new Mock<IUserPromptService>();
            Mock<ILogger<WeaponDamageInputViewModel>> _loggerMock = new Mock<ILogger<WeaponDamageInputViewModel>>();
            var vm = new WeaponDamageInputViewModel(_mechServiceMock.Object, _promptMock.Object, _loggerMock.Object, 1);
            _mech.CurrentArmor[0] = 10;
            var result = vm.ApplyDamage(5, 0, "Head", out int carry);
            _mech.CurrentArmor[0].Should().Be(5);
            carry.Should().Be(0);
            result.Should().Contain("5 damage has been applied to Head");
        }

        [Fact]
        public void ApplyDamage_ShouldCarryOverToInternals()
        {
            Mech _mech = new Mech { Id = 1, Name = "TestMech", Armor = new int[11], Internals = new int[8] };
            Mock<MechService> _mechServiceMock = new Mock<MechService>();
            _mechServiceMock.Setup(m => m.GetMechById(1)).Returns(_mech);
            Mock<IUserPromptService> _promptMock = new Mock<IUserPromptService>();
            Mock<ILogger<WeaponDamageInputViewModel>> _loggerMock = new Mock<ILogger<WeaponDamageInputViewModel>>();
            var vm = new WeaponDamageInputViewModel(_mechServiceMock.Object, _promptMock.Object, _loggerMock.Object, 1);
            _mech.CurrentArmor[0] = 2;
            _mech.CurrentInternals[0] = 3;
            string result = vm.ApplyDamage(5, 0, "Head", out int carry);
            _mech.CurrentArmor[0].Should().Be(0);
            _mech.CurrentInternals[0].Should().Be(0);
            carry.Should().Be(0); // Head destruction ends carryover
            result.Should().Contain("breached the armor");
            result.Should().Contain("destroyed the mech");
        }

        [Fact]
        public void ApplyDamage_ShouldReturnCarryover()
        {
            Mech _mech = new Mech { Id = 1, Name = "TestMech", Armor = new int[11], Internals = new int[8] };
            Mock<MechService> _mechServiceMock = new Mock<MechService>();
            _mechServiceMock.Setup(m => m.GetMechById(1)).Returns(_mech);
            Mock<IUserPromptService> _promptMock = new Mock<IUserPromptService>();
            Mock<ILogger<WeaponDamageInputViewModel>> _loggerMock = new Mock<ILogger<WeaponDamageInputViewModel>>();
            var vm = new WeaponDamageInputViewModel(_mechServiceMock.Object, _promptMock.Object, _loggerMock.Object, 1);
            _mech.CurrentArmor[3] = 0;
            _mech.CurrentInternals[3] = 2;
            var result = vm.ApplyDamage(5, 3, "Right Torso", out int carry);
            carry.Should().Be(3);
            _mech.CurrentInternals[1].Should().Be(0);
            result.Should().Contain("carry over to the next location");
        }

        [Fact]
        public void ApplyDamage_ShouldReturnNoArmorOrInternals()
        {
            Mech _mech = new Mech { Id = 1, Name = "TestMech", Armor = new int[11], Internals = new int[8] };
            Mock<MechService> _mechServiceMock = new Mock<MechService>();
            _mechServiceMock.Setup(m => m.GetMechById(1)).Returns(_mech);
            Mock<IUserPromptService> _promptMock = new Mock<IUserPromptService>();
            Mock<ILogger<WeaponDamageInputViewModel>> _loggerMock = new Mock<ILogger<WeaponDamageInputViewModel>>();
            var vm = new WeaponDamageInputViewModel(_mechServiceMock.Object, _promptMock.Object, _loggerMock.Object, 1);
            _mech.CurrentArmor[2] = 0;
            _mech.CurrentInternals[2] = 0;
            var result = vm.ApplyDamage(4, 2, "Left Torso", out int carry);
            carry.Should().Be(4);
            result.Should().Contain("no armor or internals left");
        }

        [Fact]
        public async Task GetDamageAmount_ShouldReturnParsedValue()
        {
            Mech _mech = new Mech { Id = 1, Name = "TestMech", Armor = new int[11], Internals = new int[8] };
            Mock<MechService> _mechServiceMock = new Mock<MechService>();
            _mechServiceMock.Setup(m => m.GetMechById(1)).Returns(_mech);
            Mock<IUserPromptService> _promptMock = new Mock<IUserPromptService>();
            Mock<ILogger<WeaponDamageInputViewModel>> _loggerMock = new Mock<ILogger<WeaponDamageInputViewModel>>();
            var vm = new WeaponDamageInputViewModel(_mechServiceMock.Object, _promptMock.Object, _loggerMock.Object, 1);
            _promptMock.Setup(p => p.ShowActionSheet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>())).ReturnsAsync("7");
            var result = await vm.GetDamageAmount();
            result.Should().Be(7);
        }

        [Fact]
        public async Task GetDamageAmount_ShouldReturnMinusOneOnCancel()
        {
            Mech _mech = new Mech { Id = 1, Name = "TestMech", Armor = new int[11], Internals = new int[8] };
            Mock<MechService> _mechServiceMock = new Mock<MechService>();
            _mechServiceMock.Setup(m => m.GetMechById(1)).Returns(_mech);
            Mock<IUserPromptService> _promptMock = new Mock<IUserPromptService>();
            Mock<ILogger<WeaponDamageInputViewModel>> _loggerMock = new Mock<ILogger<WeaponDamageInputViewModel>>();
            var vm = new WeaponDamageInputViewModel(_mechServiceMock.Object, _promptMock.Object, _loggerMock.Object, 1);
            _promptMock.Setup(p => p.ShowActionSheet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>())).ReturnsAsync("Cancel");
            var result = await vm.GetDamageAmount();
            result.Should().Be(-1);
        }
    }
}
