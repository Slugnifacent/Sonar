using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Sonar
{
    /// <summary>
    /// Travis Carlson
    /// 2/27/12
    /// A tile representing a spot the player can hide in
    /// </summary>
    public class HidingPlace : Environment
    {
        private float scale;
        private Vector2 position2;
        private Texture2D outline;
        private char dir;
        public bool occupied;
        bool visible;

        public HidingPlace(Vector2 init_Pos, char direction)
        {
            position = init_Pos;
            position2 = new Vector2(position.X - MapUnit.MAX_SIZE / 4, position.Y);
            dir = direction;
            occupied = false;

            if (direction == 'F')
            {
                texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard");
                outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open_outline");
            }
            else if (direction == 'L')
            {
                texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_side_left");
                outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open_side_left_outline");
            }
            else if (direction == 'R')
            {
                texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_side_right");
                outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open_side_right_outline");
            }

            boundingBox = new Rectangle((int)init_Pos.X, (int)init_Pos.Y, MapUnit.MAX_SIZE * 2, MapUnit.MAX_SIZE * 2);
            color = Color.White;
            scale = 1.2f;

            visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            boundingBox.Height = MapUnit.MAX_SIZE;
            boundingBox.Width = MapUnit.MAX_SIZE;

            if (!visible)
            {
                double xDist = Math.Pow((Player.getInstance().position.X - this.position.X), 2);
                double yDist = Math.Pow((Player.getInstance().position.Y - this.position.Y), 2);
                int dist = (int)Math.Sqrt(xDist + yDist);
                if (dist < 100f || dist < VoiceEngine.getInstance().VOLUME * 1000f)
                {
                        visible = true;
                }
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            /*batch.Draw(Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Player/temp"), new Rectangle((int)(boundingBox.X - boundingBox.Width / 1.5),
                                                  boundingBox.Y + boundingBox.Height / 4,
                                            (int)(boundingBox.Width / 2),
                                            (int)(boundingBox.Height / 2)), Color.Red);*/ // Debug for drawing interactable area

            if (dir == 'F')
            {
                if (occupied)
                {
                    texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard");
                    outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_outline");
                }
                else
                {
                    texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open");
                    outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open_outline");
                }
            }
            else if (dir == 'L')
            {
                if (occupied)
                {
                    texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_side_left");
                    outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_side_left_outline");
                }
                else
                {
                    texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open_side_left");
                    outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open_side_left_outline");
                }
            }
            else if (dir == 'R')
            {
                if (occupied)
                {
                    texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_side_right");
                    outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_side_right_outline");
                }
                else
                {
                    texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open_side_right");
                    outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Kitchen/cupboard_open_side_right_outline");
                }
            }

            if (dir == 'L')
            {
                batch.Draw(texture, position, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
            }
            else
            {
                batch.Draw(texture, position2, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
            }
        }

        public void DrawOutline(SpriteBatch batch, Color color)
        {
            if(visible)
                if (dir == 'L')
                {
                    batch.Draw(outline, position, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
                }
                else
                {
                    batch.Draw(outline, position2, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
                }
        }

        #region Collide
        /// <summary>
        /// Steven Ekejiuba 5/9/2012
        /// Determines if there is an object close to Hiding Place, without intersecting it
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Collide(Rectangle rect)
        {           
            // Determines which direction the Cupboard is facing and puts the collision box right in front of it
            if (dir == 'F')
            {
                if (rect.Intersects(new Rectangle(boundingBox.X + boundingBox.Width / 4,
                                                  boundingBox.Y + boundingBox.Height,
                                            (int)(boundingBox.Width / 2),
                                            (int)(boundingBox.Height / 2))))
                {
                    // Cupboard is occupied if player is hiding
                    if (Player.getInstance().isHiding)
                    {
                        occupied = true;
                        Player.getInstance().facing = new Vector2(0, 1);
                    }
                    else
                    {
                        occupied = false;
                    }

                    return true;
                }
            }
            else if (dir == 'L')
            {
                if (rect.Intersects(new Rectangle(boundingBox.X + boundingBox.Width,
                                                  boundingBox.Y + boundingBox.Height / 4,
                                            (int)(boundingBox.Width / 2),
                                            (int)(boundingBox.Height / 2))))
                {
                    // Cupboard is occupied if player is hiding
                    if (Player.getInstance().isHiding)
                    {
                        occupied = true;
                        Player.getInstance().facing = new Vector2(-1, 0);
                    }
                    else
                    {
                        occupied = false;
                    }

                    return true;
                }
            }
            else if (dir == 'R')
            {
                if (rect.Intersects(new Rectangle((int)(boundingBox.X - boundingBox.Width / 1.5),
                                                  boundingBox.Y + boundingBox.Height / 4,
                                            (int)(boundingBox.Width / 2),
                                            (int)(boundingBox.Height / 2))))
                {
                    // Cupboard is occupied if player is hiding
                    if (Player.getInstance().isHiding)
                    {
                        occupied = true;
                        Player.getInstance().facing = new Vector2(1, 0);
                    }
                    else
                    {
                        occupied = false;
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Testing

        public new string toString()
        {
            return "H";
        }
        #endregion
    }
}
