using System;
using UnityEngine;
using UnityEngine.Events;

public class Poolable : MonoBehaviour
{

    #region EVENTS

    public UnityEvent onCreateEvent; 
    public UnityEvent onEnableEvent;
    public UnityEvent onGetEvent; 
    public UnityEvent onReleaseEvent;
    public UnityEvent onDisableEvent;
    public UnityEvent onDestroyEvent; 

    #endregion EVENTS

    private bool _isReleased = false;

    public bool IsReleased
    {
        get => _isReleased;
//        set => _isReleased = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        throw new NotImplementedException();
    }
}
