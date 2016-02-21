using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Sonar
{
    /// <summary>
    /// Steven Ekejiuba
    /// 4/08/2012
    /// Button that the player presses to unlock the next floor that can be accessed through the elevator
    /// </summary>
    public class LockdownSwitch : Object
    {
        private bool isPressed;
        private char dir;

        public LockdownSwitch(GameVector2 pos, char direction)
        {
            dir = direction;
            if (dir == 'F')
            {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchF1");
            }
            else if (dir == 'B')
            {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchB1");
            }
            else if (dir == 'L')
            {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchL1");
            }
            else if (dir == 'R')
            {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchR1");
            }
            position = pos;
            boundingBox = new GameRectangle((int)position.X, (int)position.Y, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE);

            isPressed = false;
        }

        // Press the Lockdown Switch
        public void Press()
        {
            if (!isPressed)
            {
                isPressed = true;
                SoundManager.playSoundFX(SoundManager.ENVIRONMENT.DOOR_OPEN);
                SoundManager.ElevatorLevel((GameScreen.levels.Length - GameScreen.currentLevel) - 1);
                Exit.ElevatorVolume(80);
            }
        }

        // Sets the Lockdown Switch to default settings
        public void Reset()
        {
            isPressed = false;
            if (dir == 'F') {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchF1");
            }
            else if (dir == 'B') {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchB1");
            }
            else if (dir == 'L') {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchL1");
            }
            else if (dir == 'R') {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchR1");
            }
        }

        // Determines if the player has pressed the Lockdown Switch
        public bool IsPressed()
        {
            return isPressed;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(object spriteBatch)
        {
            // If lockdown switch is pressed, display image of pressed switch
            if (isPressed)
            {
                if (dir == 'F')
                {
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchF2");
                }
                else if (dir == 'B')
                {
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchB2");
                }
                else if (dir == 'L')
                {
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchL2");
                }
                else if (dir == 'R')
                {
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchR2");
                }
            }
            /*else
            {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/LockdownSwitch/desk_switchF1");
            }*/

            spriteBatch.Draw(texture, boundingBox, GameColor.White);
        }
    }
}
