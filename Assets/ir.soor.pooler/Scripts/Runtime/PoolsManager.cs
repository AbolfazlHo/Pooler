using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soor.Pooler
{
    public class PoolsManager : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        /// <summary>
        /// If true, all <see cref="Pooler"/> instances in the `_allPoolers` list will be generated in the Awake method.
        /// </summary>
        [SerializeField] private bool _generateAllPoolsOnAwake = false;
        
        /// <summary>
        /// A list of all <see cref="Pooler"/> instances that this component manages.
        /// </summary>
        [SerializeField] private List<Pooler> _allPoolers = new List<Pooler>();

        #endregion SERIALIZED_FIELDS


        #region METHODS

        private void Awake()
        {
            if (_generateAllPoolsOnAwake) _allPoolers.ForEach(GenerateObjectPool);
        }

        /// <summary>
        /// Adds a new <see cref="Pooler"/> instance to the list of managed pools.
        /// </summary>
        /// <param name="poolName">The unique name for the new pool.</param>
        /// <param name="objectsToPool">The list of poolable prefabs to be pooled.</param>
        /// <param name="poolDefaultCapacity">The default initial capacity of the pool.</param>
        /// <param name="poolMaxCapacity">The maximum number of objects the pool can hold.</param>
        /// <param name="generatePoolImmediately">If true, the ObjectPool will be generated immediately after the Pooler is added.</param>
        /// <exception cref="ArgumentException">Thrown if a pool with the specified name already exists.</exception>
        public void AddPooler(string poolName, List<Poolable> objectsToPool, int poolDefaultCapacity = 10, int poolMaxCapacity = 1000, bool generatePoolImmediately = true)
        {
            if (_allPoolers.Any(p => p.PoolName == poolName))
            {
                Debug.LogError($"A Pooler with the name {poolName} already exists.");
                throw new Exception("Already exists.");
            }
        
            _allPoolers.Add(new Pooler(poolName, objectsToPool, poolDefaultCapacity, poolMaxCapacity));
            if (generatePoolImmediately) GenerateObjectPool(_allPoolers[^1]);
        }

        /// <summary>
        /// Generates the <see cref="ObjectPool{T}"/> for the specified <see cref="Pooler"/> instance,
        /// if it has not been generated already.
        /// </summary>
        /// <param name="pooler">The <see cref="Pooler"/> instance for which to generate the pool.</param>
        public void GenerateObjectPool(Pooler pooler)
        {
            if (pooler.ObjectPool == null) pooler.GenerateObjectPool();
        }

        /// <summary>
        /// Finds and generates the <see cref="ObjectPool{T}"/> for the <see cref="Pooler"/> with the specified name.
        /// </summary>
        /// <param name="name">The unique name of the pool to generate.</param>
        public void GenerateObjectPool(string name)
        {
            var intendedPooler = GetPooler(name);
            GenerateObjectPool(intendedPooler);
        }

        /// <summary>
        /// Retrieves a <see cref="Pooler"/> instance by its unique name.
        /// </summary>
        /// <param name="poolerName">The unique name of the pool to retrieve.</param>
        /// <returns>The <see cref="Pooler"/> instance with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a pool with the specified name does not exist.</exception>
        public Pooler GetPooler(string poolerName)
        {
            var intendedPooler = _allPoolers.FirstOrDefault(p => p.PoolName == poolerName);
            if (intendedPooler != null) return intendedPooler;
            Debug.LogError("There isn't a pool with the intended name.");
            throw new Exception("Not found");
        }

        /// <summary>
        /// Finds and destroys the pool with the specified name, including all its GameObjects.
        /// </summary>
        /// <param name="name">The unique name of the pool to destroy.</param>
        public void DestroyObjectPool(string name)
        {
            var intendedPooler = GetPooler(name);
            DestroyObjectPool(intendedPooler);
        }

        /// <summary>
        /// Destroys a pooler and its GameObjects.
        /// </summary>
        /// <param name="pooler">The pooler instance to destroy.</param>
        public void DestroyObjectPool(Pooler pooler)
        {
            pooler.DestroyObjectPool();
        }
        
        #endregion METHODS
    }
}