using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance;

    public static EffectManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<EffectManager>();
            return instance;
        }
    }

    public PollingManager shieldEffectPollingManager;
    public PollingManager walkDustEffectPollingManager;


    public ShieldEffect CreateShieldEffect()
    {
        return (ShieldEffect)shieldEffectPollingManager.CreateObject();
    }

    public WalkDustEffect CreateWalkDustEffect()
    {
        return (WalkDustEffect)walkDustEffectPollingManager.CreateObject();
    }

}
