using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollingManager : MonoBehaviour
{
    public GameObject prefab;
    private Stack<PollingObject> stack = new ();

    public PollingObject CreateObject()
    {
        var obj = GetPollingObject();
        obj.gameObject.SetActive(true);
        return obj;
    }

    private PollingObject GetPollingObject()
    {
        if (stack.Count > 0)
        {
            return stack.Pop();
        }

        var obj = Instantiate(prefab).GetComponent<PollingObject>();
        obj.transform.SetParent(transform);
        obj.Init(this);
        return obj;
    }

    public void ReturnPollingObject(PollingObject pollingObject)
    {
        stack.Push(pollingObject);
        pollingObject.gameObject.SetActive(false);
    }
    
    
}
