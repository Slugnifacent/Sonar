using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;








namespace Sonar
{
    public class Wall : Environment
    {
        private bool isCracked;
        float colorValue;

        public Wall(GameVector2 init_Pos, bool crackVal)
        {
            position = init_Pos;
            isCracked = crackVal;
            //texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Wall/wallConcrete");
            boundingBox = new GameRectangle((int)init_Pos.X, (int)init_Pos.Y, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE);
            colorValue = 1;
        }

        public override void Update(GameTime time)
        {

        }

        public override void Draw(object spriteBatch)
        {
            //spriteBatch.Draw(texture, position, null, GameColor.White, 0, new GameVector2(0, 0), 1, SpriteEffects.None, 0);
            //spriteBatch.Draw(texture, boundingBox, new GameColor(colorValue, colorValue, colorValue, 255f));
        }

        #region Testing

        /// <summary>
        /// Chris Peterson - 1/19/12
        /// </summary>
        /// <returns></returns>
        public new string toString()
        {
            return "W";
        }
        #endregion
    }
}
