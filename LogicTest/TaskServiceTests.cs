namespace ReferenceApplicationArchitecture.LogicTest;

using FluentAssertions;
using Xunit;
using ReferenceApplicationArchitecture.Logic;
using ReferenceApplicationArchitecture.Data;

public class BilliardServiceTests
{
    [Fact]
    public void Initialize_should_create_requested_ball_count()
    {
        var repository = new InMemoryBallRepository();
        var service = new BilliardService(repository, randomSeed: 1);

        service.Initialize(8, 400, 250);
        var balls = service.GetBalls();

        balls.Should().HaveCount(8);
        balls.Should().OnlyContain(b => b.X >= b.Radius && b.Y >= b.Radius);
    }

    [Fact]
    public void Step_should_move_balls()
    {
        var repository = new InMemoryBallRepository();
        var service = new BilliardService(repository, randomSeed: 2);
        service.Initialize(2, 300, 200);
        var before = service.GetBalls().Select(b => (b.X, b.Y)).ToArray();

        service.Step(0.2);
        var after = service.GetBalls().Select(b => (b.X, b.Y)).ToArray();

        after.Should().NotBeEquivalentTo(before);
    }

    [Fact]
    public void Step_should_swap_horizontal_velocities_after_head_on_collision()
    {
        var repository = new InMemoryBallRepository();
        repository.ReplaceAll(new[]
        {
            new BallEntity(Guid.NewGuid(), 100, 100, 50, 0, 10),
            new BallEntity(Guid.NewGuid(), 119, 100, -50, 0, 10)
        });
        var service = new BilliardService(repository);
        service.Initialize(2, 300, 200);
        repository.ReplaceAll(new[]
        {
            new BallEntity(Guid.NewGuid(), 100, 100, 50, 0, 10),
            new BallEntity(Guid.NewGuid(), 119, 100, -50, 0, 10)
        });

        service.Step(0.01);
        var balls = service.GetBalls();

        balls[0].Vx.Should().BeLessThan(0);
        balls[1].Vx.Should().BeGreaterThan(0);
    }
}
