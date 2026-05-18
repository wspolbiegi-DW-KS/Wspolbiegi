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
            MoveTimer = new Timer(_ => SimulationStep(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
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
                Ball newBall = new Ball(databall, layerBellow);
                balls.Add(newBall);
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), newBall);
            });
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;
        private List<Ball> balls = new();
        private readonly UnderneathLayerAPI layerBellow;
        private readonly Timer MoveTimer;
        private readonly object collisionLock = new();

        private void SimulationStep()
        {
            // najpierw kolizje między kulami (sekcja krytyczna)
            lock (collisionLock)
            {
                for (int i = 0; i < balls.Count; i++)
                    for (int j = i + 1; j < balls.Count; j++)
                        balls[i].ResolveCollisionWith(balls[j]);
            }

            // potem ruch — współbieżnie
            Parallel.ForEach(balls, ball => ball.Step());
        }

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