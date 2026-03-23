namespace ReferenceApplicationArchitecture.Data;

/// <summary>
/// Thread-safe in-memory storage of balls.
/// </summary>
public class InMemoryBallRepository : IBallRepository
{
    private readonly object _sync = new();
    private List<BallEntity> _storage = new();

    public void ReplaceAll(IReadOnlyList<BallEntity> balls)
    {
        if (balls is null)
        {
            throw new ArgumentNullException(nameof(balls));
        }

        lock (_sync)
        {
            _storage = balls.Select(b => b.Clone()).ToList();
        }
    }

    public IReadOnlyList<BallEntity> GetAll()
    {
        lock (_sync)
        {
            return _storage.Select(b => b.Clone()).ToList();
        }
    }
}
