using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PollingObject : MonoBehaviour
{
    private PollingManager manager;

    public void Init(PollingManager manager)
    {
        this.manager = manager;
    }
    
    public void Die()
    {
        manager.ReturnPollingObject(this);
    }
    
}
