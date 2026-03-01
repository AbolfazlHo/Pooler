using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SoorPooler
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
        /// If true, a random prefab will be chosen for instantiation.
        /// If false, prefabs will be instantiated in a cyclical order.
        /// </summary>
        [SerializeField] private bool _poolRandomly = false;

        //ToDo: Fix the following summary.
        /// <summary>
        /// The parent of all poolables instantiated by this class. If it's `null` the poolables won't have parent.
        /// </summary>
        [Tooltip("This is an optional field.")]
        [SerializeField] private Transform _instantiationParent = null;

        #endregion SERIALIZED_FIELD


        #region EVENTS

        /// <summary>
        /// Invoked after the object pool is created.
        /// </summary>
        [Space] [SerializeField] private UnityEvent _onGenerateObjectPoolEvent;

        /// <summary>
        /// Invoked after the object pool is destroyed and cleaned up.
        /// </summary>
        [SerializeField] private UnityEvent _onDestroyObjectPoolEvent;

        #endregion EVENTS


        #region FIELDS

        /// <summary>
        /// The actual Object Pool instance.
        /// </summary>
        private ObjectPool<Poolable> _objectPool = null;

        /// <summary>
        /// A list to keep track of all objects ever created by the pool.
        /// </summary>
        private List<Poolable> _allPoolables = new List<Poolable>();

        /// <summary>
        /// Tracks the index of the last instantiated prefab when pooling cyclically.
        /// </summary>
        private int _lastCreatedPoolableIndex = 0;
//        private int _lastCreatedPoolableIndex = -1;

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

        /// <summary>
        /// Gets a read-only list of the prefabs used as source for pooled instances.
        /// </summary>
        public List<Poolable> ObjectsToPool => _objectsToPool;

        /// <summary>
        /// Gets or sets a value indicating whether the pool should instantiate prefabs randomly or cyclically.
        /// </summary>
        public bool PoolRandomly
        {
            get => _poolRandomly;
            set => _poolRandomly = value;
        }

        //ToDo: fix the following summary.
        /// <summary>
        /// Gets and Sets value including the parent of all poolables within this pooler.
        /// </summary>
        public Transform InstantiationParent
        {
            get => _instantiationParent;
            set => _instantiationParent = value;
        }

        #endregion PROPERTIES


        #region PUBLIC_METHODS

        //ToDo: Fix the comment of param `instantiationParent`
        
        /// <summary>
        /// Initializes a new instance of the Pooler class with specified settings.
        /// </summary>
        /// <param name="poolName">Unique identifier for the pool.</param>
        /// <param name="objectsToPool">List of Poolable prefabs used for instantiation.</param>
        /// <param name="poolRandomly">If true, prefabs are chosen randomly for instantiation. If false, they are chosen cyclically.</param>
        /// <param name="poolDefaultCapacity">Initial number of objects the pool can hold.</param>
        /// <param name="poolMaxCapacity">Maximum number of objects the pool can manage.</param>
        /// <param name="instantiationParent">The parent of all poolables instantiated by this pooler.</param>
        public Pooler(string poolName, List<Poolable> objectsToPool, bool poolRandomly = false,
            int poolDefaultCapacity = 10, int poolMaxCapacity = 1000, Transform instantiationParent = null)
        {
            _poolName = poolName;
            _objectsToPool = objectsToPool;
            _poolRandomly = poolRandomly;
            _poolDefaultCapacity = poolDefaultCapacity;
            _poolMaxCapacity = poolMaxCapacity;
            _instantiationParent = instantiationParent;
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

        /// <summary>
        /// Adds a new Poolable prefab to the list of source objects for the pool.
        /// A warning is logged if the pool has already been generated.
        /// </summary>
        /// <param name="poolable">The Poolable prefab to add.</param>
        public void AddPoolablePrefab(Poolable poolable)
        {
            if (_objectPool != null && _allPoolables.Count > 0)
            {
                Debug.LogWarning(
                    "Adding a prefab after the pool has been generated may prevent it from being instantiated. Existing pooled objects will be used first, so the new prefab may not be chosen until the pool needs to create new instances.");
            }

            _objectsToPool.Add(poolable);
        }

        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS

        /// <summary>
        /// Creates and initializes a new Poolable instance. The prefab is chosen either randomly or cyclically
        /// from the list of available prefabs, based on the `PoolRandomly` setting.
        /// </summary>
        /// <returns>The newly created Poolable instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the _objectsToPool list is empty.</exception>
        private Poolable CreatePoolable()
        {
            if (_objectsToPool.Count == 0)
            {
                Debug.LogError("_objectsToPool list is empty. Assign some poolables to it.");
                throw new Exception("Null Poolable exception!");
            }

            Poolable createdPoolable;

            if (_poolRandomly)
            {
                var randomIndex = Random.Range(0, _objectsToPool.Count);
                createdPoolable = Object.Instantiate(_objectsToPool[randomIndex]);
            }
            else
            {
                if (_objectsToPool.Count == 1)
                {
                    createdPoolable = InstantiatePoolable(_objectsToPool[0]);
                }
                else
                {
                    if (_lastCreatedPoolableIndex > _objectsToPool.Count -1)
                    {
                        _lastCreatedPoolableIndex = 0;
                    }
                    
                    createdPoolable = InstantiatePoolable(_objectsToPool[_lastCreatedPoolableIndex]);
                    _lastCreatedPoolableIndex++;
                }
            }

            createdPoolable.OnCreate();

            if (_allPoolables == null)
            {
                _allPoolables = new List<Poolable>();
            }

            _allPoolables.Add(createdPoolable);
            return createdPoolable;
        }

// ToDo: Fix the following summary
        /// <summary>
        /// Instantiates the intended poolable.
        /// </summary>
        /// <param name="poolable">The poolable intended to instantiate.</param>
        /// <returns>The new instance of poolable</returns>
        private Poolable InstantiatePoolable(Poolable poolable)
        {
            Poolable instantiatedPoolable;
            
            if (_instantiationParent != null)
            {
                instantiatedPoolable = Object.Instantiate(poolable, _instantiationParent);
            }
            else
            {
                instantiatedPoolable = Object.Instantiate(poolable);
            }

            return instantiatedPoolable;
        }

        /// <summary>
        /// Triggers the configured UnityEvent after pool creation is completed.
        /// </summary>
        private void OnGenerateObjectPool()
        {
            _onGenerateObjectPoolEvent?.Invoke();
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