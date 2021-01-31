using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class DirectorEvents : MonoBehaviour
{
    public UnityEvent onCinematicEnd;
    private UnityEngine.Playables.PlayableDirector director;
    // Start is called before the first frame update
    void Start()
    {
        director = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (director.time >= director.duration)
        {
            onCinematicEnd.Invoke();
            enabled = false;
        }
    }
}
