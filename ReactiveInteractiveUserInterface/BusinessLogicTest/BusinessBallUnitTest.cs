using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new(dataBallFixture, new DataLayerFixture());
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            newInstance.Step();
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }

        [TestMethod]
        public void BounceOffRightWallTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            dataBallFixture.Velocity = new VectorFixture(5.0, 0.0);
            dataBallFixture.Position = new VectorFixture(572.0, 0.0);
            Ball newInstance = new(dataBallFixture, new DataLayerFixture());

            newInstance.Step();

            Assert.IsTrue(dataBallFixture.Velocity.x < 0, "Po odbiciu od prawej ściany vX powinno być ujemne");
        }

        [TestMethod]
        public void BallCollisionChangesVelocityTest()
        {
            // dwie kule w tej samej pozycji, lecące w przeciwne strony
            DataBallFixture ballA = new DataBallFixture();
            ballA.Position = new VectorFixture(100.0, 100.0);
            ballA.Velocity = new VectorFixture(2.0, 0.0); // leci w prawo

            DataBallFixture ballB = new DataBallFixture();
            ballB.Position = new VectorFixture(120.0, 100.0); 
            ballB.Velocity = new VectorFixture(-2.0, 0.0); // leci w lewo

            DataLayerFixture dataLayer = new DataLayerFixture();
            Ball instanceA = new(ballA, dataLayer);
            Ball instanceB = new(ballB, dataLayer);

            instanceA.ResolveCollisionWith(instanceB);

            Assert.IsTrue(ballA.Velocity.x < 0, "Kula A powinna odbić się w lewo");
            Assert.IsTrue(ballB.Velocity.x > 0, "Kula B powinna odbić się w prawo");
        }

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
        {
            public Data.IVector Velocity { get; set; } = new VectorFixture(0.0, 0.0);
            public Data.IVector Position { get; set; } = new VectorFixture(0.0, 0.0);
            public event EventHandler<Data.IVector>? NewPositionNotification;
            public double Diameter => 25.0;
            public double Mass => 1.0;

            internal void Move(Data.IVector delta)
            {
                Position = new VectorFixture(Position.x + delta.x, Position.y + delta.y);
                NewPositionNotification?.Invoke(this, Position);
            }

            public Data.IVector GetPosition() => Position;

            void Data.IBall.Move(Data.IVector delta)
            {
                Move(delta);
            }
        }

        private class DataLayerFixture : Data.DataAbstractAPI
        {
            public override void Dispose() { }
            public override void MoveAll() { }
            public override void Start(int numberOfBalls, Action<Data.IVector, Data.IBall> upperLayerHandler)
                => throw new NotImplementedException();
            public override Data.IVector CreateVector(double x, double y)
                => new VectorFixture(x, y);
        }

        private class VectorFixture : Data.IVector
        {
            internal VectorFixture(double X, double Y)
            {
                x = X; y = Y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }

        #endregion testing instrumentation
    }
}