using System.ComponentModel;
using System.Numerics;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        private const double BoardWidth = 600.0; 
        private const double BoardHeight = 420.0;
        private readonly Data.DataAbstractAPI dataLayer;
        private readonly Data.IBall ball;

        public double Diameter => ball.Diameter;
        public double Mass => ball.Mass;
        internal Ball(Data.IBall ball, Data.DataAbstractAPI dataLayer)
        {
            this.ball = ball;
            this.dataLayer = dataLayer;
            //ball.NewPositionNotification += RaisePositionChangeEvent;

            //moveTimer = new Timer(_ => Move(), null,
            //TimeSpan.FromMilliseconds(Random.Shared.Next(0, 30)), // losowy start żeby nie startowały razem
            //TimeSpan.FromMilliseconds(30));
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        /*private void Move()
        {
            lock (moveLock)
            {
                Data.IVector pos = ball.GetPosition();
                //double vX = ball.Velocity.x;
                //double vY = ball.Velocity.y;

                //ball.Move(dataLayer.CreateVector(vX, vY)); // to wywoła NewPositionNotification
                ball.Move(ball.Velocity);
            }
        }*/

        /*private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            double diameter = ball.Diameter;
            double vX = (ball.Velocity.x);
            double vY = (ball.Velocity.y);

            if ((e.x + vX) > (BoardWidth - diameter) || (e.x + vX) < 0)
            {
                vX = -vX;
            }
            if ((e.y + vY) > (BoardHeight - diameter) || (e.y + vY) < 0)
            {
                vY = -vY;
            }

            (vX, vY) = CheckBallCollisions(e.x, e.y, vX, vY);

            ball.Velocity = dataLayer.CreateVector(vX, vY);
            lastX = e.x; 
            lastY = e.y; 

            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

        private (double vX, double vY) CheckBallCollisions(double x, double y, double vX, double vY)
        {
            lock (collisionLock)
            {
                foreach (Ball other in allBalls)
                {
                    if (other == this) continue;

                    double otherX = other.lastX;
                    double otherY = other.lastY;

                    double dx = (x + Diameter / 2) - (otherX + other.Diameter / 2);
                    double dy = (y + Diameter / 2) - (otherY + other.Diameter / 2);
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    if (distance < (Diameter / 2 + other.Diameter / 2) && distance > 0)
                    {
                        // normalna kolizji
                        double nx = dx / distance;
                        double ny = dy / distance;

                        // prędkość względna
                        double dvX = vX - other.ball.Velocity.x;
                        double dvY = vY - other.ball.Velocity.y;

                        double dot = dvX * nx + dvY * ny;

                        // odbij tylko jeśli zbliżają się do siebie
                        if (dot < 0)
                        {
                            vX -= dot * nx;
                            vY -= dot * ny;

                            // odbij drugą kulę (zakładamy równe masy)
                            double otherVX = other.ball.Velocity.x + dot * nx;
                            double otherVY = other.ball.Velocity.y + dot * ny;
                            other.ball.Velocity = dataLayer.CreateVector(otherVX, otherVY);
                        }
                    }
                }

                return (vX, vY);
            }
        }*/

        private double lastX;
        private double lastY;

        internal void Step()
        {
            Data.IVector pos = ball.GetPosition();
            double vX = ball.Velocity.x;
            double vY = ball.Velocity.y;

            // odbicie od ścian
            double nextX = pos.x + vX;
            double nextY = pos.y + vY;

            if (nextX > BoardWidth - Diameter || nextX < 0)
                vX = -vX;
            if (nextY > BoardHeight - Diameter || nextY < 0)
                vY = -vY;

            ball.Velocity = dataLayer.CreateVector(vX, vY);
            ball.Move(ball.Velocity);

            NewPositionNotification?.Invoke(this,
                new Position(ball.GetPosition().x, ball.GetPosition().y));
        }

        // wywołane przez timer przed Step — kolizja z inną kulą
        internal void ResolveCollisionWith(Ball other)
        {
            Data.IVector posA = ball.GetPosition();
            Data.IVector posB = other.ball.GetPosition();

            double dx = (posA.x + Diameter / 2) - (posB.x + other.Diameter / 2);
            double dy = (posA.y + Diameter / 2) - (posB.y + other.Diameter / 2);
            double distance = Math.Sqrt(dx * dx + dy * dy);
            double minDist = Diameter / 2 + other.Diameter / 2;

            if (distance < minDist && distance > 0)
            {
                double nx = dx / distance;
                double ny = dy / distance;

                double dvX = ball.Velocity.x - other.ball.Velocity.x;
                double dvY = ball.Velocity.y - other.ball.Velocity.y;
                double dot = dvX * nx + dvY * ny;

                if (dot < 0) // zbliżają się do siebie
                {
                    // wzór na sprężystą kolizję z masą
                    double p = 2 * dot / (Mass + other.Mass);

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