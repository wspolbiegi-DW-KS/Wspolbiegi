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
        private const double BoardWidth = 600.0 - 25.0; //rozmiar - minus średnica kuli
        private const double BoardHeight = 420.0 - 25.0;
        private readonly Data.DataAbstractAPI dataLayer;
        private readonly Data.IBall ball;
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
            //NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
            double velX = (ball.Velocity.x);
            double velY = (ball.Velocity.y);

            if (e.x + velX > BoardWidth)
            {
                velX = Math.Abs(velX) * -1;
            }
            else if (e.x + velX < 0)
            {
                velX = Math.Abs(velX);
            }

            if (e.y + velY > BoardHeight)
            {
                velY = Math.Abs(velY) * -1;
            }
            else if (e.y + velY < 0)
            {
                velY = Math.Abs(velY);
            }

            ball.Velocity = dataLayer.CreateVector(velX, velY);

            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

        #endregion private
    }
}