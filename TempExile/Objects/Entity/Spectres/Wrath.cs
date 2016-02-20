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
    class Wrath:Spectre
    {
        Texture2D textureAttack = Game1.contentManager.Load<Texture2D>("Textures/Objects/Entity/Spectres/Wrath/wrath_attack_spritesheet");

        public Wrath(MapUnit[,] map, List<MapUnit> path, List<Door> doors, Player player, int id)
            : base(map, path, doors, player)
        {
            texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Spectres/Wrath/wrath_walk_spritesheet");

            hearingRange = 500;
            hearingSphere = new BoundingSphere(new Vector3(position, 0), hearingRange);

            spriteWidth = 350;
            spriteHeight = 350;
            spriteRowSize = 4;
            spriteColNum = 4;

            InitializeAnimations();

            scale = 0.3375f;

            updateBoundingBox((int)((spriteWidth * scale) / 2), (int)((spriteHeight * scale) / 2));

            boundingBox.X *= (int)(scale + 10);
            boundingBox.Y *= (int)(scale + 10);

            homePosition = new Vector2(getCurrentUnit().x * MapUnit.MAX_SIZE, getCurrentUnit().y * MapUnit.MAX_SIZE);

            grunt = SoundManager.GetInstance().getCue(SoundManager.DUMB.GRUNT);
            roar = SoundManager.GetInstance().getCue(SoundManager.WRATH.ROAR);
            walk = SoundManager.GetInstance().getCue(SoundManager.DUMB.FOOTSTEP);
            excorcismCue = SoundManager.GetInstance().getCue(SoundManager.DUMB.EXCORCISED);
            InvestigateCue = SoundManager.GetInstance().getCue(SoundManager.DUMB.ALERT);

            this.id = id;
            this.initializeFootstepTimer(id * 5);

            isAttacking = false;
        }

        public override void playAlertCue()
        {
            SoundManager.GetInstance().createSound(position, 500, 500, 1, SoundManager.WRATH.ROAR, true, this);
            base.playAlertCue();
        }

        public override void BuildBehaviorMachine()
        {
            WrathWaitState tempWait = new WrathWaitState();
            InvestigateState tempInvestigate = new InvestigateState();
            AlertedState tempAlerted = new AlertedState();
            ChaseState tempChase = new ChaseState();
            ChaseAroundCornerState tempAroundCorner = new ChaseAroundCornerState();
            WrathAtkIdleState tempAtkIdle = new WrathAtkIdleState();
            BreakDoorState tempBreakDoor = new BreakDoorState();

            //Waiting in a spot transitions
            tempWait.addTransition(new ToInvestigateFromPatrolTransition(tempInvestigate));
            tempWait.addTransition(new ToAlertedTransition(tempAlerted));

            //Investigate Transitions
            tempInvestigate.addTransition(new ToAlertedTransition(tempAlerted));
            tempInvestigate.addTransition(new ToPatrolTransition(tempWait));

            //Alert/Chase/Attack Transitions
            tempAlerted.addTransition(new ToChaseTransition(tempChase));
            //tempChase.addTransition(new ToInvestigateFromChaseTransition(tempInvestigate));
            tempChase.addTransition(new ToBreakDoorTransition(tempBreakDoor));
            tempChase.addTransition(new ToInvestigateFromChaseTransition(tempAroundCorner));
            tempChase.addTransition(new ToPossessTransition(tempAtkIdle));

            tempAroundCorner.addTransition(new ToChaseFromAroundCornerTransition(tempChase));
            tempAroundCorner.addTransition(new ToInvestigateFromAroundCornerTransition(tempInvestigate));
            //tempAroundCorner.addTransition(new ToBreakDoorFromAroundCornerTransition(tempBreakDoor));
            tempAroundCorner.addTransition(new ToPatrolFromAroundCornerTransition(tempWait));
            tempBreakDoor.addTransition(new ToChaseFromBreakDoorTransition(tempChase));

            tempAtkIdle.addTransition(new ToPatrolTransition(tempInvestigate));
            
            //State Machine Creation
            behaviorMachine = new StateMachine(tempWait);
            behaviorMachine.AddState(tempInvestigate);
            behaviorMachine.AddState(tempAlerted);
            behaviorMachine.AddState(tempChase);
            behaviorMachine.AddState(tempAroundCorner);
            behaviorMachine.AddState(tempAtkIdle);
            behaviorMachine.AddState(tempBreakDoor);
        }

        public override void Draw(SpriteBatch batch, GraphicsDeviceManager graphics)
        {

            SetSprite();
            if (!inPlayer || !possessing)
            {
                //batch.Draw(Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Player/temp"), boundingBox, Color.White); // bounding box debug                
                animation.Draw(batch, scale);
                //batch.Draw(Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Player/temp"), new Rectangle((int)position.X, (int)position.Y, 5, 5), Color.Red); //Debugging for spectre positon
                //batch.Draw(Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Player/temp"), boundingBox, Color.White);
            }
        }

        public override void Update(GameTime gameTime, Player player)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            dmgCooldown += elapsedTime;
            abilityCooldown += elapsedTime;
            fleeTimer += elapsedTime;
            neutralizedTimer += elapsedTime;
            lastSoundTimer += elapsedTime;
            Collide(player.getBox(), Collidable.player);
            checkHearing(player.GetNoise());
            setWalkable();
            behaviorMachine.Update(this, player);
            if (this.GetType() != typeof(Siren) && active)
                move(gameTime);
            updateBoundingBox(new Vector2 (position.X + (int)(scale * 200), position.Y + (int)(scale * 200)));
            UpdateHearingSphere();
            animation.Update(gameTime, new Vector2((int)position.X - spriteWidth / 2, (int)position.Y - spriteHeight / 2));
            if (!possessing && this.GetType() != typeof(Stalker) && this.GetType() != typeof(Siren))
            {
                FootstepLightNoise(gameTime);
            }

            // Stops attacking animation when it reaches its end
            if (isAttacking && animation.Finished())
            {
                AttackEnd();
            }
        }

        protected override void InitializeAnimations()
        {
            animation = new AnimationCollection(texture, new Rectangle(0, 0, spriteWidth, spriteHeight));
            animation.add("DownWalk", 0, spriteColNum, Vector2.Zero, animationInterval, true);
            animation.add("UpWalk", 0, spriteColNum, new Vector2(0, spriteHeight * 1), animationInterval, true);
            animation.add("LeftWalk", 0, spriteColNum, new Vector2(0, spriteHeight * 2), animationInterval, true);
            animation.add("RightWalk", 0, spriteColNum, new Vector2(0, spriteHeight * 3), animationInterval, true);
            animation.add ("DownAttack", 0, spriteColNum, new Vector2 (0, spriteHeight), animationInterval * 15, true);
            animation.add ("UpAtack", 0, spriteColNum, new Vector2 (0, spriteHeight * 1), animationInterval * 15, true);
            animation.add ("LeftAttack", 0, spriteColNum, new Vector2 (0, spriteHeight * 2), animationInterval * 15, true);
            animation.add("RightAttack", 0, spriteColNum, new Vector2(0, spriteHeight * 3), animationInterval * 15, true);
            animation.RUN("RightWalk");
        }

        /// <summary>
        /// Steven Ekejiuba 5/28/2012
        /// Allows Wrath to switch to attacking animation
        /// </summary>
        public void Attack()
        {
            animation.texture = textureAttack;
            animation.RUN(animation.CurrentAnimation()); // *******************Might not be resetting
            isAttacking = true;
        }

        /// <summary>
        /// Steven Ekejiuba 5/28/2012
        /// Allows Wrath to transition to non-attacking animation
        /// </summary>
        protected void AttackEnd()
        {
            isAttacking = false;
            animation.texture = texture;
        }
    }
}
