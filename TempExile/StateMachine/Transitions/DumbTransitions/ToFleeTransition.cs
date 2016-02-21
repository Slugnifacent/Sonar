using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class ToFleeTransition : Transition
    {
        //The transition given from possess to flee.
        public ToFleeTransition(State s)
            : base(s)
        {
            condition = new ExorcisedCondition();
        }

        // Change Spectres movement speed to that of fleeing and remove possession from player
        // Some of this is also done in the entry action for to flee, but the redundancy is okay for now.
        public override void doAction(Spectre spectre, Player player)
        {
            Console.WriteLine("to flee");
            
            spectre.speed = 150;
            player.SetPossessor(Player.possessor.none);
            return;
        }

        // Can only be taken out of the player after at least a second has passed.
        public override bool isTriggered(Spectre spectre, Player player) {
            if (spectre.fleeTimer > .75) {
                return condition.test(spectre, player);
            }
            else {
                return false;
            }
        }
    }
}
