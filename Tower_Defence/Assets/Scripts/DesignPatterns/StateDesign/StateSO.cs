using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSO : ScriptableObject
{
    public string StateName;  // Name for debugging or UI purposes
    [TextArea] public string Description; // Optional description

    public virtual void OnEnter(GameObject owner) { }
    public virtual void OnExit(GameObject owner) { }
    public virtual void OnUpdate(GameObject owner) { }
}
