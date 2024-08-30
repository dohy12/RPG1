using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PollingManagerParent : MonoBehaviour
{
    private static PollingManagerParent instance;

    public static PollingManagerParent Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PollingManagerParent>();
            return instance;
        }
    }

    public PollingManager shieldEffectPollingManager;


}
