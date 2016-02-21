using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToChaseFromAroundCornerTransition : Transition
    {
        //To alerted from different states. This is used the same way from many different states, such as patrol/wander/investigate.
        public ToChaseFromAroundCornerTransition(State s)
            : base(s)
        {
            condition = new AndCondition(new PlayerCloseCondition(), new AndCondition(new PlayerInSightCondition(), new NotCondition(new PlayerHidingCondition())));
        }

        // Transition to alerted when the player is near the given Spectre, the player is within its line of sight, and the player is not hiding
        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = true;
            Console.WriteLine("to chase from around corner");
            //if(spectre.GetType() != typeof(Siren))
            //spectre.playAlertCue();
            return;
        }
    }
}
