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

    public bool IsReleased => _isReleased;

    private void OnEnable()
    {
        onEnableEvent?.Invoke();
    }

    private void OnDisable()
    {
        onDisableEvent?.Invoke();
    }

    private void OnDestroy()
    {
        Debug.Log("private void OnDestroy()      -------      private void OnDestroy()");
        onDestroyEvent?.Invoke();
    }

    public void OnCreate()
    {
        onCreateEvent?.Invoke();
    }

    public void OnGet()
    {
        _isReleased = false;
        onGetEvent?.Invoke();
    }

    public void OnRelease()
    {
        _isReleased = true;
        onReleaseEvent?.Invoke();
    }
}