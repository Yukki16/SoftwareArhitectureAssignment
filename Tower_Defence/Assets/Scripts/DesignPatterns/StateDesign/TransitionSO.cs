using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTransition", menuName = "ScriptableObjects/StateMachine/Transition")]
public class TransitionSO : ScriptableObject
{
    public List<StateSO> FromStates; // Multiple states can transition to the same target state
    public StateSO toState;
    public string condition;  // A condition identifier (e.g., "PlayerInRange")
                                //Made (with the help of GPT) an update version of the asset that transforms the tags into enums
                                //for the conditions to be turned into enums to remove the possibility of misspelling
}
