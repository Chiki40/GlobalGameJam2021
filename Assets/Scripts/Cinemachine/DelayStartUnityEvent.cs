using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayStartUnityEvent : MonoBehaviour
{
    public UnityEvent delayEvent;
    public float delayTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delayCoroutine());
    }

    // Update is called once per frame
    IEnumerator delayCoroutine()
    {
        yield return new WaitForSeconds(delayTime);
        delayEvent.Invoke();
    }
}
