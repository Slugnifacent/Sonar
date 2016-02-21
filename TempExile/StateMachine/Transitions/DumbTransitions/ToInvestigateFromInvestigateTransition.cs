using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToInvestigateFromInvestigateTransition : Transition
    {
        //This Transition is for when the spectre hears a new sound while it is already investigating.
        public ToInvestigateFromInvestigateTransition(State s)
            : base(s)
        {
            condition = new AdditionalSoundHeardCondition();
        }

        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingSearchedFor = true;
            Console.WriteLine("to investigate from investigate");
            return;
        }

        // Condition checked to investigate when already in investigate and another sound is heard
        public override bool isTriggered(Spectre spectre, Player player)
        {
            return condition.test(spectre, player);
        }
    }
}
