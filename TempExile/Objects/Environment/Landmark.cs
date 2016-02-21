using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




namespace Sonar
{
    public class Landmark: Object
    {
        protected GameTexture sprite;
        protected float scale;
        protected float numOfTilesH = 1;
        protected float numOfTilesV = 1;
        protected char dir;
        public enum Type {bedF, bedB, bedL, bedR, body, cardboardBox, chairF, chairB, chairL, chairR,
                          fridgeF, fridgeL, fridgeR, gymBench, gymBike, gymRow, gymTread, gymWeights,
                          janitor, plant, plantFallen1, plantFallen2, sinkF, sinkL, sinkR, table, deskNorm,
                          deskNormSide, deskLab, deskLabSide, deskOffice, deskOfficeSide, toiletF,
                          toiletL, toiletR, toiletB, carBasicF, carBasicB, carBasicL, carBasicR, carVanF,
                          carVanB, carVanL, carVanR, carSportsF, carSportsB, carSportsL, carSportsR,
                          bookshelfF, bookshelfL, bookshelfR, bookshelfFallenF, bookshelfFallenL,
                          bookshelfFallenR, couchF, couchL, couchR, couchB, fileCabinetF, serverF,
                          fileCabinetL, fileCabinetR, fileCabinetFallenL, fileCabinetFallenR, waterTank,
                          generator, stoveF, stoveB, stoveL, stoveR};
        private Type type;

        public Landmark(GameVector2 Position, Type unitType)
        {
            position = Position;
            type = unitType;
            scale = 1;
            String texture = "Textures/Objects/Environment/";
            switch (type)
            {
                case Type.bedF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bed");
                    numOfTilesV = 2;
                    dir = 'F';
                    break;
                case Type.bedB:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bed_side_back");
                    numOfTilesV = 2;
                    dir = 'B';
                    break;
                case Type.bedL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bed_side_left");
                    numOfTilesH = 2;
                    dir = 'L';
                    break;
                case Type.bedR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bed_side_right");
                    numOfTilesH = 2;
                    dir = 'R';
                    break;
                case Type.body:
                    Random rand = Game1.random;
                    int vialNum = rand.Next(1, 3);

                    texture += "Bodies/dead" + vialNum;
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture);
                    //scale = .15f;
                    scale = 1.2f;
                    dir = 'F';
                    break;
                case Type.bookshelfF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bookshelf");
                    dir = 'F';
                    break;
                case Type.bookshelfL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bookshelf_side_left");
                    dir = 'L';
                    break;
                case Type.bookshelfR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bookshelf_side_right");
                    dir = 'R';
                    break;
                case Type.bookshelfFallenF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bookshelf_fallen");
                    dir = 'F';
                    break;
                case Type.bookshelfFallenL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bookshelf_fallen_side_left");
                    dir = 'L';
                    break;
                case Type.bookshelfFallenR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/bookshelf_fallen_side_right");
                    dir = 'R';
                    break;
                case Type.cardboardBox:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/cardboardBox");
                    dir = 'F';
                    break;
                case Type.carBasicF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carBasic");
                    numOfTilesV = 3;
                    numOfTilesH = 1.5f;
                    dir = 'F';
                    break;
                case Type.carBasicB:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carBasic_back");
                    numOfTilesV = 3;
                    numOfTilesH = 1.5f;
                    dir = 'B';
                    break;
                case Type.carBasicL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carBasic_left");
                    numOfTilesH = 3;
                    numOfTilesV = 1.5f;
                    dir = 'L';
                    break;
                case Type.carBasicR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carBasic_right");
                    numOfTilesH = 3;
                    numOfTilesV = 1.5f;
                    dir = 'R';
                    break;
                case Type.carSportsF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carSports");
                    numOfTilesV = 3;
                    numOfTilesH = 1.5f;
                    dir = 'F';
                    break;
                case Type.carSportsB:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carSports_back");
                    numOfTilesV = 3;
                    numOfTilesH = 1.5f;
                    dir = 'B';
                    break;
                case Type.carSportsL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carSports_left");
                    numOfTilesH = 3;
                    numOfTilesV = 1.5f;
                    dir = 'L';
                    break;
                case Type.carSportsR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carSports_right");
                    numOfTilesH = 3;
                    numOfTilesV = 1.5f;
                    dir = 'R';
                    break;
                case Type.carVanF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carVan");
                    numOfTilesV = 3;
                    numOfTilesH = 1.5f;
                    dir = 'F';
                    break;
                case Type.carVanB:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carVan_back");
                    numOfTilesV = 3;
                    numOfTilesH = 1.5f;
                    dir = 'B';
                    break;
                case Type.carVanL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carVan_left");
                    numOfTilesH = 3;
                    numOfTilesV = 1.5f;
                    dir = 'L';
                    break;
                case Type.carVanR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Garage/carVan_right");
                    numOfTilesH = 3;
                    numOfTilesV = 1.5f;
                    dir = 'R';
                    break; 
                case Type.chairF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/chair");
                    dir = 'F';
                    break;
                case Type.chairB:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/chair_back");
                    dir = 'B';
                    break;
                case Type.chairL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/chair_side_left");
                    scale = 1.2f;
                    dir = 'L';
                    break;
                case Type.chairR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/chair_side_right");
                    scale = 1.2f;
                    dir = 'R';
                    break;
                case Type.couchF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/couch_up");
                    dir = 'F';
                    break;
                case Type.couchL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/couch_left");
                    dir = 'L';
                    break;
                case Type.couchR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/couch_right");
                    dir = 'R';
                    break;
                case Type.couchB:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/couch");
                    dir = 'B';
                    break;
                case Type.fridgeF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Kitchen/fridge");
                    dir = 'F';
                    break;
                case Type.fridgeL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Kitchen/fridge_side_left");
                    dir = 'L';
                    break;
                case Type.fridgeR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Kitchen/fridge_side_right");
                    dir = 'R';
                    break;
                case Type.gymBench:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Gym/gymBench");
                    numOfTilesV = 2;
                    dir = 'F';
                    break;
                case Type.gymBike:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Gym/gymBike");
                    numOfTilesV = 2;
                    dir = 'F';
                    break;
                case Type.gymRow:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Gym/gymRow");
                    numOfTilesV = 2;
                    dir = 'F';
                    break;
                case Type.gymTread:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Gym/gymTread");
                    numOfTilesV = 2;
                    dir = 'F';
                    break;
                case Type.gymWeights:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Gym/gymWeights");
                    dir = 'F';
                    break;
                case Type.janitor:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Bodies/janitor");
                    scale = 1.2f;
                    dir = 'F';
                    break;
                case Type.plant:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/plant");
                    scale = 1.2f;
                    dir = 'F';
                    break;
                case Type.plantFallen1:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/plant_fallen_left");
                    dir = 'F';
                    break;
                case Type.plantFallen2:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/plant_fallen_right");
                    dir = 'F';
                    break;
                case Type.sinkF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/sink");
                    dir = 'F';
                    break;
                case Type.sinkL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/sink_side_left");
                    dir = 'L';
                    break;
                case Type.sinkR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/sink_side_right");
                    dir = 'R';
                    break;
                case Type.table:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/table");
                    dir = 'F';
                    break;
                case Type.deskNorm:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/desk_basic");
                    dir = 'F';
                    break;
                case Type.deskNormSide:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/desk_basic_side");
                    dir = 'L';
                    break;
                case Type.deskLab:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Lab/desk_lab");
                    dir = 'F';
                    break;
                case Type.deskLabSide:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Lab/desk_lab_side");
                    dir = 'L';
                    break;
                case Type.deskOffice:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Office/desk_office");
                    dir = 'F';
                    break;
                case Type.deskOfficeSide:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Office/desk_office_side");
                    dir = 'L';
                    break;
                case Type.toiletF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Restroom/toilet");
                    dir = 'F';
                    break;
                case Type.toiletL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Restroom/toilet_side_left");
                    dir = 'L';
                    break;
                case Type.toiletR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Restroom/toilet_side_right");
                    dir = 'R';
                    break;
                case Type.toiletB:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Restroom/toilet_side_back");
                    dir = 'B';
                    break;
                case Type.fileCabinetF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Office/filecabinet");
                    dir = 'F';
                    break;
                case Type.fileCabinetL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Office/filecabinet_left");
                    dir = 'L';
                    break;
                case Type.fileCabinetR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Office/filecabinet_right");
                    dir = 'R';
                    break;
                case Type.fileCabinetFallenL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Office/filecabinet_fallen_left");
                    dir = 'L';
                    break;
                case Type.fileCabinetFallenR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Office/filecabinet_fallen_right");
                    dir = 'R';
                    break;
                case Type.serverF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/server");
                    dir = 'F';
                    break;
                case Type.stoveF:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Kitchen/stoveF");
                    dir = 'F';
                    break;
                case Type.stoveB:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Kitchen/stoveB");
                    dir = 'B';
                    break;
                case Type.stoveL:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Kitchen/stoveL");
                    dir = 'L';
                    break;
                case Type.stoveR:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Kitchen/stoveR");
                    dir = 'R';
                    break;
                case Type.waterTank:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/waterTank");
                    dir = 'F';
                    break;
                case Type.generator:
                    sprite = Game1.contentManager.Load<GameTexture>(@"" + texture + "Misc/generator");
                    dir = 'F';
                    break;
                default:
                    sprite = Game1.contentManager.Load<GameTexture>(@"player/player");
                    dir = 'F';
                    break;
            }
            boundingBox = new GameRectangle((int)position.X, (int)position.Y, (int)(MapUnit.MAX_SIZE * scale), (int)(MapUnit.MAX_SIZE * scale));
            updateBoundingBox (position);
            if (scale != 1)
            {
                boundingBox.X = (int)position.X - (int)(boundingBox.Width * scale / 12);
                boundingBox.Y = (int)position.Y - (int)(boundingBox.Height * scale / 12);
            }

            if (numOfTilesH != 1 || numOfTilesV != 1)
            {
                /*// Make the image scale to the amount of tiles it should take up
                if (dir == 'L')
                {
                    boundingBox.Width *= numOfTilesH;
                    boundingBox.Height *= numOfTilesV;
                }
                else if (dir == 'R') // *****************************Continue here
                {*/
                    boundingBox.Width = (int)(boundingBox.Width * numOfTilesH);
                    boundingBox.Height = (int)(boundingBox.Height * numOfTilesV);

                // Ensures that Landmarks with a base that is not its left or top are ofset to the proper position
                    if (numOfTilesH > 1 && (dir != 'L' && dir != 'F'))
                    {
                        boundingBox.X -= MapUnit.MAX_SIZE * (int)(Math.Abs (numOfTilesH) - 1);
                    }
                    else if (numOfTilesH < 1 && (dir != 'L' && dir != 'F'))
                    {
                        boundingBox.X += MapUnit.MAX_SIZE * (int)(Math.Abs (numOfTilesH) - 1);
                    }
                    if (numOfTilesV > 1 && (dir != 'L' && dir != 'F'))
                    {
                        boundingBox.Y -= MapUnit.MAX_SIZE * (int)(Math.Abs (numOfTilesV) - 1);
                    }
                    else if (numOfTilesV < 1 && (dir != 'L' && dir != 'F'))
                    {
                        boundingBox.Y += MapUnit.MAX_SIZE * (int)(Math.Abs (numOfTilesV) - 1);
                    }
                /*}
                else if (dir == 'F')
                {
                    boundingBox.Height *= numOfTiles;
                }
                else if (dir == 'B')
                {
                    boundingBox.Height *= numOfTiles;
                    boundingBox.Y -= boundingBox.Height / 2;
                }*/
            }
        }

        public override void Draw(object batch)
        {
            //batch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/Concrete"), boundingBox, GameColor.Red); // Draw bounding box

            // Account for individual size defects
            if (scale == 1)
            {
                batch.Draw(sprite, boundingBox, GameColor.White);
                //batch.Draw(sprite, position, null, GameColor.White, 0, GameVector2.Zero, scale, SpriteEffects.None, 1);
            }
            else
            {
                if (type == Type.body)
                {
                    batch.Draw(sprite, position, null, GameColor.White, 0, new GameVector2 ((int)(boundingBox.Width / 4.5), (int)(boundingBox.Height / 4.5)), scale, SpriteEffects.None, 1);
                }
                else
                {
                    //boundingBox.Width = (int)(boundingBox.Width * scale);
                    //boundingBox.Height = (int)(boundingBox.Height * scale);
                    batch.Draw(sprite, boundingBox, GameColor.White);
                    //GameVector2 shiftedPos = new GameVector2(position.X - (boundingBox.Width * scale / 8), position.Y - (boundingBox.Height * scale / 8));
                    //batch.Draw(sprite, shiftedPos, null, GameColor.White, 0, GameVector2.Zero, scale, SpriteEffects.None, 1);
                }
            }
        }

        public override void Update(GameTime time)
        {

        }
    }
}
