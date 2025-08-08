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
    /// Manages the lifecycle of a Poolable object pool, including creation, reuse, and cleanup.
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
        /// List of prefabs used as source for pooled instances.
        /// </summary>
        [SerializeField] private List<Poolable> _objectsToPool = new List<Poolable>();
        
        /// <summary>
        /// Invoked after the object pool is created.
        /// </summary>
        [SerializeField] private UnityEvent _onGeneratObjectPoolEvent;
        
        /// <summary>
        /// Invoked after the object pool is destroyed and cleaned up.
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
        /// Creates a new Pooler instance with custom settings.
        /// </summary>
        /// <param name="poolName">Unique identifier for the pool.</param>
        /// <param name="objectsToPool">List of Poolable prefabs used for instantiation.</param>
        /// <param name="poolDefaultCapacity">Initial number of objects the pool can hold.</param>
        /// <param name="poolMaxCapacity">Maximum number of objects the pool can manage.</param>
        public Pooler(string poolName,List<Poolable> objectsToPool, int poolDefaultCapacity = 10, int poolMaxCapacity = 1000)
        {
            _poolName = poolName;
            _objectsToPool = objectsToPool;
            _poolDefaultCapacity = poolDefaultCapacity;
            _poolMaxCapacity = poolMaxCapacity;
        }

        /// <summary>
        /// Initializes the ObjectPool with core lifecycle callbacks and capacity settings.
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
        /// Disposes the ObjectPool, destroys its objects, and clears internal references.
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
        /// Creates a new Poolable instance by instantiating a random prefab from the list.
        /// Adds it to the internal tracking list.
        /// </summary>
        /// <returns>The newly created Poolable.</returns>
        /// <exception cref="Exception">Thrown if no prefabs are available.</exception>
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
        /// Triggers the configured UnityEvent after pool creation is completed.
        /// </summary>
        private void OnGenerateObjectPool()
        {
            _onGeneratObjectPoolEvent?.Invoke();
        }

        /// <summary>
        /// Triggers the configured UnityEvent after pool destruction is completed.
        /// </summary>
        private void OnDestroyObjectPool()
        {
            _onDestroyObjectPoolEvent?.Invoke();
        }

        /// <summary>
        /// Invoked when a Poolable object is taken from the pool for use.
        /// </summary>
        /// <param name="poolable">The retrieved Poolable instance.</param>
        private void OnGetPoolable(Poolable poolable)
        {
            poolable.OnGet();
        }

        /// <summary>
        /// Invoked when a Poolable is released back into the pool.
        /// </summary>
        /// <param name="poolable">The returned Poolable instance.</param>
        private void OnReleasePoolable(Poolable poolable)
        {
            poolable.OnRelease();
        }

        /// <summary>
        /// Invoked when a Poolable object is being permanently destroyed by the ObjectPool.
        /// </summary>
        /// <param name="poolable">The Poolable instance to destroy.</param>
        private void OnDestroyPoolable(Poolable poolable)
        {
        }
        
        #endregion PRIVATE_METHODS
    }
}