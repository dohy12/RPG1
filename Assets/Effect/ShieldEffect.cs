using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : PollingObject
{
    private Transform tracingObj;
    
    private Material material;

    public float duration;
    private float time = 0f;

    public float maxSize = 3f;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= duration)
        {
            Die();
        }

        int index = (int)(time / duration * 4f);

        material.mainTextureOffset = new Vector2(index * 0.25f, 0f);

        var t = (time / duration);
        var scaleValue =  Mathf.Sin((Mathf.PI / 4) * t + (Mathf.PI / 4)) * maxSize ;
        transform.localScale =  new Vector3(1f, 1f, 1f) * scaleValue;

        SetAlpha();
    }

    private void FixedUpdate()
    {
        if (tracingObj != null)
        {
            transform.position = tracingObj.position+ Vector3.up/2f + transform.forward;
        }
        
    }

    public void Active(Transform tr)
    {
        tracingObj = tr;
        transform.rotation = tracingObj.rotation;
        transform.position = tracingObj.position + Vector3.up/2f + transform.forward;
        
        time = 0f;
    }

    void SetAlpha()
    {
        Color color = material.color;
        color.a = Mathf.Clamp01(1-time/duration); // 알파 값이 0과 1 사이에 있도록 제한
        material.color = color;
    }

}
