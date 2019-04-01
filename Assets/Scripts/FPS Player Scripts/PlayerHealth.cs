using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour
{

    [SyncVar]
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        if (!this.isServer)
        {
            return;
        }

        this.health -= damage;

        //print("DAMAGE RECEIVED");

        if (this.health <= 0f)
        {

        }
    }
} // class