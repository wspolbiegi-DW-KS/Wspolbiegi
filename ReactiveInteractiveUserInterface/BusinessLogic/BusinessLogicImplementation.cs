using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.CreateNewDataLayer() : underneathLayer;
            //MoveTimer = new Timer(_ => layerBellow.MoveAll(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(30));
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            //MoveTimer.Dispose();
            layerBellow.Dispose();
            Disposed = true;
        }

        List<Ball> balls = new();

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
            //upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall, layerBellow)));
            {
                //Ball newBall = new Ball(databall, layerBellow, balls); // przekaż listę
                Ball newBall = new Ball(databall, layerBellow, balls);
                balls.Add(newBall);
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), newBall);
            });
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;
        private readonly Timer MoveTimer;
        private static readonly object CollisionLock = new();

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}