namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        #endregion IBall

        #region private

        private Vector Position;
        private readonly object _lock = new object();
        internal const double BallDiameter = 25.0;  

        public double Diameter => BallDiameter;
        internal const double BallMass = 1.0; 
        public double Mass => BallMass;

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        public void Move(IVector delta)
        {
            lock (_lock)
            {
                Position = new Vector(Position.x + delta.x, Position.y + delta.y);
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