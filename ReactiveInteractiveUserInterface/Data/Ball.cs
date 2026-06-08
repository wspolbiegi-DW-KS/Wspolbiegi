using System.Security.Cryptography;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity, int id)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            Id = id;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        #endregion IBall

        #region private

        private Vector Position;
        private readonly object _lock = new object();
        private DateTime _lastUpdateTime = DateTime.Now;

        internal const double BallDiameter = 25.0;  

        public double Diameter => BallDiameter;
        internal const double BallMass = 1.0; 
        public double Mass => BallMass;
        public int Id { get; }

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        public void Move(IVector delta)
        {
            DateTime currentTime = DateTime.Now;
            double deltaTime = (currentTime - _lastUpdateTime).TotalMilliseconds / 1000.0; // Convert to seconds
            _lastUpdateTime = currentTime;

            lock (_lock)
            {
                //Position = new Vector(Position.x + delta.x, Position.y + delta.y);
                Position = new Vector(
                    Position.x + delta.x * deltaTime * 100,
                    Position.y + delta.y * deltaTime * 100
                );
            }
            RaiseNewPositionChangeNotification();
        }

        public IVector GetPosition() {
            lock (_lock)
            {
                return Position;
            }
        }

        #endregion private
    }
}