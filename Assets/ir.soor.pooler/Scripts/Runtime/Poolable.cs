using UnityEngine;
using UnityEngine.Events;

namespace Soor.Pooler
{
    /// <summary>
    /// Manages a GameObject's lifecycle within an object pool.
    /// UnityEvents allow hooking custom logic to key lifecycle stages.
    /// </summary>
    public class Poolable : MonoBehaviour
    {
        #region EVENTS

        /// <summary>
        /// Called when the object is first created by the pool.
        /// </summary>
        public UnityEvent onCreateEvent; 
        
        /// <summary>
        /// Called when the object is enabled (OnEnable).
        /// </summary>
        public UnityEvent onEnableEvent;
        
        /// <summary>
        /// Called when the object is taken from the pool.
        /// </summary>
        public UnityEvent onGetEvent;
        
        /// <summary>
        /// Called when the object is returned to the pool.
        /// </summary>
        public UnityEvent onReleaseEvent;
        
        /// <summary>
        /// Called when the object is disabled (OnDisable).
        /// </summary>
        public UnityEvent onDisableEvent;
        
        /// <summary>
        /// Called when the object is destroyed.
        /// </summary>
        public UnityEvent onDestroyEvent; 

        #endregion EVENTS

        /// <summary>
        /// A field to track if the object has been returned to the pool.
        /// </summary>
        private bool _isReleased = false;

        /// <summary>
        /// A read-only property to access the _isReleased state from outside the class.
        /// True if the object is currently released (returned to the pool).
        /// </summary>
        public bool IsReleased => _isReleased;

        #region MONIBEHAVIOUR_METHODS

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
        
        #endregion MONIBEHAVIOUR_METHODS

        /// <summary>
        /// Called by the pool after instantiation.
        /// </summary>
        public void OnCreate()
        {
            onCreateEvent?.Invoke();
        }

        /// <summary>
        /// Called by the pool when retrieved.
        /// </summary>
        public void OnGet()
        {
            _isReleased = false;
            onGetEvent?.Invoke();
        }

        /// <summary>
        /// Called by the pool when returned.
        /// </summary>
        public void OnRelease()
        {
            _isReleased = true;
            onReleaseEvent?.Invoke();
        }
    }
}