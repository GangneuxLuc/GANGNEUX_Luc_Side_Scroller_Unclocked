using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class State : MonoBehaviour // State pour les soldats
{
    
    public SoldierScript soldierScript;
    [Range(0,5f)] public float speed;
    protected Transform activeTimeline;
    protected Transform playerPos;

    public abstract State RunCurrentState();
    public void Awake()
    {
       //ennemyClass = MonoScript.FromMonoBehaviour(this);
       //Debug.Log("State Awake : " + ennemyClass.name);
       soldierScript = GetComponentInParent<SoldierScript>();
       activeTimeline = soldierScript.activeTimeline;
        playerPos = soldierScript.playerPos;
    }
    
}

