using UnityEngine;

public class StateManagerA : MonoBehaviour
{
    public StateA currentState;

    private void RunStateMachine()
    {
        StateA nextState = currentState?.RunCurrentState();
        // ne switcher que si nextState est différent pour éviter de réactiver inutilement
        if (nextState != null && nextState != currentState)
        {
            SwitchToNextState(nextState);
        }
    }
    private void SwitchToNextState(StateA nextState)
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
