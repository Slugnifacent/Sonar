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
    class Pride:Spectre
    {
        public Pride(MapUnit[,] map, List<MapUnit> path, List<Door> doors, Player player, int id)
            : base(map, path, doors, player)
        {
            texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Entity/Spectres/Pride/pride_spritesheet2");
            hearingRange = 500;
            hearingSphere = new BoundingSphere(new Vector3(position, 0), hearingRange);

            spriteWidth = 72;
            spriteHeight = 90;
            spriteRowSize = 4;
            spriteColNum = 4;

            InitializeAnimations();

            updateBoundingBox(spriteWidth / 2, spriteHeight / 2);
            scale = 1f;


            grunt = SoundManager.GetInstance().getCue(SoundManager.DUMB.GRUNT);
            roar = SoundManager.GetInstance().getCue(SoundManager.WRATH.ROAR);
            walk = SoundManager.GetInstance().getCue(SoundManager.DUMB.FOOTSTEP);
            excorcismCue = SoundManager.GetInstance().getCue(SoundManager.DUMB.EXCORCISED);
            InvestigateCue = SoundManager.GetInstance().getCue(SoundManager.DUMB.ALERT);


            this.id = id;

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
            PrideHallucinateState tempHallucinate = new PrideHallucinateState();
            FleeState tempFlee = new FleeState();

            //Wander Transitions
            //tempWander.addTransition(new ToOpenDoorFromInvestigateTransition(tempInvestigateDoor));
            tempWander.addTransition(new ToInvestigateFromPatrolTransition(tempInvestigate));
            tempWander.addTransition(new ToAlertedTransition(tempAlerted));
            //tempWander.addTransition(new ToOpenDoor(tempDoorOpen));
            //tempDoorOpen.addTransition(new ToPatrolFromOpenDoorTransition(tempWander));

            //Investigate Transitions
            tempInvestigate.addTransition(new ToPatrolTransition(tempWander));
            tempInvestigate.addTransition(new ToAlertedTransition(tempAlerted));
            tempInvestigate.addTransition(new ToPossessTransition(tempHallucinate));
            //tempInvestigate.addTransition(new ToOpenDoorFromInvestigateTransition(tempInvestigateDoor));
            tempInvestigate.addTransition(new ToInvestigateFromInvestigateTransition(tempInvestigate));
            tempInvestigateDoor.addTransition(new ToPatrolFromOpenDoorTransition(tempInvestigate));
            
            //Chase and Alerted Transitions
            tempAlerted.addTransition(new ToChaseTransition(tempChase));
            tempChase.addTransition(new ToPossessTransition(tempHallucinate));
            tempChase.addTransition(new ToBreakDoorTransition(tempDoorOpen));
            tempChase.addTransition(new ToInvestigateFromChaseTransition(tempAroundCorner));
            tempAroundCorner.addTransition(new ToChaseFromAroundCornerTransition(tempChase));
            tempAroundCorner.addTransition(new ToInvestigateFromAroundCornerTransition(tempInvestigate));
            tempAroundCorner.addTransition(new ToPatrolFromAroundCornerTransition(tempWander));
            tempDoorOpen.addTransition(new ToChaseFromBreakDoorTransition(tempInvestigate));
            
            //Possess and Flee
            tempHallucinate.addTransition(new ToFleeTransition(tempFlee));
            tempHallucinate.addTransition(new ToPatrolFromPossessedNeutralizedTransition(tempWander));
            tempFlee.addTransition(new ToPatrolFromFleeTransition(tempWander));
            
            //State Machine Creation
            behaviorMachine = new StateMachine(tempWander);
            behaviorMachine.AddState(tempInvestigate);
            behaviorMachine.AddState(tempAlerted);
            behaviorMachine.AddState(tempChase);
            behaviorMachine.AddState(tempAroundCorner);
            behaviorMachine.AddState(tempHallucinate);
            behaviorMachine.AddState(tempDoorOpen);
            behaviorMachine.AddState(tempInvestigateDoor);
            behaviorMachine.AddState(tempFlee);
        }
    }
}