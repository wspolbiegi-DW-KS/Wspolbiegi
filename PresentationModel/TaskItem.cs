namespace ReferenceApplicationArchitecture.PresentationModel;

/// <summary>
/// Presentation-layer model used by the WPF view to render a ball.
/// </summary>
public sealed class BallViewData
{
    public BallViewData(Guid id, double x, double y, double radius)
    {
        Id = id;
        X = x;
        Y = y;
        Radius = radius;
    }

    public Guid Id { get; }

    public double X { get; }

    public double Y { get; }

    public double Radius { get; }
}
