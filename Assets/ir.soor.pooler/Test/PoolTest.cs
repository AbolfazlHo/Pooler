using System.Collections.Generic;
using Soor.Pooler;
using SoorPooler;
using UnityEngine;

public class PoolTest : MonoBehaviour
{

    [SerializeField] private PoolsManager _poolsManager;
    [SerializeField] private List<Poolable> _secondPoolables;


    [SerializeField] private Transform _parent;
    

    private Pooler _pooler = null;
    private List<Poolable> _poolables = new List<Poolable>();
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetAPoolable();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_poolables.Count != 0)
            {
                ReleasePoolable(_poolables[^1]);
                _poolables.Remove(_poolables[^1]);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            DestroyObjectPool();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
//            CreateNewPool();
            ThirdPool();

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
//            _pooler = _poolsManager.GetPooler("other-pool");
//            _pooler.AddPoolablePrefab(_secondPoolables[0]);


            GetFromThirdPool();
        }
    }

    private void GetAPoolable()
    {
        if (_pooler == null)
        {
//            _pooler = _poolsManager.GetPooler("my-pool");
            _pooler = _poolsManager.GetPooler("other-pool");
            _poolsManager.GenerateObjectPool(_pooler);
        }
        
        var p = _pooler.ObjectPool.Get();
        _poolables.Add(p);
    }

    private void ReleasePoolable(Poolable poolable)
    {
        _pooler?.ObjectPool.Release(poolable);
    }

    private void DestroyObjectPool()
    {
        Debug.Log("private void DestroyObjectPool()");
        if (_pooler == null) return;
        _poolsManager.DestroyObjectPool(_pooler);
        _pooler = null;
    }

    private void CreateNewPool()
    {
//        _poolsManager.AddPooler("second-pool", _secondPoolables, 10, 20, true);
        _poolsManager.AddPooler("other-pool", _secondPoolables, false, 10, 20, true);
        _pooler = _poolsManager.GetPooler("other-pool");
        _pooler.InstantiationParent = _parent;

    }


    private Pooler _thirdPooler = null;
    
    private void ThirdPool()
    {
        _thirdPooler = new Pooler("thirdPool", _secondPoolables);
    }

    private void GetFromThirdPool()
    {
        _thirdPooler.ObjectPool.Get();
    }
    
    
}
