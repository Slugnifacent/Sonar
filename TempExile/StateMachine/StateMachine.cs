using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;








namespace Sonar
{
    public class StateMachine
    {
        List<State> states;
        State initialState;
        State currentState;

        public StateMachine(State initial)
        {
            states = new List<State>();
            initialState = initial;
            currentState = initialState;
        }

        public void Update(Spectre spectre, Player player)
        {
            Transition triggeredTransition = null;
            // If a transition for the current state has its conditions met, go to its corresponding state
            foreach (Transition t in currentState.getTransitions())
            {
                if (t.isTriggered(spectre, player))
                {
                    triggeredTransition = t;
                    break;
                }
            }

            if (triggeredTransition != null)
            {
                State targetState = triggeredTransition.getTargetState();
                currentState.doExitAction(spectre, player);
                triggeredTransition.doAction(spectre, player);
                targetState.doEntryAction(spectre, player);
                currentState = targetState;
                return;
            }
            // If not transitions for the current state have conditions fully met, stay in current state
            else
                currentState.doAction(spectre, player);
        }

        public List<State> GetStates()
        {
            return states;
        }

        public State getCurrenState()
        {
            return currentState;
        }

        // Add a new state to the list of possible states for the given Spectre
        public void AddState(State s)
        {
            states.Add(s);
        }

        /// <summary>
        /// Travis Carlson
        /// 2/15/12
        /// Resets the spectre to it's initial state
        /// </summary>
        /// <param name="spectre"></param>
        /// <param name="player"></param>

        public void toInitialState(Spectre spectre, Player player)
        {
            if (currentState != initialState)
            {
                currentState.doExitAction(spectre, player);
                //initialState.doAction(spectre, player);
                currentState = initialState;
            }
            initialState.doEntryAction(spectre, player);
        }
    }
}
