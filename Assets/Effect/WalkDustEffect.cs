using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkDustEffect : PollingObject
{
    public GameObject[] effObjs; 
    private Material[] materials = new Material[2];
    
    private float time = 0f;
    public float duration = 0.1f;
    public int maxIndex = 4;

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            materials[i] = effObjs[i].GetComponent<Renderer>().material;
        }
        
    }
    
    void Update()
    {
        time += Time.deltaTime;
        if (time >= duration)
        {
            Die();
        }

        int index = (int)(time / duration * maxIndex);

        for (int i = 0; i < 1; i++)
        {
            materials[i].mainTextureOffset = new Vector2(index * 1f/maxIndex, 0f);
        }
    }

    public void Active(Transform tr)
    {
        time = 0f;
        transform.rotation = tr.rotation * Quaternion.AngleAxis(90, Vector3.up);
        transform.position = tr.transform.position + new Vector3(0, 0.1f, 0);
    }
    
}
