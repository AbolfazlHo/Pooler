using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Soor.Pooler
{
    [Serializable]
    public class Pooler
    {
        #region SERIALIZED_FIELD

        [SerializeField] private string _poolName;
        [SerializeField] private int _poolDefaultCapacity = 10;
        [SerializeField] private int _poolMaxCapacity = 1000;
        [SerializeField] private List<Poolable> _objectsToPool = new List<Poolable>();
        [SerializeField] private UnityEvent _onGeneratObjectPoolEvent;
        [SerializeField] private UnityEvent _onDestroyObjectPoolEvent;

        #endregion SERIALIZED_FIELD


        #region FIELDS

        private ObjectPool<Poolable> _objectPool = null;
        private Poolable _newPoolable;
        private List<Poolable> _allPoolables = new List<Poolable>();

        #endregion FIELDS

        
        #region PROPERTIES

        public string PoolName => _poolName;
        public ObjectPool<Poolable> ObjectPool => _objectPool;

        #endregion PROPERTIES


        #region PUBLIC_METHODS

        public Pooler(string poolNamem,List<Poolable> objectsToPool, int poolDefaultCapacity = 10, int poolMaxCapacity = 1000)
        {
            _poolName = poolNamem;
            _objectsToPool = objectsToPool;
            _poolDefaultCapacity = poolDefaultCapacity;
            _poolMaxCapacity = poolMaxCapacity;
        }

        public void GenerateObjectPool()
        {
            _objectPool = new ObjectPool<Poolable>
            (
                CreatePoolable, OnGetPoolable, OnReleasePoolable, OnDestroyPoolable,
                true, _poolDefaultCapacity, _poolMaxCapacity
            );

            OnGenerateObjectPool();
        }

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

        private void OnGenerateObjectPool()
        {
            _onGeneratObjectPoolEvent?.Invoke();
        }

        private void OnDestroyObjectPool()
        {
            _onDestroyObjectPoolEvent?.Invoke();
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
        }
        
        #endregion PRIVATE_METHODS
    }
}