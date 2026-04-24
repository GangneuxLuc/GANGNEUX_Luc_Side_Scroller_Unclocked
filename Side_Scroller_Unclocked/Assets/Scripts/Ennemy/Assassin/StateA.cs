using UnityEngine;

public abstract class StateA : MonoBehaviour
{
    public AssassinScript assassinScript;
    [Range(0, 5f)] public float speed;
    protected Transform activeTimeline;
    protected Transform playerPos;

    public abstract StateA RunCurrentState();
}