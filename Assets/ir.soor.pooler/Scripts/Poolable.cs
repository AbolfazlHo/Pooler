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
        onDestroyEvent?.Invoke();
    }

    public void OnCreate()
    {
        Debug.Log("OnCreate    ---------    OnCreate      --------    OnCreate     --------     OnCreate");
        
        onCreateEvent?.Invoke();
    }

    public void OnGet()
    {
        _isReleased = false;
        
        Debug.Log("OnGet      --------    OnGet      --------    OnGet     --------   OnGet");
        
        onGetEvent?.Invoke();
    }

    public void OnRelease()
    {
        _isReleased = true;
        Debug.Log("OnRelease       ---------      OnRelease       ----------     OnRelease     ---------      OnRelease");
        
        onReleaseEvent?.Invoke();
    }
}