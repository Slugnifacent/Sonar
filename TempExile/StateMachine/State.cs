using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    /// <summary>
    /// DJ and Corey
    /// This is the abstract State class. These states will carry what spectre will be doing constantly until
    /// the state changes.
    /// </summary>
    public abstract class State
    {
        List<Transition> transitions; //The different checks to change to a different state
        int timeElapsed;

        public State()
        {
            transitions = new List<Transition>();
        }

        // Execute the effects of being in a certain state
        public abstract void doAction(Spectre spectre, Player player);

        // Execute an action that should only happen when a Spectre transitions into a certain state
        public abstract void doEntryAction(Spectre spectre, Player player);

        // Execute an action that should only happen when a Spectre transitions to leave a certain state
        public abstract void doExitAction(Spectre spectre, Player player);

        // Returns the possible transitions a Spectre has to exit its current state
        public List<Transition> getTransitions()
        {
            return transitions;
        }

        // Add another transition to the list of possible transitions to exit the Spectre's current state
        public void addTransition(Transition t)
        {
            transitions.Add(t);
        }

        public int getTimeElapsed()
        {
            return timeElapsed;
        }

        public void setTimeElapsed(int value)
        {
            timeElapsed = value;
        }
    }
}
