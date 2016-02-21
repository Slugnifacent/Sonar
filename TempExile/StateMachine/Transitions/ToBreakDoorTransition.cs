using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class ToBreakDoorTransition : Transition {
        //To chase from the alerted state.
        PlayerBehindDoorCondition behindDoor = new PlayerBehindDoorCondition();
        public ToBreakDoorTransition(State s)
            : base(s) {
                condition = new PlayerBehindDoorCondition();
        }

        // Transition to chase after having paused from alerted
        public override void doAction(Spectre spectre, Player player) {
            spectre.behindDoor = true;
            return;
        }
    }
}
