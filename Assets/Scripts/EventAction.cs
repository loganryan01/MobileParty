using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventAction", menuName = "ScriptableObjects/EventAction", order = 1)]
public class EventAction : ScriptableObject
{
    public string message;
    
    public int coinsToAdd;
    public int coinsToRemove;
    public int starsToAdd;
    public int starsToRemove;
}
