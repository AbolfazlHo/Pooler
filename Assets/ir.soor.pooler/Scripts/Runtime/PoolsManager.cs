using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soor.Pooler
{
    public class PoolsManager : MonoBehaviour
    {
        [SerializeField] private bool _generateAllPoolsOnAwake = false;
        [SerializeField] private List<Pooler> _allPoolers = new List<Pooler>();

        private void Awake()
        {
            if (_generateAllPoolsOnAwake) _allPoolers.ForEach(GenerateObjectPool);
        }

        public void AddPooler(string poolName, List<Poolable> objectsToPool, int poolDefaultCapacity = 10, int poolMaxCapecity = 1000, bool generatePoolImmediately = true)
        {
            if (_allPoolers.Any(p => p.PoolName == poolName))
            {
                Debug.LogError($"A Pooler with the name {poolName} already exists.");
                throw new Exception("Already exists.");
            }
        
            _allPoolers.Add(new Pooler(poolName, objectsToPool, poolDefaultCapacity, poolMaxCapecity));
            if (generatePoolImmediately) GenerateObjectPool(_allPoolers[^1]);
        }

        public void GenerateObjectPool(Pooler pooler)
        {
            if (pooler.ObjectPool == null) pooler.GenerateObjectPool();
        }

        public void GenerateObjectPool(string name)
        {
            var intendedPooler = GetPooler(name);
            GenerateObjectPool(intendedPooler);
        }

        public Pooler GetPooler(string poolerName)
        {
            var intendedPooler = _allPoolers.FirstOrDefault(p => p.PoolName == poolerName);
            if (intendedPooler != null) return intendedPooler;
            Debug.LogError("There isn't a pool with the intended name.");
            throw new Exception("Not found");
        }

        public void DestroyObjectPool(string name)
        {
            var intendedPooler = GetPooler(name);
            DestroyObjectPool(intendedPooler);
        }

        public void DestroyObjectPool(Pooler pooler)
        {
            pooler.DestroyObjectPool();
        }
    }
}