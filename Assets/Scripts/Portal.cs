using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public StateChange stateChange;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == 8){
            stateChange.CrossScene_Direct();
        }
    }
}
