using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToChaseTransition : Transition
    {
        //To chase from the alerted state.
        public ToChaseTransition(State s) : base(s)
        {
            condition = new AlertedTimeCondition();
        }

        // Transition to chase after having paused from alerted
        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = true;
            Console.WriteLine("to chase");              
            //if(spectre.GetType() != typeof(Siren))
                spectre.playAlertCue();
            return;
        }
    }
}
