using System.Collections;
using System.Collections.Generic;
using Script.StateMachine;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject",fileName = "Unit")]
public class ScriptableUnit : ScriptableObject
{
    public EntityType type;
    public Entity unitPrefab;
}

public enum EntityType
{
    Player = 0,
    Block = 1
}
