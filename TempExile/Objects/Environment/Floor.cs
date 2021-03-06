﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;








namespace Sonar
{
    public class Floor : Environment
    {
        public enum FloorType : sbyte { Default, Carpet, Concrete, DoorMat, Lab, Bathroom, Kitchen, Hardwood, Hardwood2, UnderDoor/*, Tile, Cement*/ };
        private FloorType type;

        public Floor(GameVector2 init_Pos, FloorType Type)
        {
            position = init_Pos;
            type = Type;
            //texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/floorTile");
            
            switch(Type){
                case FloorType.Default:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/floorTile");
                    //texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/floorTileTest");
                    break;
                case FloorType.Carpet:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/carpet");
                    break;
                case FloorType.Concrete:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/concrete");
                    break;
                case FloorType.DoorMat:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/doorMat");
                    break;
                case FloorType.Lab:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/lab");
                    break;
                case FloorType.Bathroom:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/bathroom");
                    break;
                case FloorType.Kitchen:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/kitchen");
                    break;
                case FloorType.Hardwood:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/hardwood");
                    break;
                case FloorType.Hardwood2:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/hardwood2");
                    break;
                case FloorType.UnderDoor:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/floorTileTest3");
                    break;
                default:
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/floorTileTest3");
                    break;
            }           
            boundingBox = new GameRectangle((int)init_Pos.X, (int)init_Pos.Y, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(object batch)
        {
           batch.Draw(texture, boundingBox, GameColor.White);
        }

        public FloorType GetFloorType()
        {
            return type;
        }

        public GameTexture GetTexture()
        {
            return texture;
        }

        #region Testing
        /// <summary>
        /// Chris Peterson - 1/19/12
        /// </summary>
        /// <returns></returns>
        public new string toString()
        {
            return "F";
        }
        #endregion
    }
}
