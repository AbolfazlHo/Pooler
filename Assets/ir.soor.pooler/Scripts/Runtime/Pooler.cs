using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Soor.Pooler
{
    /// <summary>
    /// This class is responsible for managing an Object Pool.
    /// </summary>
    [Serializable]
    public class Pooler
    {
        #region SERIALIZED_FIELD

        /// <summary>
        /// A name to identify this specific pool.
        /// </summary>
        [SerializeField] private string _poolName;
        
        /// <summary>
        /// The initial size of the pool.
        /// </summary>
        [SerializeField] private int _poolDefaultCapacity = 10;
        
        /// <summary>
        /// The maximum number of objects the pool can hold.
        /// </summary>
        [SerializeField] private int _poolMaxCapacity = 1000;
        
        /// <summary>
        /// A list of prefabs that will be pooled. The pool will instantiate from this list.
        /// </summary>
        [SerializeField] private List<Poolable> _objectsToPool = new List<Poolable>();
        
        /// <summary>
        /// Event invoked when the pool is generated.
        /// </summary>
        [SerializeField] private UnityEvent _onGeneratObjectPoolEvent;
        
        /// <summary>
        /// Event invoked when the pool is destroyed.
        /// </summary>
        [SerializeField] private UnityEvent _onDestroyObjectPoolEvent;

        #endregion SERIALIZED_FIELD


        #region FIELDS

        /// <summary>
        /// The actual Object Pool instance.
        /// </summary>
        private ObjectPool<Poolable> _objectPool = null;
        
        /// <summary>
        /// A list to keep track of all objects ever created by the pool.
        /// </summary>
        private List<Poolable> _allPoolables = new List<Poolable>();

        #endregion FIELDS

        
        #region PROPERTIES

        /// <summary>
        /// Read-only property to access the pool's name.
        /// </summary>
        public string PoolName => _poolName;
        
        /// <summary>
        /// Read-only property to access the ObjectPool itself.
        /// </summary>
        public ObjectPool<Poolable> ObjectPool => _objectPool;

        #endregion PROPERTIES


        #region PUBLIC_METHODS

        /// <summary>
        /// A constructor to create a new Pooler instance from code.
        /// </summary>
        /// <param name="poolNamem">The name of the pool.</param>
        /// <param name="objectsToPool">The list of objects that supposed to be pooled.</param>
        /// <param name="poolDefaultCapacity">The default capacity of the pool.</param>
        /// <param name="poolMaxCapacity">The max count of objects within the pool.</param>
        public Pooler(string poolNamem,List<Poolable> objectsToPool, int poolDefaultCapacity = 10, int poolMaxCapacity = 1000)
        {
            _poolName = poolNamem;
            _objectsToPool = objectsToPool;
            _poolDefaultCapacity = poolDefaultCapacity;
            _poolMaxCapacity = poolMaxCapacity;
        }

        /// <summary>
        /// Generates ObjectPool.
        /// Create a new ObjectPool, defining the four core callbacks:
        /// 1. CreatePoolable: The function called to create a new object.
        /// 2. OnGetPoolable: The function called when an object is retrieved from the pool.
        /// 3. OnReleasePoolable: The function called when an object is returned to the pool.
        /// 4. OnDestroyPoolable: The function called when an object is permanently destroyed from the pool.
        /// </summary>
        public void GenerateObjectPool()
        {
            _objectPool = new ObjectPool<Poolable>
            (
                CreatePoolable, OnGetPoolable, OnReleasePoolable, OnDestroyPoolable,
                true, _poolDefaultCapacity, _poolMaxCapacity
            );

            OnGenerateObjectPool();
        }

        /// <summary>
        /// Completely destroys and cleans the ObjectPool.
        /// </summary>
        public void DestroyObjectPool()
        {
            if (_objectPool == null) return;

            _objectPool.Clear();
            _objectPool.Dispose();
            _objectPool = null;
        
            _allPoolables.ForEach(p =>
                {
                    if (p.gameObject != null) Object.Destroy(p.gameObject);
                })
                ;
        
            _allPoolables.Clear();
            _allPoolables = new List<Poolable>();
        
            OnDestroyObjectPool();
        }
        
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS

        /// <summary>
        /// Creates (instantiates) a new Poolable.
        /// </summary>
        /// <returns>The instantiated Poolable object.</returns>
        /// <exception cref="Exception">If there is no Poolable prefab to create.</exception>
        private Poolable CreatePoolable()
        {
            if (_objectsToPool.Count == 0)
            {
                Debug.LogError("_objectsToPool list is empty. Assign some poolables to it.");
                throw new Exception("Null Poolable exception!");
            }

            Poolable createdPoolable;

            if (_objectsToPool.Count == 1)
            {
                createdPoolable = Object.Instantiate(_objectsToPool[0]);
            }
            else
            {
                var randomIndex = Random.Range(0, _objectsToPool.Count);
                createdPoolable = Object.Instantiate(_objectsToPool[randomIndex]);
            }

            createdPoolable.OnCreate();
        
            if (_allPoolables == null)
            {
                _allPoolables = new List<Poolable>();
            }
        
            _allPoolables.Add(createdPoolable);
            return createdPoolable;
        }

        /// <summary>
        /// Invokes the custom event for pool generation.
        /// </summary>
        private void OnGenerateObjectPool()
        {
            _onGeneratObjectPoolEvent?.Invoke();
        }

        /// <summary>
        /// Invokes the custom event for pool destruction.
        /// </summary>
        private void OnDestroyObjectPool()
        {
            _onDestroyObjectPoolEvent?.Invoke();
        }

        /// <summary>
        /// Method called by the ObjectPool when an object is retrieved.
        /// </summary>
        /// <param name="poolable">The taken Poolable from the pool.</param>
        private void OnGetPoolable(Poolable poolable)
        {
            poolable.OnGet();
        }

        /// <summary>
        /// Method called by the ObjectPool when an object is returned.
        /// </summary>
        /// <param name="poolable">The given poolable to the pool.</param>
        private void OnReleasePoolable(Poolable poolable)
        {
            poolable.OnRelease();
        }

        /// <summary>
        /// Method called by the ObjectPool when an object is destroyed.
        /// </summary>
        /// <param name="poolable">The Poolable object will be destroyed.</param>
        private void OnDestroyPoolable(Poolable poolable)
        {
        }
        
        #endregion PRIVATE_METHODS
    }
}