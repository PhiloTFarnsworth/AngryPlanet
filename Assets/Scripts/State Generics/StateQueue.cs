using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateQueue
{
    //For the time being, I like the idea of uniform duration of status, so we'll use a state queue.
    //When a game token has a status applied, the enter function will apply the status effect and
    //the exit will restore the stats of the particular malus.  

    //Could also use this for 
    public Queue<BaseState> states = new Queue<BaseState>();
    public void Enqueue(BaseState state) {
        states.Enqueue(state);
        state.Enter();
    }

    public void Dequeue() {
        if (states.Count > 0) {
            var toDestroy = states.Dequeue();
            toDestroy.Exit();
        }
    }

    public BaseState Peek() {
        return states.Peek();
    }

    //A turn base update where we read through all the states on the queue and run their turnbehavior.
    //Used for Status Effects that like poison.  Problem is that it's unclear whether we want to expand our
    //base states for a turn by turn update.  PROS: 
    // public void TurnUpdate() {
    //     if (states.Count > 0) {
    //         BaseState[] allStates = states.ToArray();
    //         for (i = 0; i < allStates.Count; i++) {
    //             allStates[i].TurnBehaviour();
    //         }
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        //unused at the moment, but I'm looking for a spot for FIFO
        if (states.Count > 0) {
            BaseState currentState = states.Peek();
            currentState.Execute();
        }
    }
}
