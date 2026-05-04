//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

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
        public Ball(Data.IBall ball, Data.DataAbstractAPI dataLayer)
        {
            this.ball = ball;
            this.dataLayer = dataLayer;
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
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

            ball.Velocity = dataLayer.CreateVector(vX, vY);

            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

        #endregion private
    }
}