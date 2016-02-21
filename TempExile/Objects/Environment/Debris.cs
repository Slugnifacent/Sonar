using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar {
    public class Debris : Object {
        int weight;
        public int soundCD; //So it doesn't just keep going off


        public Debris(GameVector2 Position, int weight, GameTexture sprite) {
            //if (sprite == null) {
            //    sprite = Content.Load<GameTexture>(@"Textures/Objects/Entity/Glass/broken_glass1");
            //}
            //else { texture = sprite; }

            //rootPos = position;
            //position = Position;
            //orientation.Normalize();
            //this.weight = weight;
            //boundingBox = new GameRectangle((int)(position.X - MapUnit.MAX_SIZE / 2 - 1), (int)(position.Y - MapUnit.MAX_SIZE / 2 - 1), (int)(MapUnit.MAX_SIZE), (int)(MapUnit.MAX_SIZE));
            //texture = sprite;
            //soundCD = 0;
        }

        public void setPos(GameVector2 v) {
            //position = v;
            //updateBoundingBox(position);
        }

        /// <summary>
        /// Corey - 2/6
        /// Collision code taken from the spectre class. 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Collide(GameRectangle rect) {
            //if ((((rect.Bottom > boundingBox.Top) && (rect.Bottom < boundingBox.Bottom)) || ((rect.Top > boundingBox.Top) && (rect.Top < boundingBox.Bottom))) &&
            //    (((rect.Left > boundingBox.Left) && (rect.Left < boundingBox.Right)) || ((rect.Right > boundingBox.Left) && (rect.Right < boundingBox.Right)))) {
            //    //put whatever collision shit is supposed to happen here.
            //    //Console.WriteLine("Colliding with Debris");
            //    return true;
            //}
            return false;
        }

        /// <summary>
        /// Corey - 2/6
        /// Returns just the weight. No setter for you.
        /// </summary>
        /// <returns></returns>
        public int getWeight() {
            return weight;
        }

        public override void Draw(object spriteBatch) {
            //spriteBatch.Draw(texture, boundingBox, GameColor.Red);
        }

        public override void Update(object gameTime) {
            if (soundCD > 0) {
                soundCD--;
            }
        }
    }
}
