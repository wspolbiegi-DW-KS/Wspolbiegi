namespace TP.ConcurrentProgramming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        public static DataAbstractAPI CreateNewDataLayer()
        {
            return new DataImplementation();
        }

        #endregion Layer Factory

        #region public API

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);

        public abstract IVector CreateVector (double x, double y);

        #endregion public API

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

        #endregion private
    }

    public interface IVector
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        double x { get; init; }

        /// <summary>
        /// The y component of the vector.
        /// </summary>
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;

        IVector Velocity { get; set; }
        double Diameter { get; }
        double Mass { get; }
        IVector GetPosition();
        int Id { get; }
        public void Move(IVector delta);
    }


    public interface ILogger
    {
        void Log(string message, DateTime? timestamp = null);
        void Dispose();
    }

}