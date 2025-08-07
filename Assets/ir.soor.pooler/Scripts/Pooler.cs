using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class Pooler
{
    [SerializeField] private string _poolName;
    [SerializeField] private int _poolDefaultCapacity = 10;
    [SerializeField] private int _poolMaxCapacity = 100;

    [SerializeField] private List<Poolable> _objectsToPool = new List<Poolable>();

    private ObjectPool<Poolable> _objectPool;

    private Poolable _newPoolable;

    #region EVENTS

    // ?????
//    [SerializeField] private UnityEvent _onNewPoolableCreated;
    [SerializeField] private UnityEvent _onPoolGeneratedEvent;
    

    #endregion EVENTS
    
    public static Pooler Instance
    {
        get
        {
            if (Instance != null) return Instance;
            else return new Pooler();
        }
    }

    public void GenerateObjectPool()
    {
        _objectPool = new ObjectPool<Poolable>(CreatePoolable, OnGetPoolable, OnReleasePoolable, OnDestroyPoolable,
            true, _poolDefaultCapacity, _poolMaxCapacity);




        OnPoolGenerated();
    }

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

//        createdPoolable.onCreateEvent.AddListener(OnNewPoolableCreated);
        createdPoolable.OnCreate();
//        OnNewPoolableCreated();
        return createdPoolable;
    }

//    public void OnNewPoolableCreated()
//    {
//        _onNewPoolableCreated?.Invoke();
//    }


    public void OnPoolGenerated()
    {
        _onPoolGeneratedEvent?.Invoke();
    }

    private void OnGetPoolable(Poolable poolable)
    {
        poolable.OnGet();
    }

    private void OnReleasePoolable(Poolable poolable)
    {
        poolable.OnRelease();
    }

    private void OnDestroyPoolable(Poolable poolable)
    {
        //
    }
    
    
}
