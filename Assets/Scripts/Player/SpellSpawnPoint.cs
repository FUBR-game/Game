using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class SpellSpawnPoint : SpellSpawnPointBehavior
{

    // Update is called once per frame
    void Update()
    {
        if (!networkObject.IsOwner)
        {
            transform.rotation = networkObject.rotation;
            return;
        }
        
        networkObject.rotation = transform.rotation;
    }
}
