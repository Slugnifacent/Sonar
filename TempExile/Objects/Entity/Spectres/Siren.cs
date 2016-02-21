using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;








namespace Sonar
{
    class Siren : Spectre
    {
        int snoreTimer;
        AudioEmitter emitter;
        Sound Snore;
        Sound Scream;
        Sound test;
        Floor drawTile;

        public Siren(MapUnit[,] map, List<MapUnit> path, List<Door> doors, Player p, int id)
            : base(map, path, doors, p)
        {
            texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Spectres/Siren/siren_spritesheet");
            hearingRange = 200;
            hearingSphere = new BoundingSphere(new Vector3(position, 0), hearingRange);

            //position.Y += 8;
            position.Y += 3;
            position.X += MapUnit.MAX_SIZE * 3/4;
            positionPrevious = position;

            spriteWidth = 60;
            spriteHeight = 90;
            spriteRowSize = 4;
            spriteColNum = 4;
            InitializeAnimations();
            //updateBoundingBox(position);
            updateBoundingBox(spriteWidth / 2, spriteHeight / 2);

            Snore = SoundManager.getCue(SoundManager.SIREN.SNORE);
            Scream = SoundManager.getCue(SoundManager.SIREN.SCREAM);
            emitter = new AudioEmitter();
            emitter.Position = new Vector3(position.X,0, position.Y);
            isMoving = false;

            this.id = id;
            soundTimer = id * 3;
            test = new Sound(position, 500, SoundManager.getCue(SoundManager.SIREN.SCREAM), true, this);
        }

        public override void stopAudio()
        {
            SoundManager.Stop(ref Snore);
            SoundManager.Stop(ref Scream);
        }

        public override void pauseAudio()
        {
            SoundManager.Puase(ref Snore);
            SoundManager.Puase(ref Scream);
            base.pauseAudio();
        }

        public override void unpauseAudio()
        {
            if (Snore.IsPaused) SoundManager.Play3D(ref Snore, emitter, SoundManager.SIREN.SNORE);
            if (Scream.IsPaused) SoundManager.Play3D(ref Scream, emitter, SoundManager.SIREN.SCREAM);
            base.unpauseAudio();
        }

        protected override void InitializeAnimations()
        {
            animationInterval = 200;
            animation = new AnimationCollection(texture, new GameRectangle(0, 0, spriteWidth, spriteHeight));
            animation.add("SleepD", 0, spriteColNum, GameVector2.Zero, animationInterval, true);
            animation.add("AwakeD", 0, spriteColNum, new GameVector2(0, spriteHeight * 2), animationInterval, true);
            animation.add("SleepU", 0, spriteColNum, new GameVector2(0, spriteHeight * 3), animationInterval, true);
            animation.add("AwakeU", 0, spriteColNum, new GameVector2(0, spriteHeight * 5), animationInterval, true);
            animation.add("SleepL", 0, spriteColNum, new GameVector2(0, spriteHeight * 6), animationInterval, true);
            animation.add("AwakeL", 0, spriteColNum, new GameVector2(0, spriteHeight * 8), animationInterval, true);
            animation.add("SleepR", 0, spriteColNum, new GameVector2(0, spriteHeight * 9), animationInterval, true);
            animation.add("AwakeR", 0, spriteColNum, new GameVector2(0, spriteHeight * 11), animationInterval, true);
            animation.RUN("SleepU");
        }

        public override void BuildBehaviorMachine()
        {
            //Only 2 states, goes between sleeping and not sleeping.
            SleepState tempSleep = new SleepState();
            AwakeState tempAwake = new AwakeState();
            
            //Only 2 transitions back and forth
            tempSleep.addTransition(new WakeUpTransition(tempAwake));
            tempAwake.addTransition(new ToSleepTransition(tempSleep));

            //State Machine Creation
            behaviorMachine = new StateMachine(tempSleep);
            behaviorMachine.AddState(tempAwake);
        }

        public override void Update(GameTime time, Player player)
        {
            position = positionPrevious;
            snoreTimer += time.ElapsedGameTime.Milliseconds;
            SoundManager.cueUpdate(ref Scream, emitter);
            SoundManager.cueUpdate(ref Snore, emitter);
            base.Update(time, player);
        }

        public override void SetSprite()
        {
            // Update current frame
            updateSprite(positionPrevious, position);
            updateBoundingBox(new GameVector2(position.X, position.Y + (MapUnit.MAX_SIZE / 2)));
        }

        // Animate the sprites
        public override void updateSprite(GameVector2 prepos, GameVector2 pos)
        {
            float dir_x = pos.X - prepos.X;
            float dir_y = pos.Y - prepos.Y;

            if (NextFrame())
            {
                currSprite.X = (currSprite.X + 1) % spriteRowSize;
            }
        }

        // regular Snore 
        public override void playCue()
        {
            SoundManager.Stop(ref Scream);
        }

        // Investigation Sound 
        public override void playInvestigationCue()
        {
        }


        // Alerted Scream
        public override void playAlertCue()
        {
            this.storeSound(Game1.random.Next(400, 400));
            SoundManager.Play3D(ref Scream, emitter, SoundManager.SIREN.SCREAM);
            SoundManager.createSound(position, 500, 500, 1,null, true, this);
            //SoundManager.Stop(ref Snore);
            currSprite.Y = 5; // I should add logic to find which way the Siren is facing (Steven)**********
            StartAnimation();
        }

        public void SetDrawTile(Floor t)
        {
            drawTile = t;
        }

        public override void Draw(object spriteBatch)
        {
            drawTile.Draw(spriteBatch);
        }
    }
}

