using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    class Dumb : Spectre
    {
        public Dumb(MapUnit[,] map, List<MapUnit> path, List<Door> doors, Player p, int id)
            : base(map, path, doors, p)
        {
            texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Spectres/Dumb/dumbSheet");
            spriteWidth = 90;
            spriteHeight = 90;
            spriteRowSize = 4;
            spriteColNum = 4;
            InitializeAnimations();

            updateBoundingBox(spriteWidth / 2, spriteHeight / 2);
            hearingRange = 500;
            hearingSphere = new BoundingSphere(new Vector3(position, 0), hearingRange);

            grunt = SoundManager.getCue(unityGameObject,SoundType.DUMB.DUMB_GRUNT.ToString());
            roar = SoundManager.getCue(unityGameObject,SoundType.DUMB.DUMB_ROAR.ToString());
            walk = SoundManager.getCue(unityGameObject,SoundType.DUMB.DUMB_FOOTSTEP.ToString());
            excorcismCue = SoundManager.getCue(unityGameObject,SoundType.DUMB.DUMB_EXCORCISED.ToString());
            InvestigateCue = SoundManager.getCue(unityGameObject,SoundType.DUMB.DUMB_ALERT.ToString());

            this.id = id;
            soundTimer = id * 3;
        }

        public override void BuildBehaviorMachine()
        {
            PatrolState tempPatrol = new PatrolState();
            OpenDoorState tempDoorOpen = new OpenDoorState();
            InvestigateState tempInvestigate = new InvestigateState();
            OpenDoorInvestigateState tempInvestigateDoor = new OpenDoorInvestigateState();
            AlertedState tempAlerted = new AlertedState();
            ChaseState tempChase = new ChaseState();
            ChaseAroundCornerState tempAroundCorner = new ChaseAroundCornerState();
            BreakDoorState tempBreakDoor = new BreakDoorState();
            PossessState tempPossess = new DumbPossessState();
            FleeState tempFlee = new FleeState();

            //Patrol Transitions
            tempPatrol.addTransition(new ToOpenDoorFromInvestigateTransition(tempInvestigateDoor));
            tempPatrol.addTransition(new ToInvestigateFromPatrolTransition(tempInvestigate));
            tempPatrol.addTransition(new ToAlertedTransition(tempAlerted));
            //tempPatrol.addTransition(new ToOpenDoor(tempDoorOpen));
            //tempDoorOpen.addTransition(new ToPatrolFromOpenDoorTransition(tempPatrol));

            //Investigate Transitions
            tempInvestigate.addTransition(new ToPatrolTransition(tempPatrol));
            //tempInvestigate.addTransition(new ToChaseTransition(tempChase));
            tempInvestigate.addTransition(new ToAlertedTransition(tempAlerted));
            tempInvestigate.addTransition(new ToPossessTransition(tempPossess));
            //tempInvestigate.addTransition(new ToOpenDoorFromInvestigateTransition(tempInvestigateDoor));
            tempInvestigate.addTransition(new ToInvestigateFromInvestigateTransition (tempInvestigate));
            //tempInvestigateDoor.addTransition(new ToPatrolFromOpenDoorTransition(tempInvestigate));

            //Alerted and Chase Transitions
            tempAlerted.addTransition(new ToChaseTransition(tempChase));
            //tempChase.addTransition(new ToInvestigateFromChaseTransition(tempInvestigate));
            tempChase.addTransition(new ToBreakDoorTransition(tempBreakDoor));
            tempChase.addTransition(new ToInvestigateFromChaseTransition(tempAroundCorner));
            tempChase.addTransition(new ToPossessTransition(tempPossess));

            //Around Corner Transitions
            tempAroundCorner.addTransition(new ToChaseFromAroundCornerTransition(tempChase));
            tempAroundCorner.addTransition(new ToInvestigateFromAroundCornerTransition(tempInvestigate));
            //tempAroundCorner.addTransition(new ToBreakDoorFromAroundCornerTransition(tempBreakDoor));
            tempAroundCorner.addTransition(new ToPatrolFromAroundCornerTransition(tempPatrol));
            tempBreakDoor.addTransition(new ToChaseFromBreakDoorTransition(tempChase));

            //Possess and Flee Transitions
            tempPossess.addTransition(new ToFleeTransition(tempFlee));
            tempPossess.addTransition(new ToPatrolFromPossessedNeutralizedTransition(tempPatrol));
            tempFlee.addTransition(new ToPatrolFromFleeTransition(tempPatrol));

            //Creating the Behavior Machine
            behaviorMachine = new StateMachine(tempPatrol);
            behaviorMachine.AddState(tempInvestigate);
            behaviorMachine.AddState(tempAlerted);
            behaviorMachine.AddState(tempChase);
            behaviorMachine.AddState(tempAroundCorner);
            behaviorMachine.AddState(tempPossess);
            behaviorMachine.AddState(tempFlee);
            behaviorMachine.AddState(tempDoorOpen);
            behaviorMachine.AddState(tempInvestigateDoor);
            behaviorMachine.AddState(tempBreakDoor);
            behaviorMachine.AddState(tempFlee);
        }

        public override void playAlertCue()
        {
            SoundManager.createSound(position, 500, 500, 1, SoundType.DUMB.DUMB_ROAR.ToString(), true, this);
            base.playAlertCue();
        }

        public override void Draw(object batch, object graphics)
        {
            //batch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp"), boundingBox, GameColor.White); // bounding box debug
            //batch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp"), new GameRectangle((int)position.X, (int)position.Y, 5, 5), GameColor.Red); //Debugging for spectre positon
            if (isChasing || playerVisible)
            {
                animation.texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Spectres/Dumb/dumb_run_spritesheet");
               //this. texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Spectres/Wrath/wrath_walk_spritesheet");
            }
            else
            {
                animation.texture = Game1.contentManager.Load<GameTexture> (@"Textures/Objects/Entity/Spectres/Dumb/dumbSheet");
            }

            SetSprite();
            if ((!inPlayer || !possessing) && behaviorMachine.getCurrenState().GetType() != typeof(Sonar.FleeState))
            {
                animation.Draw(batch, scale);
            }
        }

    }
}
