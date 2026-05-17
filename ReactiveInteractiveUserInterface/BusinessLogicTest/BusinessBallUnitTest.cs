namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new(dataBallFixture, Data.DataAbstractAPI.GetDataLayer());
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move(new VectorFixture(0.0, 0.0));
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }

        //
        [TestMethod]
        public void BounceOffRightWallTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            dataBallFixture.Velocity = new VectorFixture(5.0, 0.0); // leci w prawo
            Ball newInstance = new(dataBallFixture, new DataLayerFixture());

            dataBallFixture.Move(new VectorFixture(572.0, 0.0));

            Assert.IsTrue(dataBallFixture.Velocity.x < 0, "Po odbiciu od prawej ściany vX powinno być ujemne");
        }
        //

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
        {
            //public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public Data.IVector Velocity { get; set; } = new VectorFixture(0.0, 0.0);
            public event EventHandler<Data.IVector>? NewPositionNotification;
            public double Diameter => 25.0;

            internal void Move(VectorFixture position)
            {
                //NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
                NewPositionNotification?.Invoke(this, position);
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