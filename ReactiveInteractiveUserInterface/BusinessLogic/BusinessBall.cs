using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        private const double BoardWidth = 600.0; 
        private const double BoardHeight = 420.0;
        private readonly Data.DataAbstractAPI dataLayer;
        private readonly Data.IBall ball;
        private readonly List<Ball> _allBalls;
        private readonly object _collisionLock;

        private Thread _thread;
        private volatile bool _running = false;

        public double Diameter => ball.Diameter;
        public double Mass => ball.Mass;
        internal Ball(Data.IBall ball, Data.DataAbstractAPI dataLayer, List<Ball> allBalls, object collisionLock)
        {
            this.ball = ball;
            this.dataLayer = dataLayer;
            _allBalls = allBalls;
            _collisionLock = collisionLock;
            _thread = new Thread(Run) { IsBackground = true };
        }

        internal void Start()
        {
            _running = true;
            _thread.Start();
        }

        internal void Stop()
        {
            _running = false;
        }

        private void Run()
        {
            while (_running)
            {
                Thread.Sleep(16);

                lock (_collisionLock)
                {
                    foreach (var other in _allBalls)
                    {
                        if (!ReferenceEquals(this, other))
                            ResolveCollisionWith(other);
                    }
                }
                Step();
            }
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        internal void Step()
        {
            Data.IVector pos = ball.GetPosition();
            double vX = ball.Velocity.x;
            double vY = ball.Velocity.y;

            double nextX = pos.x + vX;
            double nextY = pos.y + vY;

            //odbijanie od ścian
            if (nextX > BoardWidth - Diameter || nextX < 0)
                vX = -vX;
            if (nextY > BoardHeight - Diameter || nextY < 0)
                vY = -vY;

            ball.Velocity = dataLayer.CreateVector(vX, vY);
            ball.Move(ball.Velocity);

            NewPositionNotification?.Invoke(this, new Position(ball.GetPosition().x, ball.GetPosition().y));
        }

        // wywołane przez timer przed Step — kolizja z inną kulą
        internal void ResolveCollisionWith(Ball other)
        {
            Data.IVector posA = ball.GetPosition();
            Data.IVector posB = other.ball.GetPosition();

            double dx = (posA.x + Diameter / 2) - (posB.x + other.Diameter / 2);
            double dy = (posA.y + Diameter / 2) - (posB.y + other.Diameter / 2);
            //odległość między środkami kul
            double distance = Math.Sqrt(dx * dx + dy * dy);
            //double minDist = Diameter / 2 + other.Diameter / 2;
            double minDist = Diameter; //obie kula mają ten sam rozmiar, więc można uprościć

            if (distance < minDist)
            {
                double nx = dx / distance;
                double ny = dy / distance;

                double dvX = ball.Velocity.x - other.ball.Velocity.x;
                double dvY = ball.Velocity.y - other.ball.Velocity.y;
                double dot = dvX * nx + dvY * ny;

                if (dot < 0) 
                {
                    // wzór na sprężystą kolizję z masą
                    double p = 2 * (dot) / (Mass + other.Mass);

                    ball.Velocity = dataLayer.CreateVector(
                        ball.Velocity.x - p * other.Mass * nx,
                        ball.Velocity.y - p * other.Mass * ny);

                    other.ball.Velocity = dataLayer.CreateVector(
                        other.ball.Velocity.x + p * Mass * nx,
                        other.ball.Velocity.y + p * Mass * ny);
                }
            }
        }
        #endregion private
    }
}