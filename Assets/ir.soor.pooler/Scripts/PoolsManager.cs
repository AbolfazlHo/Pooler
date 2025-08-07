using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        
        if (intendedPooler == null)
        {
            Debug.LogError("There isn't a pool with the intended name.");
            throw new Exception("A pool with the intended name not found");
        }

        GenerateObjectPool(intendedPooler);
    }

    public Pooler GetPooler(string poolerName)
    {
        var intendedPooler = _allPoolers.FirstOrDefault(p => p.PoolName == poolerName);
        return intendedPooler;
    }
    
    
    
    
    
    // DestroyObjectPool
    
}
