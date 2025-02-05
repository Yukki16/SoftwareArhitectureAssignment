using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float timer = 3f;
    public bool timedDeath;
    public void DestroyThisGO()
    {
        Destroy(this.gameObject);
    }

    private void Start()
    {
        if(timedDeath)
        {
            StartCoroutine(TimedDeath());
        }
    }

    IEnumerator TimedDeath()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}
