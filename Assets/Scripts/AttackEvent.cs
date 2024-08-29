using System;
using System.Collections;
using System.Collections.Generic;
using Tiny;
using UnityEngine;

public class AttackEvent : MonoBehaviour
{
    public float power = 1f;
    private Rigidbody rb;
    public Trail trail;

    public float ShieldDashPower = 30f;
    public float ShieldDashDuaration = 1f;

    public float AttackPower = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void EnableTrail()
    {
        trail.enabled = true;
    }

    public void DisableTrail()
    {
        trail.enabled = false;
    }


    public void Attack1Enter()
    {
        EnableTrail();
        SetForce(AttackPower);
    }
    
    public void Attack3Enter()
    {
        EnableTrail();
        SetForce(AttackPower * 2f);
    }

    public void ShieldDash()
    {
        StartCoroutine(ForceRoutine(ShieldDashPower, ShieldDashDuaration));
    }

    private void SetForce(float pow)
    {
        StartCoroutine(ForceRoutine(pow, 0.1f));
    }
    
    private IEnumerator ForceRoutine(float pow, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.fixedDeltaTime;
            
            Vector3 direction = transform.forward;
            float distance = pow * Time.fixedDeltaTime;

            // 충돌 감지
            if (!Physics.Raycast(transform.position, direction, distance))
            {
                // 충돌이 없다면 이동
                Vector3 newPosition = transform.position + direction * distance;
                rb.MovePosition(newPosition);
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
