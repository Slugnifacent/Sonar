using System;
using System.Collections.Generic;
using System.Linq;


namespace Sonar
{
    public abstract class Object
    {
        public GameVector2 position;
        protected GameVector2 rootPos;
        public float speed;
        public float baseSpeed;
        public GameVector2 positionPrevious;
        public GameVector2 positionFuture;
        public GameVector2 orientation;
        protected GameTexture texture;
        protected GameRectangle boundingBox;
        protected GameColor color;
        protected int spriteWidth, spriteHeight, spriteRowSize, spriteColNum;
        protected GameVector2 currSprite;

        abstract public void Update(object gameTime);

        virtual public void Draw(object spriteBatch)
        {
            
        }

        [Obsolete("Obsolete for Unity Port")]
        public virtual void updateBoundingBox(GameVector2 position)
        {
            //if (texture != null)
            //{
            //    boundingBox.X = (int)position.X - spriteWidth / 4 - 1;
            //    boundingBox.Y = (int)position.Y - spriteHeight / 4 - 1;
            //}
        }

        [Obsolete("Obsolete for Unity Port")]
        public void updateBoundingBox(int width, int height)
        {
            //boundingBox.Width = width;
            //boundingBox.Height = height;
        }

        public GameVector2 getPrevPos()
        {
            return positionPrevious;
        }
        public GameVector2 getCurrPos()
        {
            return position;
        }

        public GameRectangle getBox()
        {
            return boundingBox;
        }
    }
}
