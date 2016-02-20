using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Sonar
{

    public enum GameButtons : byte
    {
        ACTION,
        STEALTH,
        THROW
    }

    public enum GameAxes : byte
    {
        HORIZONTAL,
        VERTICAL,
        TRIGGER_LEFT,
        TRIGGER_RIGHT
    }

    /// <summary>
    /// This is the controller class. It will be used to handle all controller/keyboard input from the player.
    /// An instance of controller will be created in game1 and will call the proper function based on what screen it is in.
    /// Currently it just calls playerControls().
    /// </summary>
    public class ControllerInput
    {
        static ControllerInput input;

        Dictionary<GameButtons, GameButton> gButtons;
        Dictionary<GameAxes, GameAxis> gAxis;

        private ControllerInput()
        {
            gButtons = new Dictionary<GameButtons, GameButton>() { };
            AddButton(GameButtons.ACTION);
            AddButton(GameButtons.THROW);
            AddButton(GameButtons.STEALTH);

            gAxis = new Dictionary<GameAxes, GameAxis>() { };
            AddAxis(GameAxes.HORIZONTAL);
            AddAxis(GameAxes.VERTICAL);
            AddAxis(GameAxes.TRIGGER_LEFT);
            AddAxis(GameAxes.TRIGGER_RIGHT);
            
        }

        public static ControllerInput getInstance() {
            if (input == null) input = new ControllerInput();
            return input;
        }
        
        void AddButton(GameButtons button)
        {
            gButtons.Add(button, new GameButton(button.ToString()));            
        }

        void AddAxis(GameAxes axis)
        {
            gAxis.Add(axis, new GameAxis(axis.ToString()));
        }

        /// <summary>
        /// The controls if the player is using the keyboard. Done by Travis, moved here by Corey.
        /// </summary>
        /// <param name="player"></param>
        private void playerKeyboard(object player)
        {


            //if (Keyboard[Keys.Delete].Held)
            //{
            //    player.setNoise(851);
            //    player.setSlowDown(false);
            //    VisionManager.setVisionColor(Color.CornflowerBlue);
            //}

            //#region Cardinal Movement
            //if (Game1.camera.Zoom.X >= 1 && !player.isHiding)
            //{
            //    if (Keyboard[Keys.Up].Held)
            //    {
            //        player.orientation.X += 0;
            //        player.orientation.Y += (!player.messControls) ? -1 : 1;
            //        player.orientation.Normalize();
            //        player.isMoving = true;
            //    }

            //    if (Keyboard[Keys.Down].Held)
            //    {
            //        player.orientation.X += 0;
            //        player.orientation.Y += (!player.messControls) ? 1 : -1;
            //        player.orientation.Normalize();
            //        player.isMoving = true;
            //    }
            //    if (Keyboard[Keys.Left].Held)
            //    {
            //        player.orientation.X += (!player.messControls) ? -1 : 1;
            //        player.orientation.Y += 0;
            //        player.orientation.Normalize();
            //        player.isMoving = true;
            //    }
            //    if (Keyboard[Keys.Right].Held)
            //    {
            //        player.orientation.X += (!player.messControls) ? 1 : -1;
            //        player.orientation.Y += 0;
            //        player.orientation.Normalize();
            //        player.isMoving = true;
            //    }
            //}

            //if (!Keyboard[Keys.Right].Held && !Keyboard[Keys.Left].Held && !Keyboard[Keys.Up].Held && !Keyboard[Keys.Down].Held)
            //{
            //    player.isMoving = false;
            //}
            //#endregion Cardinal Movement


            //if (Keyboard[Keys.C].Pressed)
            //{
            //    if (!player.isHiding) {
            //        if (player.throwing.Count != 0 && !player.throwCooldown) {
            //            player.throwing[0].throwObject(player);
            //        }
            //    }
            //}


            //#region Space Key

            //// If player is near lockdown switch and presses the action key, set the switch state to pressed
            //if (Keyboard[Keys.Space].Pressed)
            //{
            //    if (player.ldSwitch != null && !player.ldSwitch.IsPressed())
            //    {
            //        player.ldSwitch.Press();
            //        player.pressedLockdown = true;
            //    }
            //}

            ////Allows the player to hide if she is at a hideable tile
            //if (Keyboard[Keys.Space].Pressed && player.canHide)
            //{
            //    player.isHiding = (!player.isHiding) ? true : false;
            //}

            //if (Keyboard[Keys.Space].Pressed)
            //{
            //    // Open or Close Door
            //    if (player.door != null)
            //    {
            //        if ((!player.door.isOccupied && player.door.isOpen) || !player.door.isOpen)
            //        {
            //            player.door.Interact();
            //        }
            //    }

            //    // Turn Radio On or Off
            //    else if (player.radio != null && player.radio.ALPHA > 100)
            //    {
            //        player.radio.Toggle();
            //    }
            //    CutSceneScreen.skipCutScene();
            //}
            //#endregion Space Key

            //player.isStealthing = (Keyboard[Keys.X].Held && !player.pooped) ? true : false;

            //if (player.orientation.Length() > .1)
            //{
            //    player.speed = 2;
            //    player.isMoving = true;

            //    //player.isStealthing = (Keyboard[Keys.X].Held && !player.pooped) ? true : false;

            //    player.isRunning = (Keyboard[Keys.Z].Held && !player.isStealthing && !player.pooped) ? true : false;
                
            //    if (player.isStealthing)
            //    {
            //        player.speed = 1f;
            //    }
                
            //    if (player.isRunning)
            //    {
            //        player.speed = (player.getSlowDown()) ? 2 : 4;
            //    }

            //    if (player.pooped)
            //    {
            //        player.speed = 1f;
            //    }
            //}
            //else player.isMoving = player.isRunning = false;
            


            //if (Keyboard[Keys.D1].Pressed)
            //{
            //    Shadows.toggleKeyPressed = !Shadows.toggleKeyPressed;
            //    Shadows.isActive = !Shadows.isActive;
            //}


        }

        /// <summary>
        /// The player controls when using the Xbox 360 Controller. Made by Travis, moved here by Corey.
        /// </summary>
        /// <param name="player"></param>
        private void playerController(object player)
        {
            //player.orientation.Y = gAxis[GameAxes.VERTICAL].Value;

            ////if (Controller[Buttons.DPadUp].Pressed) Shadows.isActive = true;
            ////if (Controller[Buttons.DPadDown].Pressed) Shadows.isActive = false;

            //#region A Button // Throw Object           

            //if (player.radio != null)
            //{
            //   if (gAxis[GameAxes.VERTICAL].Value > .8f) player.radio.decreaseVolume(.01f);
            //   if (gAxis[GameAxes.VERTICAL].Value < .8f) player.radio.increaseVolume(.01f);
            //}

            //// Action
            //if (gButtons[GameButtons.ACTION].Pressed)
            //{
            //    // Open or Close Door
            //    if (player.door != null)
            //    {
            //        if ((!player.getBox().Intersects(player.door.getBox()) && player.door.isOpen) ||
            //            !player.door.isOpen)
            //        {
            //            player.door.Interact();
            //        }
            //    }
            //    else if (player.canHide)
            //    {
            //        player.isHiding = (!player.isHiding) ? true : false;
            //    }
            //    // Turn Radio On or Off
            //    else if (player.radio != null && player.radio.ALPHA > 100)
            //    {
            //        player.radio.Toggle();
            //    }

            //    // If player is near lockdown switch and presses the action key, set the switch state to pressed
            //    if (player.ldSwitch != null && !player.ldSwitch.IsPressed())
            //    {
            //        player.ldSwitch.Press();
            //        player.pressedLockdown = true;
            //    }
            //}


            //#endregion A Button 

            //#region X Button
            //if (gButtons[GameButtons.STEALTH].Pressed)
            //    player.isStealthing = !player.isStealthing;
            //#endregion X Button

            //#region B Button
            //if (gButtons[GameButtons.THROW].Pressed)
            //{
            //    if (player.throwing.Count != 0 && !player.throwCooldown)
            //    {
            //        player.throwing[0].throwObject(player);
            //    }
            //}           
            //#endregion

            //#region Movement Logic
            //if (player.orientation.Length() > .1)
            //{
            //    if (!player.isHiding)
            //    {
            //        player.isMoving = true;
            //        int stealthMod = (player.isStealthing) ? 1 : 2;
            //        float triggerMod = (player.isStealthing) ? 0 : gAxis[GameAxes.TRIGGER_RIGHT].Value;

            //        player.speed = stealthMod * (player.orientation.Length() + triggerMod);
            //        player.isRunning = (player.speed > 3.3f) ? true : false;
            //    }
            //}else{
            //    player.isMoving = player.isRunning = false;
            //}
            //#endregion Movement Logic
        }


        /// <summary>
        /// The function called to allow the player to take actions in game. Currently just handles movement. Made by Travis, moved here by Corey.
        /// </summary>
        /// <param name="player"></param>

        public void playerControls(object player)
        {
            //player.orientation = GameVector2.ZERO;

            //if (!player.isKnockedBack)
            //{ // disables controls when player knocked back by Wrath
            //    //if(usingKeyboard)
            //    //    playerKeyboard(player);
            //    //else
            //    //    playerController(player);                
            //}
            //else
            //    player.orientation = player.facing;
            //if (float.IsNaN(player.orientation.X) || float.IsNaN(player.orientation.Y)) player.orientation = GameVector2.ZERO;
            //player.positionPrevious = player.position;
            ////if (Keyboard[Keys.Right].Held || Keyboard[Keys.Left].Held || Keyboard[Keys.Up].Held || Keyboard[Keys.Down].Held)
            //player.position += player.orientation * player.speed;
            //if (player.orientation != GameVector2.ZERO && !player.isKnockedBack) // sets facing direction for throwing objects and stalker behavior
            //    player.facing = player.orientation;
            ////Console.WriteLine(player.orientation.X + " " + player.orientation.Y);
            //player.positionFuture = player.position + player.orientation * player.speed;
            //player.updateBoundingBox(player.position);
            ////player.updateSprite(player.positionPrevious, player.position);
        }

        public GameButton getButtonState(GameButtons Button) {
            return gButtons[Button];
        }

        [Obsolete("For Unity Port")]
        public void updateInputs() {
        }

        [Obsolete("For Unity Port")]
        public void updateController() {
        }

        [Obsolete("For Unity Port")]
        public void updateKeyboard()
        {
        }
    }
}