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
    class Stalker:Spectre
    {
        Texture2D runTex;

        public Stalker(MapUnit[,] map, List<MapUnit> path, List<Door> doors, Player player, int id)
            : base(map, path, doors, player)
        {
            texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Spectres/Stalker/stalker_walk_spritesheet");
            runTex = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Spectres/Stalker/stalker_run_spritesheet");

            hearingRange = 500;
            hearingSphere = new BoundingSphere(new Vector3(position, 0), hearingRange);

            spriteWidth = 80;
            spriteHeight = 80;
            spriteRowSize = 4;
            spriteColNum = 4;
            InitializeAnimations();
            updateBoundingBox(spriteWidth / 2, spriteHeight / 2);


            grunt = SoundManager.GetInstance().getCue(SoundManager.DUMB.GRUNT);
            roar = SoundManager.GetInstance().getCue(SoundManager.WRATH.ROAR);
            walk = SoundManager.GetInstance().getCue(SoundManager.DUMB.FOOTSTEP);
            excorcismCue = SoundManager.GetInstance().getCue(SoundManager.DUMB.EXCORCISED);
            InvestigateCue = SoundManager.GetInstance().getCue(SoundManager.DUMB.ALERT);


            this.id = id;

        }

        public override void playAlertCue()
        {
            SoundManager.GetInstance().createSound(position, 500, 500, 1, SoundManager.STALKER.ROAR, true, this);
            base.playAlertCue();
        }

        public override void BuildBehaviorMachine()
        {
            WanderState tempWander = new WanderState();
            OpenDoorState tempDoorOpen = new OpenDoorState();
            InvestigateState tempInvestigate = new InvestigateState();
            OpenDoorInvestigateState tempInvestigateDoor = new OpenDoorInvestigateState();
            AlertedState tempAlerted = new AlertedState();
            ChaseState tempChase = new ChaseState();
            ChaseAroundCornerState tempAroundCorner = new ChaseAroundCornerState();
            StalkState tempStalk = new StalkState();
            StalkerConfusionState tempConfuse = new StalkerConfusionState();
            FleeState tempFlee = new FleeState();

            //Wander Transitions
            tempWander.addTransition(new ToOpenDoorFromInvestigateTransition(tempInvestigateDoor));
            tempWander.addTransition(new ToInvestigateFromPatrolTransition(tempInvestigate));
            tempWander.addTransition(new ToAlertedTransition(tempAlerted));
            //tempWander.addTransition(new ToOpenDoor(tempDoorOpen));
            tempDoorOpen.addTransition(new ToPatrolFromOpenDoorTransition(tempWander));

            //Investigate Transitions
            tempInvestigate.addTransition(new ToPatrolTransition(tempWander));
            tempInvestigate.addTransition(new ToAlertedTransition(tempAlerted));
            tempInvestigate.addTransition(new ToPossessTransition(tempConfuse));
            //tempInvestigate.addTransition(new ToOpenDoorFromInvestigateTransition(tempInvestigateDoor));
            tempInvestigate.addTransition(new ToInvestigateFromInvestigateTransition(tempInvestigate));
            tempInvestigateDoor.addTransition(new ToPatrolFromOpenDoorTransition(tempInvestigate));

            //Chase and Alerted Transitions
            tempAlerted.addTransition(new ToChaseTransition(tempChase));
            tempChase.addTransition(new ToPossessTransition(tempConfuse));
            tempChase.addTransition(new ToBreakDoorTransition(tempDoorOpen));
            tempChase.addTransition(new ToInvestigateFromChaseTransition(tempAroundCorner));
            tempChase.addTransition(new StalkerToStalkTransition(tempStalk));
            tempStalk.addTransition(new ToBreakDoorTransition(tempDoorOpen));
            tempStalk.addTransition(new StalkerToInvestigateFromStalkTransition(tempInvestigate));
            tempAroundCorner.addTransition(new ToChaseFromAroundCornerTransition(tempChase));
            tempAroundCorner.addTransition(new ToInvestigateFromAroundCornerTransition(tempInvestigate));
            tempAroundCorner.addTransition(new ToPatrolFromAroundCornerTransition(tempWander));
            tempDoorOpen.addTransition(new ToChaseFromBreakDoorTransition(tempInvestigate));

            //Possess and Flee
            tempConfuse.addTransition(new ToFleeTransition(tempFlee));
            tempConfuse.addTransition(new ToPatrolFromPossessedNeutralizedTransition(tempWander));
            tempFlee.addTransition(new ToPatrolFromFleeTransition(tempWander));

            //State Machine Creation
            behaviorMachine = new StateMachine(tempWander);
            behaviorMachine.AddState(tempInvestigate);
            behaviorMachine.AddState(tempAlerted);
            behaviorMachine.AddState(tempChase);
            behaviorMachine.AddState(tempStalk);
            behaviorMachine.AddState(tempConfuse);
            behaviorMachine.AddState(tempDoorOpen);
            behaviorMachine.AddState(tempInvestigateDoor);
            behaviorMachine.AddState(tempFlee);
        }

        /// <summary>
        /// Steven Ekejiuba 6/7/2012
        /// Allows animation to change during chase
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="graphics"></param>
        public override void Draw(SpriteBatch batch, GraphicsDeviceManager graphics)
        {
            if (isChasing)
            {
                animation.texture = runTex;
            }
            else
            {
                animation.texture = texture;
            }
            SetSprite();
            if ((!inPlayer || !possessing) && behaviorMachine.getCurrenState().GetType() != typeof(Sonar.FleeState))
            {
                animation.Draw(batch, scale);
                //batch.Draw(texture, boundingBox, Color.Red);
                //batch.Draw(Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Player/temp"), boundingBox, Color.White); // bounding box debug
                //batch.Draw(Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Player/temp"), new Rectangle((int)position.X, (int)position.Y, 5, 5), Color.Red); //Debugging for spectre positon
                //if (target != null)
                //batch.Draw(Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Player/temp"), new Rectangle((int)target.GetPosition().X - 24, (int)target.GetPosition().Y - 24, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE), Color.Red);
            }
        }
    }
}
