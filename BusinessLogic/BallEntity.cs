namespace ReferenceApplicationArchitecture.BusinessLogic;

/// <summary>
/// Ball state used by the simulation domain.
/// </summary>
public sealed class BallEntity
{
    public BallEntity(Guid id, double x, double y, double vx, double vy, double radius)
    {
        Id = id;
        X = x;
        Y = y;
        Vx = vx;
        Vy = vy;
        Radius = radius;
    }

    public Guid Id { get; }

    public double X { get; }

    public double Y { get; }

    public double Vx { get; }

    public double Vy { get; }

    public double Radius { get; }

    public BallEntity With(double? x = null, double? y = null, double? vx = null, double? vy = null)
    {
        return new BallEntity(Id, x ?? X, y ?? Y, vx ?? Vx, vy ?? Vy, Radius);
    }

    public BallEntity Clone()
    {
        return new BallEntity(Id, X, Y, Vx, Vy, Radius);
    }
}