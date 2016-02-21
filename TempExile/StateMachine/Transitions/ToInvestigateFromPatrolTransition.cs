using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToInvestigateFromPatrolTransition : Transition
    {
        //Transition when patroling and a sound is heard so the spectre investigates.
        public ToInvestigateFromPatrolTransition(State s)
            : base(s)
        {
            condition = new AndCondition(new SoundHeardCondition(), new NotNeutralizedCondition());//PlayerInSightCondition();
        }

        // Transition to Investigate when the given Spectre hears a sound nearby
        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingSearchedFor = true;
            spectre.playHeardCue();
            Console.WriteLine("to investigate from patrol");
            return;
        }

    }
}
