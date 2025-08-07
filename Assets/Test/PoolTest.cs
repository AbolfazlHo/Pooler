using System.Collections.Generic;
using UnityEngine;

public class PoolTest : MonoBehaviour
{

    [SerializeField] private PoolsManager _poolsManager;
    [SerializeField] private List<Poolable> _secondPoolables;

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
            CreateNewPool();
        }
    }

    private void GetAPoolable()
    {
        if (_pooler == null)
        {
//            _pooler = _poolsManager.GetPooler("my-pool");
            _pooler = _poolsManager.GetPooler("second-pool");
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
        _poolsManager.AddPooler("second-pool", _secondPoolables, 10, 20, true);
    }
}
