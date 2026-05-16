using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour
{
    public State currentState;
  
    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();
        // ne switcher que si nextState est différent pour éviter de réactiver inutilement
        if (nextState != null && nextState != currentState)
        {
            SwitchToNextState(nextState);
        }

      

    }

    

    private void SwitchToNextState(State nextState)
    {
        // désactive l'ancien état s'il existe
        if (currentState != null)
            currentState.enabled = false;

        currentState = nextState;

        // active le nouvel état s'il existe
        if (currentState != null && !currentState.enabled)
            currentState.enabled = true;
    }

    void Update()
    {
        RunStateMachine();
    }
}
