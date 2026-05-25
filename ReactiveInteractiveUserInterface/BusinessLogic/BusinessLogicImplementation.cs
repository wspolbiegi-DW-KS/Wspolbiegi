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
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            lock (balls) { foreach (var b in balls) b.Stop(); }
            layerBellow.Dispose();
            Disposed = true;
        }

        

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
            {
                Ball newBall = new Ball(databall, layerBellow);
                lock (balls)
                {
                    balls.Add(newBall);
                }
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), newBall);
                newBall.Initialize(balls, _collisionLock);
                newBall.Start();
            });
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;
        private List<Ball> balls = new();
        private readonly UnderneathLayerAPI layerBellow;
        private readonly object _collisionLock = new();

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