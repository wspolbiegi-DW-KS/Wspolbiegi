namespace ReferenceApplicationArchitecture.PresentationModelTest;

using FluentAssertions;
using Xunit;
using ReferenceApplicationArchitecture.PresentationModel;

public class BallViewDataTests
{
    [Fact]
    public void Constructor_should_assign_all_properties()
    {
        var id = Guid.NewGuid();
        var viewData = new BallViewData(id, 12.5, 30.2, 10);

        viewData.Id.Should().Be(id);
        viewData.X.Should().Be(12.5);
        viewData.Y.Should().Be(30.2);
        viewData.Radius.Should().Be(10);
    }
}
