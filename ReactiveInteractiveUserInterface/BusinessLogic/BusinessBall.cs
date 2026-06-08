using System.ComponentModel;
using System.Numerics;
using System.Text;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        private const double BoardWidth = 600.0; 
        private const double BoardHeight = 420.0;
        private readonly Data.DataAbstractAPI dataLayer;
        private readonly Data.IBall ball;
        private List<Ball> _allBalls;
        private object _collisionLock;

        private Thread _thread;
        private volatile bool _running = false;

        private Logger _logger;

        private DateTime _lastUpdateTime = DateTime.UtcNow;

        public double Diameter => ball.Diameter;
        public double Mass => ball.Mass;
        internal Ball(Data.IBall ball, Data.DataAbstractAPI dataLayer, Logger logger)
        {
            this.ball = ball;
            this.dataLayer = dataLayer;
            this._logger = logger;
            _logger.Log($"New Ball - ID {ball.Id}, position ({Math.Round(ball.GetPosition().x, 4)}, {Math.Round(ball.GetPosition().y, 4)}), velocity ({Math.Round(ball.Velocity.x, 4)}, {Math.Round(ball.Velocity.y, 4)})");
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
                DateTime now = DateTime.UtcNow;
                double dt = (now - _lastUpdateTime).TotalSeconds;
                _lastUpdateTime = now;
                if (dt > 0.1) dt = 0.1;

                if (_allBalls != null)
                {
                    List<Ball> snapshot;
                    lock (_collisionLock)
                    {
                        snapshot = new List<Ball>(_allBalls);
                    }
                    foreach (var other in snapshot)
                    {
                        if (!ReferenceEquals(this, other) && ball.Id < other.ball.Id)
                            ResolveCollisionWith(other);
                    }
                    
                }
                Step(dt);
                Thread.Sleep(16);
            }
        }

        internal void Initialize(List<Ball> allBalls, object collisionLock)
        {
            _allBalls = allBalls;
            _collisionLock = collisionLock;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        internal void Step(double dt)
        {
            Data.IVector pos = ball.GetPosition();
            double vX = ball.Velocity.x;
            double vY = ball.Velocity.y;
            dt *= 50; //skalujemy dt, aby ruch był bardziej widoczny

            //uwzględnienie upływu czasu przy obliczeniach położenia
            double nextX = pos.x + vX * dt;
            double nextY = pos.y + vY * dt;
            Boolean isColliding = false;

            //odbijanie od ścian
            if (nextX > BoardWidth - Diameter || nextX < 0)
            {
                vX = -vX;
                isColliding = true;
            }
            if (nextY > BoardHeight - Diameter || nextY < 0)
            {
                vY = -vY;
                isColliding = true;
            }
            ball.Velocity = dataLayer.CreateVector(vX, vY);
            ball.Move(
                dataLayer.CreateVector(
                vX*dt,
                vY*dt )
            );

            NewPositionNotification?.Invoke(this, new Position(ball.GetPosition().x, ball.GetPosition().y));
            if (isColliding == true)
            {
                _logger.Log($"Bounce - {ball.Id} position ({Math.Round(ball.GetPosition().x, 4)}, {Math.Round(ball.GetPosition().y, 4)}), new velocity ({Math.Round(ball.Velocity.x, 4)}, {Math.Round(ball.Velocity.y, 4)})");
            }
        }

        internal void ResolveCollisionWith(Ball other)
        {
            Data.IVector posA = ball.GetPosition();
            Data.IVector posB = other.ball.GetPosition();

            double dx = (posA.x + Diameter / 2) - (posB.x + other.Diameter / 2);
            double dy = (posA.y + Diameter / 2) - (posB.y + other.Diameter / 2);
            //odległość między środkami kul
            double distance = Math.Sqrt(dx * dx + dy * dy);
            double minDist = Diameter; 

            if (distance <= minDist)
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
                _logger.Log($"Collision - ball {ball.Id}, ball {other.ball.Id}. New velocities: ball {ball.Id} ({Math.Round(ball.Velocity.x, 4)}, " +
                    $"{Math.Round(ball.Velocity.y, 4)}), ball {other.ball.Id} ({Math.Round(other.ball.Velocity.x, 4)}, {Math.Round(other.ball.Velocity.y, 4)})");

            }
        }
        #endregion private
    }
}