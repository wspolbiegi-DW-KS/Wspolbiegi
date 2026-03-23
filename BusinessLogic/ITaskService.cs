namespace ReferenceApplicationArchitecture.BusinessLogic;

using ReferenceApplicationArchitecture.Data;

/// <summary>
/// Concurrent simulation operations for balls on a rectangular table.
/// </summary>
public interface IBilliardService
{
    void Initialize(int ballCount, double tableWidth, double tableHeight);
    void Step(double deltaTimeSeconds);
    IReadOnlyList<BallEntity> GetBalls();
}
