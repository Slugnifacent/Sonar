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
    abstract public class Entity : Object
    {
        //protected Vector2 rootPos;
        //public float speed;
        protected float health;
        protected int footstepTimer = 0;
        //TimeSpan time = 
        int frameSpeed = 50;
        long animationTime = 0;
        long elapsedTime;
        long targetTime = 0;
        public bool isRunning = false;
        public bool isStealthing = false;
        public bool isMoving = true;
        public bool isAttacking = false;
        protected AnimationCollection animation;
        protected float animationInterval;
        public int soundTimer = 0;
        public int soundTimerReset = 20;
        public int id = 0;
        public float sound = 0;

        public override void Update(GameTime gameTime)
        {

        }

        public void initializeFootstepTimer(int value)
        {
            footstepTimer = value;
        }

        public void resetSound()
        {
            sound = 0;
        }

        public void storeSound(float soundStrength)
        {
            if(soundStrength > sound) // Only store the loudest sound of an entity
                sound = soundStrength;
        }

        public void updateSound()
        {
            //Console.WriteLine("Update Sound Is Being Called.");
            //Console.WriteLine("Timer: " + soundTimer);
            soundTimer--;
            if (soundTimer < 0)
            {
                soundTimer = soundTimerReset;
                if (sound > 100f)
                    VisionManager.addVisionPoint(position, this.sound, this.GetType().Equals(typeof(Player)));
                resetSound();
            }
        }

        protected int updateFootstepTimer()
        {
            int left = 1;
            int right = 2;
            int maxTimer = 55;

            footstepTimer++;
            if (footstepTimer > maxTimer)
            {
                footstepTimer = 0;
                return left;
            }
            if (footstepTimer == (int)(maxTimer / 2))
                return right;
            
            return -1;
        }

        protected abstract void InitializeAnimations();

        public virtual void SetSprite(String name)
        {
            animation.RUN(name);
        }

        public virtual void SetSprite()
        {
            //if (isMoving)
            //{

                // Facing Right
                if (positionPrevious==position){
                    // If player presses Stealth Key when not moving, stealth
                    if (isStealthing)
                    {
                        if (animation.CurrentAnimation() == "UpRun" ||
                            animation.CurrentAnimation() == "UpWalk")
                        {
                            animation.RUN("UpStealth");
                        }
                        else if (animation.CurrentAnimation() == "DownRun" ||
                            animation.CurrentAnimation() == "DownWalk")
                        {
                            animation.RUN("DownStealth");
                        }
                        else if (animation.CurrentAnimation() == "LeftRun" ||
                            animation.CurrentAnimation() == "LeftWalk")
                        {
                            animation.RUN("LeftStealth");
                        }
                        else if (animation.CurrentAnimation() == "RightRun" ||
                            animation.CurrentAnimation() == "RightWalk")
                        {
                            animation.RUN("RightStealth");
                        }
                    }
                    // If player releases Stealth Key when not moving, go back to walking sprites
                    else
                    {
                        if (animation.CurrentAnimation() == "UpStealth" ||
                            animation.CurrentAnimation() == "UpRun")
                        {
                            animation.RUN("UpWalk");
                        }
                        else if (animation.CurrentAnimation() == "DownStealth" ||
                                 animation.CurrentAnimation() == "DownRun")
                        {
                            animation.RUN("DownWalk");
                        }
                        else if (animation.CurrentAnimation() == "LeftStealth" ||
                                 animation.CurrentAnimation() == "LeftRun")
                        {
                            animation.RUN("LeftWalk");
                        }
                        else if (animation.CurrentAnimation() == "RightStealth" ||
                                 animation.CurrentAnimation() == "RightRun")
                        {
                            animation.RUN("RightWalk");
                        }
                    }

                    animation.STOP();
                }

                else if (position.X == positionPrevious.X)
                {
                    if (positionPrevious.Y < position.Y)
                    {
                        if (isRunning)
                        {
                            currSprite.Y = 4;
                            animation.RUN("DownRun");
                        }
                        else if (isStealthing)
                        {
                            currSprite.Y = 12;
                            animation.RUN("DownStealth");
                        }
                        else
                        {
                            currSprite.Y = 0;
                            animation.RUN("DownWalk");
                        }
                    }
                    else
                    {
                        if (isRunning)
                        {
                            currSprite.Y = 4;
                            animation.RUN("UpRun");
                        }
                        else if (isStealthing)
                        {
                            currSprite.Y = 12;
                            animation.RUN("UpStealth");
                        }
                        else
                        {
                            currSprite.Y = 0;
                            animation.RUN("UpWalk");
                        }
                    }
                }

                else if (positionPrevious.X < position.X/* || orientation.X == 1*/)
                {
                    // If entity is also moving upward
                    if (positionPrevious.Y > position.Y/* || orientation.Y == -1*/)
                    {
                        // Facing NorthEast (but going mainly North)
                        if ((position.X - positionPrevious.X) < (positionPrevious.Y - position.Y))
                        {
                            if (isRunning)
                            {
                                currSprite.Y = 5;
                                animation.RUN("UpRun");
                            }
                            else if (isStealthing)
                            {
                                currSprite.Y = 11;
                                animation.RUN("UpStealth");
                            }
                            else
                            {
                                currSprite.Y = 1;
                                animation.RUN("UpWalk");
                            }
                        }
                        // Facing NorthEast (but going mainly East)
                        else
                        {
                            if (isRunning)
                            {
                                currSprite.Y = 7;
                                animation.RUN("RightRun");
                            }
                            else if (isStealthing)
                            {
                                currSprite.Y = 9;
                                animation.RUN("RightStealth");
                            }
                            else
                            {
                                currSprite.Y = 3;
                                animation.RUN("RightWalk");
                            }
                        }
                    }
                    // If entity is also moving downward
                    else if ((positionPrevious.Y < position.Y/* || orientation.Y == 1*/))
                    {
                        // Facing SouthEast (but going mainly South)
                        if ((position.X - positionPrevious.X) < (position.Y - positionPrevious.Y))
                        {
                            if (isRunning)
                            {
                                currSprite.Y = 4;
                                animation.RUN("DownRun");
                            }
                            else if (isStealthing)
                            {
                                currSprite.Y = 12;
                                animation.RUN("DownStealth");
                            }
                            else
                            {
                                currSprite.Y = 0;
                                animation.RUN("DownWalk");
                            }
                        }
                        // Facing SouthEast (but going mainly East)
                        else
                        {
                            if (isRunning)
                            {
                                currSprite.Y = 7;
                                animation.RUN("RightRun");
                            }
                            else if (isStealthing)
                            {
                                currSprite.Y = 9;
                                animation.RUN("RightStealth");
                            }
                            else
                            {
                                currSprite.Y = 3;
                                animation.RUN("RightWalk");
                            }
                        }
                    }
                    // Entity is going straight to the right
                    else if (positionPrevious.Y == position.Y)
                    {
                        if (isRunning)
                        {
                            currSprite.Y = 7;
                            animation.RUN("RightRun");
                        }
                        else if (isStealthing)
                        {
                            currSprite.Y = 9;
                            animation.RUN("RightStealth");
                        }
                        else
                        {
                            currSprite.Y = 3;
                            animation.RUN("RightWalk");
                        }
                    }
                }

                // Facing Left

                else if (positionPrevious.X > position.X/* || orientation.X == -1*/)
                {
                    // If entity is also moving upward
                    if (positionPrevious.Y > position.Y/* || orientation.Y == -1*/)
                    {
                        // Facing NorthWest (but going mainly North)
                        if ((positionPrevious.X - position.X) < (positionPrevious.Y - position.Y))
                        {
                            if (isRunning)
                            {
                                currSprite.Y = 5;
                                animation.RUN("UpRun");
                            }
                            else if (isStealthing)
                            {
                                currSprite.Y = 11;
                                animation.RUN("UpStealth");
                            }
                            else
                            {
                                currSprite.Y = 1;
                                animation.RUN("UpWalk");
                            }
                        }
                        // Facing NorthWest (but going mainly West)
                        else
                        {
                            if (isRunning)
                            {
                                currSprite.Y = 6;
                                animation.RUN("LeftRun");
                            }
                            else if (isStealthing)
                            {
                                currSprite.Y = 10;
                                animation.RUN("LeftStealth");
                            }
                            else
                            {
                                currSprite.Y = 2;
                                animation.RUN("LeftWalk");
                            }
                        }
                    }
                    // If entity is also moving downward
                    else if ((positionPrevious.Y < position.Y/* || orientation.Y == 1*/))
                    {
                        // Facing SouthWest (but going mainly South)
                        if ((positionPrevious.X - position.X) < (position.Y - positionPrevious.Y))
                        {
                            if (isRunning)
                            {
                                currSprite.Y = 4;
                                animation.RUN("DownRun");
                            }
                            else if (isStealthing)
                            {
                                currSprite.Y = 12;
                                animation.RUN("DownStealth");
                            }
                            else
                            {
                                currSprite.Y = 0;
                                animation.RUN("DownWalk");
                            }
                        }
                        // Facing SouthWest (but going mainly West)
                        else
                        {
                            if (isRunning)
                            {
                                currSprite.Y = 6;
                                animation.RUN("LeftRun");
                            }
                            else if (isStealthing)
                            {
                                currSprite.Y = 10;
                                animation.RUN("LeftStealth");
                            }
                            else
                            {
                                currSprite.Y = 2;
                                animation.RUN("LeftWalk");
                            }
                        }
                    }
                    // Entity is going straight to the left
                    else if (positionPrevious.Y == position.Y)
                    {
                        if (isRunning)
                        {
                            currSprite.Y = 6;
                            animation.RUN("LeftRun");
                        }
                        else if (isStealthing)
                        {
                            currSprite.Y = 10;
                            animation.RUN("LeftStealth");
                        }
                        else
                        {
                            currSprite.Y = 2;
                            animation.RUN("LeftWalk");
                        }
                    }
                }

                // Facing Down

                else if ((positionPrevious.Y < position.Y/* || orientation.Y == 1*/) && orientation.X == 0)
                {
                    if (isRunning)
                    {
                        currSprite.Y = 4;
                        animation.RUN("DownRun");
                    }
                    else if (isStealthing)
                    {
                        currSprite.Y = 12;
                        animation.RUN("DownStealth");
                    }
                    else
                    {
                        currSprite.Y = 0;
                        animation.RUN("DownWalk");
                    }
                }

                // Facing Up

                else if ((positionPrevious.Y > position.Y/* || orientation.Y == -1*/) && orientation.X == 0)
                {
                    if (isRunning)
                    {
                        currSprite.Y = 5;
                        animation.RUN("UpRun");
                    }
                    else if (isStealthing)
                    {
                        currSprite.Y = 11;
                        animation.RUN("UpStealth");
                    }
                    else
                    {
                        currSprite.Y = 1;
                        animation.RUN("UpWalk");
                    }
                }

                // Any other case not covered (this should not be reached)

                else
                {
                    //Console.Out.WriteLine("No proper sprite");
                }
            
            /*else
            {
                // If player presses Stealth Key when not moving, stealth
                if (isStealthing)
                {
                    if (animation.CurrentAnimation() == "UpRun" ||
                        animation.CurrentAnimation() == "UpWalk")
                    {
                        animation.RUN("UpStealth");
                    }
                    else if (animation.CurrentAnimation() == "DownRun" ||
                        animation.CurrentAnimation() == "DownWalk")
                    {
                        animation.RUN("DownStealth");
                    }
                    else if (animation.CurrentAnimation() == "LeftRun" ||
                        animation.CurrentAnimation() == "LeftWalk")
                    {
                        animation.RUN("LeftStealth");
                    }
                    else if (animation.CurrentAnimation() == "RightRun" ||
                        animation.CurrentAnimation() == "RightWalk")
                    {
                        animation.RUN("RightStealth");
                    }
                }
                // If player releases Stealth Key when not moving, go back to walking sprites
                else
                {
                    if (animation.CurrentAnimation() == "UpStealth" ||
                        animation.CurrentAnimation() == "UpRun")
                    {
                        animation.RUN("UpWalk");
                    }
                    else if (animation.CurrentAnimation() == "DownStealth" ||
                             animation.CurrentAnimation() == "DownRun")
                    {
                        animation.RUN("DownWalk");
                    }
                    else if (animation.CurrentAnimation() == "LeftStealth" ||
                             animation.CurrentAnimation() == "LeftRun")
                    {
                        animation.RUN("LeftWalk");
                    }
                    else if (animation.CurrentAnimation() == "RightStealth" ||
                             animation.CurrentAnimation() == "RightRun")
                    {
                        animation.RUN("RightWalk");
                    }
                }*/

                //animation.STOP();
            

            // Update current frame
            updateSprite(positionPrevious, position);
        }

        // Animate the sprites
        public virtual void updateSprite(Vector2 prepos, Vector2 pos)
        {
            float dir_x = pos.X - prepos.X;
            float dir_y = pos.Y - prepos.Y;

            if (position == positionPrevious)
            {
                isMoving = false;
            }
            else
            {
                isMoving = true;
            }

            if (isMoving)
            {
                if (NextFrame())
                {
                    currSprite.X = (currSprite.X + 1) % spriteRowSize;
                }
            }
            else
            {
                currSprite.X = 0;
            }
        }

        ///
        /// Steven Ekejiuba - 3/3/'12
        /// Added logic for regulating frame speed
        ///
        // Reset timer for current frame
        public void setTargetTime()
        {
            targetTime = elapsedTime + frameSpeed;
        }

        // Check to see if it is time to switch to the next frame of the current animation
        public bool NextFrame()
        {
            elapsedTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

            if (targetTime == 0)
            {
                setTargetTime();
            }

            if (elapsedTime >= targetTime)
            {
                setTargetTime();
                return true;
            }
            return false;
        }

        // Begin timer for animation
        public void StartAnimation()
        {
            animationTime = (spriteRowSize * frameSpeed) + elapsedTime; // Probably need to tweak values (Steven)****
        }

        // The animation is complete
        public bool AnimationDone()
        {
            if (elapsedTime >= animationTime)
            {
                return true;
            }
            return false;
        }

        public void ResetTimer()
        {
            this.soundTimer = this.soundTimerReset;
        }

        public AnimationCollection GetAnimation()
        {
            return animation;
        }
    }
}
