using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;








namespace Sonar
{
    public class Player : Entity
    {

        public static Player player;
        private bool slowDown;
        public float stamina;
        public bool pooped;
        public List<Throwable> throwing = new List<Throwable>(); //List of throwable objects player is carrying.
        public bool throwCooldown;
        public WalkieTalkie walkieTalkie;
        public enum loudness : sbyte { silent, whispering, talking, yelling, exorcising, neutralizing };
        private loudness noise;
        public enum possessor : sbyte { none, Dumb, Stalker, Pride, Wrath };
        public String carriedPassword;
        public bool drawPass;
        private possessor possessedBy;
        private SpriteFont font;
        public GameTexture keyImage;
        
        //Confusion and Hallucination fields
        public bool messControls;
        private float soundPotential;
        public bool drawFake;
        public GameTexture fakeTexture;
        public float randLocX;
        public float randLocY;
        public int hallucinateWidth;
        public int hallucinateHeight;
        public bool safe;
        public bool beingChased;
        public bool beingSearchedFor;

        public Radio radio;
        public Door door;
        public float radius = 0;
        public BoundingSphere soundSphere;
        private float MicSensitivity = 1000;
        public bool canHide { get; set; } //is the player on a hideable tile
        public bool isHiding { get; set; } //is the player hiding
        public bool isKnockedBack; // when knocked back by Wrath
        private float knockBackTimer; // timer for knockback effect
        public GameVector2 facing;
        public ExcorcismPhrase exorcismPhrase;
        public bool pressedLockdown;
        public LockdownSwitch ldSwitch;
        private GameVector2 whereItShouldBe;
        private GameVector2 whereItShouldBeNew;
        public GameVector2 passPosition;
        private float moveOffsetX;
        private float moveOffsetY;
        public bool stayThere;
        public bool startDrawingPass = false;
        Sound breath;
        int randomTimer = 0;
        float soundDifference;

        Text soundPhrase;

        private Player()
        {
            this.health = 100f;

            texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/playerSheet");
            keyImage = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/KeyIcon");
            font = Game1.contentManager.Load<SpriteFont>("Fonts/Tutorial");
            spriteWidth = 80;
            spriteHeight = 80;
            spriteRowSize = 4;
            spriteColNum = 4;
            InitializeAnimations();
            boundingBox = new GameRectangle((int)position.X, (int)position.Y, spriteWidth / 2, spriteHeight / 2);
            slowDown = false;
            canHide = false;
            isHiding = false;
            isKnockedBack = false;
            throwCooldown = false;
            beingChased = false;
            beingSearchedFor = false;
            soundPotential = 1;
            drawFake = false;
            //Test additions for now.
            //throwing.Add(new Throwable(Content, position, 20, 5, null));
            //walkieTalkie = new WalkieTalkie();
            facing = GameVector2.Zero;   
            exorcismPhrase = new ExcorcismPhrase();
            knockBackTimer = 0;
            pressedLockdown = false;
            ldSwitch = null;
            this.id = 0;
            this.soundTimer = id * 3;
            soundSphere = new BoundingSphere(new Vector3(position, 0), radius);
            soundPhrase = new Text(VoiceEngine.getInstance().RandomPhrase(), position);
            soundPhrase.color = GameColor.White;
            breath = SoundManager.getCue(SoundManager.PLAYER.BREATHING);
        }

        protected override void InitializeAnimations()
        {
            animationInterval = 150;
            animation = new AnimationCollection(texture, new GameRectangle(0, 0, spriteWidth, spriteHeight));
            animation.add("DownWalk", 0, spriteColNum, GameVector2.Zero, animationInterval, true);
            animation.add("UpWalk", 0, spriteColNum, new GameVector2(0, spriteHeight * 1), animationInterval, true);
            animation.add("LeftWalk", 0, spriteColNum, new GameVector2(0, spriteHeight * 2), animationInterval, true);
            animation.add("RightWalk", 0, spriteColNum, new GameVector2(0, spriteHeight * 3), animationInterval, true);
            animation.add("DownRun", 0, spriteColNum, new GameVector2(0, spriteHeight * 4), animationInterval, true);
            animation.add("UpRun", 0, spriteColNum, new GameVector2(0, spriteHeight * 5), animationInterval, true);
            animation.add("LeftRun", 0, spriteColNum, new GameVector2(0, spriteHeight * 6), animationInterval, true);
            animation.add("RightRun", 0, spriteColNum, new GameVector2(0, spriteHeight * 7), animationInterval, true);
            animation.add("Hide", 0, spriteColNum, new GameVector2 (0, spriteHeight * 8), animationInterval, true);
            animation.add("RightStealth", 0, spriteColNum, new GameVector2(0, spriteHeight * 9), animationInterval, true);
            animation.add("LeftStealth", 0, spriteColNum, new GameVector2(0, spriteHeight * 10), animationInterval, true);
            animation.add("UpStealth", 0, spriteColNum, new GameVector2(0, spriteHeight * 11), animationInterval, true);
            animation.add("DownStealth", 0, spriteColNum, new GameVector2(0, spriteHeight * 12), animationInterval, true);
            animation.RUN("RightWalk");
        }

        public static Player getInstance() {
            if (player == null) player = new Player();
            return player;
        }

        public float gethealth()
        {
            return this.health;
        }

        public void LoseHealth(int i)
        {
            this.health -= i;
            VisionManager.blurEffect.activate();
            SoundManager.getCue(SoundManager.PLAYER.PLAYER_HIT).Play();
        }

        public void ResetHealth()
        {
            this.health = 100f;
            stamina = 300;
        }

        public bool isDead()
        {
            if (this.health <= 0  || Keyboard.GetState().IsKeyDown(Keys.K))
            {
                noise = loudness.silent;
                if (exorcismPhrase.ACTIVE)
                    exorcismPhrase.ACTIVE = false;
                return true;
            }
            else return false;
        }

        public void Initialize(GameVector2 Position, GameVector2 Orientation, float Speed)
        {
            rootPos = Position;
            position = Position;
            positionFuture = Position;
            orientation = Orientation;
            boundingBox.X = (int)Position.X;
            boundingBox.Y = (int)Position.Y;
            orientation.Normalize();
            speed = Speed;
            exorcismPhrase.ACTIVE = false;
            noise = loudness.silent;
            possessedBy = possessor.none;
            slowDown = false;
            walkieTalkie = null;
            radio = null;
            door = null;
            carriedPassword = null;
            startDrawingPass = false;
            stayThere = false;
            isMoving = false;
            resetHallucinate();
            resetConfusion();
            throwing.Clear();
            ldSwitch = null;
            pressedLockdown = false;
            isKnockedBack = false;
            health = 100;
            stamina = 300;
            pooped = false;
        }

        /// <summary>
        /// Corey - 2/6
        /// Gets the throwing item to use.
        /// </summary>
        /// <param name="t"></param>
        public void obtainThrowing(Throwable t)
        {
            throwing.Add(t);
        }

        public void KnockBack(GameTime gameTime)
        {
            knockBackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (knockBackTimer < .5)
            {
                //orientation = facing;
                
            }
            else
            {
                //facing *= -1;
                speed /= 2;
                knockBackTimer = 0;
                isKnockedBack = false;
            }
        }

        public void UpdateSoundSphere()
        {
            soundSphere.Center.X = position.X;
            soundSphere.Center.Y = position.Y;
            soundSphere.Radius = radius;
        }

        public override void Update(GameTime gameTime)
        {
             radius = VoiceEngine.getInstance().VOLUME * Game1.micSens;   
             /*if (VoiceEngine.getInstance().SpokenWord() == soundPhrase.word)
             {
                 radius = 300;
                 soundPhrase.setWord(VoiceEngine.getInstance().RandomPhrase());
             }
             else radius = 10;
             soundPhrase.position.X = position.X;
             soundPhrase.position.Y = position.Y + 32;//**/

            if (Keyboard.GetState().IsKeyDown(Keys.L) || ControllerInput.getInstance().getButtonState(Buttons.RightShoulder).Held) radius = 500;
            if (isHiding) {
                radius = radius / 3f;
            }
            else if (isStealthing && !isHiding) {
                radius = radius / 6f;
            }
            setNoise(radius);
            //if (noise == loudness.exorcising) noise = loudness.yelling;



            //Console.WriteLine(noise);
            // Sets up visual representation of the player's sound via the microphone
            this.storeSound(radius * 5);
            this.updateSound();
             
            UpdateSoundSphere();

            //Goes through the throwing items for updating.
            #region Throwing Update
            for (int index = 0; index < throwing.Count; index++)
            {
                if (throwing[index].getFlightTime() > 0)
                {
                    throwing[index].Update(gameTime);
                }
            }
            #endregion


            exorcismPhrase.Update(gameTime);

            if (possessedBy != possessor.none && !exorcismPhrase.ACTIVE && possessedBy != possessor.Pride)
            {
                string temp = VoiceEngine.getInstance().CalibratedPhrase();
                exorcismPhrase.initialize(temp, position, VoiceEngine.getInstance().excorcismAudio(temp));
                VisionManager.setVisionColor(GameColor.DarkRed);

            }

            if (exorcismPhrase.ACTIVE && possessedBy != possessor.none && possessedBy != possessor.Pride)
            {
                if (exorcismPhrase.CheckSpeech(VoiceEngine.getInstance().SpokenWord()) && VoiceEngine.getInstance().getTimeSinceSpoken() != 0)
                {
                    //exorcismPhrase.ACTIVE = false;
                    noise = loudness.neutralizing;
                    //slowDown = false;
                    //VisionManager.setVisionColor(GameColor.CornflowerBlue);
                    //VisionManager.addVisionPoint(position, 300f, true);
                    //possessedBy = possessor.none;
                }
            }

            if (isKnockedBack)
                KnockBack(gameTime);

            animation.Update(gameTime, new GameVector2((int)position.X - spriteWidth / 2, (int)position.Y - spriteHeight / 2));
            FootstepLightNoise(gameTime);

            if (isRunning)
            {
                stamina -= 1;
                if (stamina == 0)
                {
                    pooped = true;
                }
            }
            else if (stamina < 300)
            {
                stamina += 1;
                if (stamina == 300 && pooped == true)
                {
                    pooped = false;
                }
            }
            if (!pooped) breath.SetVariable("Stamina", stamina / 3); // 3 is the max stamina divided by 100
            else breath.SetVariable("Stamina", (stamina / 3) - (stamina/2)/3);
            SoundManager.Play(ref breath, SoundManager.PLAYER.BREATHING);

            #region Moving Password
            if (carriedPassword != null) {
                if (startDrawingPass) {
                    //passPosition = position;
                    whereItShouldBe = new GameVector2(Game1.screenWidth, 0) - new GameVector2(Player.getInstance().carriedPassword.Length * 24, 0);
                    moveOffsetX = (whereItShouldBe.X - passPosition.X) / 60;
                    moveOffsetY = (whereItShouldBe.Y - passPosition.Y) / 60;
                    //startDrawingPass = false;
                    randomTimer++;
                    stayThere = false;
                    if (randomTimer > 90) {
                        randomTimer = 0;
                        startDrawingPass = false;
                    }
                }
                else {
                    //whereItShouldBeNew = Game1.camera.topRight() - new GameVector2(Player.getInstance().carriedPassword.Length * 24, 0);
                    if (!stayThere) {
                       // if (whereItShouldBeNew != whereItShouldBe) {
                            //whereItShouldBe = whereItShouldBeNew;
                            //moveOffsetX = (whereItShouldBe.X - passPosition.X) / 20;
                            //moveOffsetY = (whereItShouldBe.Y - passPosition.Y) / 20;
                        //}
                        if (passPosition != whereItShouldBe) {
                            if (passPosition.Y > whereItShouldBe.Y) {
                                passPosition.Y += moveOffsetY;
                            }
                            else {
                                passPosition.Y = whereItShouldBe.Y;
                                stayThere = true;
                            }
                            if (passPosition.X < whereItShouldBe.X) {
                                passPosition.X += moveOffsetX;
                            }
                            else {
                                passPosition.X = whereItShouldBe.X;
                                stayThere = true;
                            }
                        }
                    }
                    else {
                        passPosition = whereItShouldBe;
                    }
                }
            }
            #endregion

        }

        public override void Draw(object spriteBatch)
        {
            SetSprite();
            font = Game1.contentManager.Load<SpriteFont>("Fonts/Gothic");
            /*
            soundPhrase.Draw(spriteBatch);
            //*/
            // spriteBatch.Draw(texture, position, null, GameColor.White, 0, new GameVector2(0, 21), 1, SpriteEffects.None, 0);

            if (drawFake) {
                spriteBatch.Draw(fakeTexture, new GameVector2(randLocX - hallucinateWidth / 2, randLocY - hallucinateHeight / 2),
                                  new GameRectangle(0, 0, hallucinateWidth, hallucinateHeight), GameColor.White);
            }
            if (!isHiding)
            {
                //spriteBatch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp"), boundingBox, GameColor.White); //Debugging, player bounding box                
                /*spriteBatch.Draw(texture, new GameVector2((int)position.X - spriteWidth / 2, (int)position.Y - spriteHeight / 2),
                                  new GameRectangle((int)currSprite.X * (spriteWidth), (int)currSprite.Y * (spriteHeight), spriteWidth, spriteHeight), GameColor.CornflowerBlue);*/
                animation.Draw(spriteBatch, 1);


                // Used for Sound Testing spriteBatch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp"), new GameRectangle((int)(position.X + radius), (int)position.Y, 5, 5), GameColor.Red); //Debugging, player position
            }
            //Drawing the throwing items long as there are things.
            if (throwing.Count != 0)
            {
                foreach (Throwable t in throwing)
                {
                    if (t.getFlightTime() > 0)
                    {
                        t.Draw(spriteBatch);
                    }
                }
            }
        }

        public void setPos(GameVector2 v)
        {
            position = v;
            updateBoundingBox(position);
        }

        // Animate the sprites
        public override void updateSprite(GameVector2 prepos, GameVector2 pos)
        {
            float dir_x = pos.X - prepos.X;
            float dir_y = pos.Y - prepos.Y;

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
                isRunning = false;
            }
        }

        /// <summary>
        /// Derrick - 3/12
        /// Used by spectre to reveal the player if found while hiding
        /// </summary>
        public void Reveal()
        {
            if (isHiding)
                isHiding = false;
        }

        public void SetPossessor(possessor p)
        {
            possessedBy = p;
        }

        public possessor GetPossessor() {
            return possessedBy;
        }

        /// <summary>
        /// Corey - 2/28
        /// Does a random action associated with the stalker.
        /// </summary>
        /// <param name="randAction"></param>
        public void ConfusionEffect() {
            /*switch (randAction) {
                //Sets everything normal
                case 0:
                    messControls = false;
                    soundPotential = 1f;
                    //Console.WriteLine("Normal");
                break;
                //Halves the player sound
                case 1:
                    soundPotential = 2f;
                    messControls = false;
                    //Console.WriteLine("Half");
                break;
                //Doubles player sound
                case 2:
                    soundPotential = .5f;
                    messControls = false;
                    //Console.WriteLine("Double");
                break;
                //Throws the object carried by the player
                case 3:
                    if (throwing.Count != 0 && !throwCooldown) {
                        throwing[0].throwObject(this);
                    }
                    messControls = false;
                    soundPotential = 1f;
                    //Console.WriteLine("Throw object");
                break;
                //Messes with controls
                default:
                    messControls = true;
                    soundPotential = 1f;
                    //Console.WriteLine("Opposite Controls");
                break;
            }*/
            messControls = true;
            soundPotential = .75f;
        }

        /// <summary>
        /// Corey - 2/28
        /// Resets the confusion values back.
        /// </summary>
        public void resetConfusion() {
            messControls = false;
            soundPotential = 1f;
            speed = 2;
        }

        /// <summary>
        /// Corey - 3/3
        /// Does the random hallucination effects for Pride
        /// </summary>
        /// <param name="randValue">Chooses randomly which hallucination effect to do</param>
        public void Hallucinate(int randValue) {
            switch (randValue) {
                case 0:
                    //Does nothing, to possibly throw you off.
                    drawFake = false;
                break;
                case 1:
                //Flashes a random spectre in front of the player.
                    drawFake = true;
                    randLocY = Game1.random.Next(0, 2);
                    randLocX = Game1.random.Next(170, 230);
                    //Fake Wrath
                    if (randLocY == 0) {
                        Console.WriteLine("Wrath");
                        randLocY = this.position.Y + (this.facing.Y * randLocX);
                        randLocX = this.position.X + (this.facing.X * randLocX);
                        fakeTexture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Spectres/Wrath/wrath_walk_spritesheet");
                        //VisionManager.addVisionPoint(new GameVector2(player.position.X + (player.facing.X * randLocX), player.position.Y + (player.facing.Y * randLocY)), 450, false);
                        SoundManager.createSound(new GameVector2(randLocX, randLocY), 400, 400, 1, SoundManager.WRATH.FOOTSTEP, false);
                        hallucinateHeight = 350;
                        hallucinateWidth = 350;
                    }
                    //Fake Stalker
                    else {
                        Console.WriteLine("Stalker");
                        randLocY = this.position.Y + (this.facing.Y * randLocX);
                        randLocX = this.position.X + (this.facing.X * randLocX);
                        fakeTexture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Spectres/Stalker/stalkerSheet");
                        //VisionManager.addVisionPoint(new GameVector2(player.position.X + (player.facing.X * randLocX), player.position.Y + (player.facing.Y * randLocY)), 300, false);
                        SoundManager.createSound(new GameVector2(randLocX, randLocY), 300, 300, 1, SoundManager.WRATH.FOOTSTEP, false);
                        hallucinateHeight = 100;
                        hallucinateWidth = 100;
                    }
                break;
                case 5:
                    //This is for hearing footsteps (Logic is in the state machine class)
                    drawFake = false;
                break;
                default:
                    //Currently for drawing fake hiding spots.
                    Console.WriteLine("Hiding Spots");
                    drawFake = true;
                    randLocX = Game1.random.Next(0, 2) == 0 ? (-1 * Game1.random.Next(90, 125)) : (Game1.random.Next(90, 125));
                    randLocX += this.position.X;
                    randLocY = Game1.random.Next(0, 2) == 0 ? (-1 * Game1.random.Next(90, 125)) : (Game1.random.Next(90, 125));
                    randLocY += this.position.Y;
                    hallucinateHeight = MapUnit.MAX_SIZE;
                    hallucinateWidth = MapUnit.MAX_SIZE;
                    fakeTexture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Kitchen/cupboard");
                break;
            }
        }

        /// <summary>
        /// Corey - 3/4
        /// Reset the hallucination effects.
        /// </summary>
        public void resetHallucinate() {
            drawFake = false;
        }

        /// <summary>
        /// Steven Ekejiuba - 1/27/'12
        /// Determines if for any reason, the players' speed should change
        /// Modified by Joshua Ray, Kept like so in case it is needed later
        /// </summary>
        public void speedModifier()
        {

        }

        /// <summary>
        /// Steven Ekejiuba - 1/27/'12
        /// Slow down player movement
        /// </summary>
        public void decreaseMovementSpeed()
        {
            slowDown = true;
        }

        /// <summary>
        /// Steven Ekejiuba - 2/1/'12
        /// Return slowDown
        /// </summary>
        public bool getSlowDown()
        {
            return slowDown;
        }

        /// <summary>
        /// Evan Weintraub - 3/1/12
        /// Sets slowDown
        /// </summary>
        public void setSlowDown(bool value)
        {
            slowDown = value;
        }

        /// <summary>
        /// Steven Ekejiuba - 2/11/'12
        /// Return the level of noise the player is making
        /// </summary>
        /// <returns></returns>
        public loudness GetNoise()
        {
            return noise;
        }

        /// <summary>
        /// Evan Weintraub - 3/5/12
        /// Gets the current ExorcismPhrase
        /// </summary>
        public ExcorcismPhrase getExcorcismPhrase()
        {
            return exorcismPhrase;
        }

        /// <summary>
        /// Evan Weintraub - 4/18/12
        /// Refactored code from above.
        /// </summary>
        protected void FootstepLightNoise(GameTime time)
        {
            if (isMoving)
            {
                int timer = updateFootstepTimer();
                Sound temp = SoundManager.getCue(SoundManager.PLAYER.FOOTSTEP_TILE);
                temp.SetVariable("Muffled", 0);
                if (timer == 1)
                {
                    if (isRunning)
                        SoundManager.createSound(position + new GameVector2(orientation.Y * 10f, orientation.X * 10f) + orientation * 25f, 500, 300, 1, temp, true);
                    else if (isStealthing)
                    {
                        temp.SetVariable("Muffled", 1);
                        SoundManager.createSound(position + new GameVector2(orientation.Y * 10f, orientation.X * 10f) + orientation * 25f, 150, 75, 1, temp, true);
                    }
                    else
                        SoundManager.createSound(position - new GameVector2(orientation.Y * 10f, orientation.X * 10f) + orientation * 25f, 300, 150, 1, temp, true);
                }
                else if (timer == 2)
                {
                    if (isRunning)
                        SoundManager.createSound(position - new GameVector2(orientation.Y * 10f, orientation.X * 10f) + orientation * 25f, 500, 300, 1, temp, true);
                    else if (isStealthing)
                    {
                        temp.SetVariable("Muffled", 1);
                        SoundManager.createSound(position - new GameVector2(orientation.Y * 10f, orientation.X * 10f) + orientation * 25f, 150, 75, 1, temp, true);
                    }
                    else
                        SoundManager.createSound(position - new GameVector2(orientation.Y * 10f, orientation.X * 10f) + orientation * 25f, 300, 150, 1, temp, true);
                }
            }
            else { footstepTimer = 55; }
        }

        /// <summary>
        /// Steven Ekejiuba - 2/11/'12
        /// Determine if the amount of sound the player is making
        /// </summary>
        /// <param name="radius"></param>
        public void setNoise(float radius)
        {
            soundDifference = Game1.micSens / 300f;

            if (radius <= 20 * soundDifference)
            {
                //Console.WriteLine("silent");
                noise = loudness.silent;
            }
            else if (radius <= 80 * soundDifference)
            {
                //Console.WriteLine("whispering");
                noise = loudness.whispering;
            }
            else if (radius <= 120 * soundDifference)
            {
                //Console.WriteLine("talking");
                noise = loudness.talking;
            }
            else if (radius <= 200 * soundDifference)
            {
                //Console.WriteLine("yelling");
                noise = loudness.yelling;
            }
            else
            {
                //Console.WriteLine("exorcise");
                noise = loudness.exorcising;
            }
        
            //SoundManager.playSoundFX(position, radius);
        }

        public void setMicSens(int newMicSensitivity)
        {
            MicSensitivity = newMicSensitivity;
        }

        #region Sound Controls
        public void stopSounds() {
            exorcismPhrase.ACTIVE = false;
            SoundManager.Stop(ref breath);
        }
        public void pauseSounds()
        {
            exorcismPhrase.pause();
            SoundManager.Puase(ref breath);
        }
        public void unpauseSounds()
        {
            exorcismPhrase.unpause();
            SoundManager.Play(ref breath, SoundManager.PLAYER.BREATHING);
        }
        #endregion
       
    }
}

