using System.Collections.Generic;
using UnityEngine;

public class PoolTest : MonoBehaviour
{

    [SerializeField] private PoolsManager _poolsManager;


    private Pooler _pooler = null;

    private List<Poolable> _poolables = new List<Poolable>();
    
    
    public void PoolabelEnabled()
    {
        Debug.Log("PoolabelEnabled");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
    }

    private void GetAPoolable()
    {
        if (_pooler == null)
        {
            _pooler = _poolsManager.GetPooler("my-pool");
            _poolsManager.GenerateObjectPool(_pooler);
        }
        
        var p = _pooler.ObjectPool.Get();
//        _poolables.Add(_pooler.ObjectPool.Get());
        _poolables.Add(p);
    }

    private void ReleasePoolable(Poolable poolable)
    {
        if (_pooler == null) return;
        
        _pooler.ObjectPool.Release(poolable);
        
    }
}
