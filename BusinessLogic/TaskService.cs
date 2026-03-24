namespace ReferenceApplicationArchitecture.BusinessLogic;

/// <summary>
/// Encapsulates concurrent simulation rules for billiard balls.
/// </summary>
public class BilliardService : IBilliardService
{
    private readonly IBallRepository _repository;
    private readonly Random _random;
    private readonly object _sync = new();
    private double _tableWidth;
    private double _tableHeight;
    private const double Radius = 10;
    private const double MaxSpeed = 140;

    public BilliardService(IBallRepository repository, int randomSeed = 42)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _random = new Random(randomSeed);
    }

    public void Initialize(int ballCount, double tableWidth, double tableHeight)
    {
        if (ballCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ballCount), "Ball count must be positive.");
        }

        if (tableWidth <= 2 * Radius || tableHeight <= 2 * Radius)
        {
            throw new ArgumentException("Table area is too small.");
        }

        var balls = new List<BallEntity>(ballCount);
        var columns = Math.Max(1, (int)((tableWidth - 2 * Radius) / (2.5 * Radius)));
        var rowsNeeded = (int)Math.Ceiling(ballCount / (double)columns);
        var maxRows = Math.Max(1, (int)((tableHeight - 2 * Radius) / (2.5 * Radius)));
        if (rowsNeeded > maxRows)
        {
            throw new ArgumentException("Too many balls for the selected table size.");
        }

        var spacingX = (tableWidth - 2 * Radius) / (columns + 1);
        var spacingY = (tableHeight - 2 * Radius) / (rowsNeeded + 1);

        for (var i = 0; i < ballCount; i++)
        {
            var col = i % columns;
            var row = i / columns;
            var x = Radius + (col + 1) * spacingX;
            var y = Radius + (row + 1) * spacingY;

            var vx = (_random.NextDouble() * 2 - 1) * MaxSpeed;
            var vy = (_random.NextDouble() * 2 - 1) * MaxSpeed;
            if (Math.Abs(vx) < 15)
            {
                vx = 15 * Math.Sign(vx == 0 ? 1 : vx);
            }
            if (Math.Abs(vy) < 15)
            {
                vy = 15 * Math.Sign(vy == 0 ? 1 : vy);
            }

            balls.Add(new BallEntity(Guid.NewGuid(), x, y, vx, vy, Radius));
        }

        lock (_sync)
        {
            _tableWidth = tableWidth;
            _tableHeight = tableHeight;
            _repository.ReplaceAll(balls);
        }
    }

    public void Step(double deltaTimeSeconds)
    {
        if (deltaTimeSeconds <= 0)
        {
            return;
        }

        lock (_sync)
        {
            var current = _repository.GetAll();
            if (current.Count == 0)
            {
                return;
            }

            var moved = new BallEntity[current.Count];
            Parallel.For(0, current.Count, i =>
            {
                var ball = current[i];
                var x = ball.X + ball.Vx * deltaTimeSeconds;
                var y = ball.Y + ball.Vy * deltaTimeSeconds;
                var vx = ball.Vx;
                var vy = ball.Vy;

                if (x - ball.Radius < 0)
                {
                    x = ball.Radius;
                    vx = -vx;
                }
                else if (x + ball.Radius > _tableWidth)
                {
                    x = _tableWidth - ball.Radius;
                    vx = -vx;
                }

                if (y - ball.Radius < 0)
                {
                    y = ball.Radius;
                    vy = -vy;
                }
                else if (y + ball.Radius > _tableHeight)
                {
                    y = _tableHeight - ball.Radius;
                    vy = -vy;
                }

                moved[i] = ball.With(x: x, y: y, vx: vx, vy: vy);
            });

            ResolveBallCollisions(moved);
            _repository.ReplaceAll(moved);
        }
    }

    public IReadOnlyList<BallEntity> GetBalls()
    {
        return _repository.GetAll();
    }

    private static void ResolveBallCollisions(BallEntity[] balls)
    {
        for (var i = 0; i < balls.Length; i++)
        {
            for (var j = i + 1; j < balls.Length; j++)
            {
                var b1 = balls[i];
                var b2 = balls[j];
                var dx = b2.X - b1.X;
                var dy = b2.Y - b1.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy);
                var minDistance = b1.Radius + b2.Radius;

                if (distance <= 0 || distance >= minDistance)
                {
                    continue;
                }

                var nx = dx / distance;
                var ny = dy / distance;
                var tx = -ny;
                var ty = nx;

                var v1n = b1.Vx * nx + b1.Vy * ny;
                var v1t = b1.Vx * tx + b1.Vy * ty;
                var v2n = b2.Vx * nx + b2.Vy * ny;
                var v2t = b2.Vx * tx + b2.Vy * ty;

                var newV1n = v2n;
                var newV2n = v1n;

                var newV1x = tx * v1t + nx * newV1n;
                var newV1y = ty * v1t + ny * newV1n;
                var newV2x = tx * v2t + nx * newV2n;
                var newV2y = ty * v2t + ny * newV2n;

                var overlap = minDistance - distance;
                var shift = overlap / 2.0;
                var newX1 = b1.X - nx * shift;
                var newY1 = b1.Y - ny * shift;
                var newX2 = b2.X + nx * shift;
                var newY2 = b2.Y + ny * shift;

                balls[i] = b1.With(x: newX1, y: newY1, vx: newV1x, vy: newV1y);
                balls[j] = b2.With(x: newX2, y: newY2, vx: newV2x, vy: newV2y);
            }
        }

        for (var i = 0; i < balls.Length; i++)
        {
            var b = balls[i];
            const double damping = 0.999;
            balls[i] = b.With(vx: b.Vx * damping, vy: b.Vy * damping);
        }
    }

}

