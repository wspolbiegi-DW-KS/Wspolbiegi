namespace ReferenceApplicationArchitecture.BusinessLogic;

/// <summary>
/// Thread-safe persistence boundary for simulation balls.
/// </summary>
public interface IBallRepository
{
    void ReplaceAll(IReadOnlyList<BallEntity> balls);
    IReadOnlyList<BallEntity> GetAll();
}