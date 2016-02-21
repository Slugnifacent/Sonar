using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class ToOpenDoor : Transition {
        //To chase from the alerted state.
        public ToOpenDoor(State s)
            : base(s) {
            condition = new BehindDoorCondition();
        }

        // Transition to chase after having paused from alerted
        public override void doAction(Spectre spectre, Player player) {
            return;
        }
    }
}
