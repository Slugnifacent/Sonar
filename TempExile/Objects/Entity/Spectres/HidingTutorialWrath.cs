using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    class HidingTutorialWrath : Spectre
    {
        public HidingTutorialWrath(MapUnit[,] map, List<MapUnit> path, List<Door> doors, Player player, int id)
            : base(map, path, doors, player)
        {
            texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Spectres/Wrath/wrath_walk_spritesheet");
            hearingRange = 500;
            hearingSphere = new BoundingSphere(new Vector3(position, 0), hearingRange);

            spriteWidth = 350;
            spriteHeight = 350;
            spriteRowSize = 4;
            spriteColNum = 2;
            InitializeAnimations();
            updateBoundingBox(spriteWidth / 2, spriteHeight / 2);

            homePosition = new GameVector2(3 * MapUnit.MAX_SIZE, 2 * MapUnit.MAX_SIZE);


            grunt = SoundManager.getCue(SoundManager.DUMB.GRUNT);
            roar = SoundManager.getCue(SoundManager.WRATH.ROAR);
            walk = SoundManager.getCue(SoundManager.DUMB.FOOTSTEP);
            excorcismCue = SoundManager.getCue(SoundManager.DUMB.EXCORCISED);
            InvestigateCue = SoundManager.getCue(SoundManager.DUMB.ALERT);
            orientation = new GameVector2(-1, 0);
            active = false;
            this.id = id;
            this.initializeFootstepTimer(id * 5);
        }

        public override void BuildBehaviorMachine()
        {
            InactiveState tempInactive = new InactiveState();
            PatrolState tempPatrol = new PatrolState();
            WrathWaitState tempWait = new WrathWaitState();
            InvestigateState tempInvestigate = new InvestigateState();
            AlertedState tempAlerted = new AlertedState();
            ChaseState tempChase = new ChaseState();
            ChaseAroundCornerState tempAroundCorner = new ChaseAroundCornerState();
            WrathAtkIdleState tempAtkIdle = new WrathAtkIdleState();

            tempInactive.addTransition(new ToPatrolFromInactiveTransition(tempPatrol));
            tempInactive.addTransition(new ToAlertedTransition(tempAlerted));

            tempPatrol.addTransition(new ToWaitFromPatrolTransition(tempWait));
            tempPatrol.addTransition(new ToInvestigateFromPatrolTransition(tempInvestigate));
            tempPatrol.addTransition(new ToAlertedTransition(tempAlerted));

            //Waiting in a spot transitions
            tempWait.addTransition(new ToInvestigateFromPatrolTransition(tempInvestigate));
            tempWait.addTransition(new ToAlertedTransition(tempAlerted));

            //Investigate Transitions
            tempInvestigate.addTransition(new ToAlertedTransition(tempAlerted));
            tempInvestigate.addTransition(new ToPatrolTransition(tempPatrol));//tempWait));

            //Alert/Chase/Attack Transitions
            tempAlerted.addTransition(new ToChaseTransition(tempChase));
            //tempChase.addTransition(new ToInvestigateFromChaseTransition(tempInvestigate));
            tempChase.addTransition(new ToInvestigateFromChaseTransition(tempAroundCorner));
            tempChase.addTransition(new ToPossessTransition(tempAtkIdle));

            tempAroundCorner.addTransition(new ToChaseFromAroundCornerTransition(tempChase));
            tempAroundCorner.addTransition(new ToInvestigateFromAroundCornerTransition(tempInvestigate));
            tempAroundCorner.addTransition(new ToPatrolFromAroundCornerTransition(tempPatrol));//tempWait));

            tempAtkIdle.addTransition(new ToPatrolTransition(tempInvestigate));

            //State Machine Creation
            behaviorMachine = new StateMachine(tempInactive);
            behaviorMachine.AddState(tempPatrol);
            behaviorMachine.AddState(tempWait);
            behaviorMachine.AddState(tempInvestigate);
            behaviorMachine.AddState(tempAlerted);
            behaviorMachine.AddState(tempChase);
            behaviorMachine.AddState(tempAroundCorner);
            behaviorMachine.AddState(tempAtkIdle);
        }
    }
}
