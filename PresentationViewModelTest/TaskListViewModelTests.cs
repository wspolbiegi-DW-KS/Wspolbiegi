namespace ReferenceApplicationArchitecture.PresentationViewModelTest;

using FluentAssertions;
using Xunit;
using ReferenceApplicationArchitecture.BusinessLogic;
using ReferenceApplicationArchitecture.Data;
using ReferenceApplicationArchitecture.PresentationViewModel;

public class BilliardTableViewModelTests
{
    [Fact]
    public void TryInitialize_should_fill_ball_collection_when_input_valid()
    {
        var service = new BilliardService(new InMemoryBallRepository(), randomSeed: 12);
        var vm = new BilliardTableViewModel(service);

        vm.TryInitialize(5, 400, 250).Should().BeTrue();
        vm.Balls.Should().HaveCount(5);
        vm.LastError.Should().BeNull();
    }

    [Fact]
    public void TryInitialize_should_capture_error_when_input_invalid()
    {
        var service = new BilliardService(new InMemoryBallRepository(), randomSeed: 1);
        var vm = new BilliardTableViewModel(service);

        vm.TryInitialize(0, 400, 250).Should().BeFalse();
        vm.LastError.Should().NotBeNull();
    }

    [Fact]
    public void Step_should_update_positions_after_initialization()
    {
        var service = new BilliardService(new InMemoryBallRepository(), randomSeed: 3);
        var vm = new BilliardTableViewModel(service);
        vm.TryInitialize(2, 350, 220).Should().BeTrue();
        var before = vm.Balls.Select(b => (b.X, b.Y)).ToArray();

        vm.Step(0.03);
        var after = vm.Balls.Select(b => (b.X, b.Y)).ToArray();

        after.Should().NotBeEquivalentTo(before);
    }
}
