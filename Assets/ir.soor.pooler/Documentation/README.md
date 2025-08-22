# Pooler

[Read in Persian/پارسی](./PersianReadMe.md)

Pooler is a Unity package designed to simplify and streamline object pooling in your projects. It provides a robust and reusable way to manage pooled objects, offering a clean API and customizable lifecycle events. This can lead to significant performance improvements by reducing the overhead of instantiating and destroying GameObjects.

##Core Components
The package consists of three main scripts:

* Poolable.cs: This is a MonoBehaviour script that you attach to any GameObject you want to be pooled. It provides UnityEvents that are triggered at different stages of the object's lifecycle, such as when it's created, retrieved from the pool, or returned to the pool. This allows you to easily attach custom logic for initialization, activation, and deactivation without needing to manually manage these states in your code.

* Pooler.cs: A [Serializable] class that manages a single object pool. It utilizes Unity's built-in ObjectPool<T> class and includes serialized fields for the pool's name, default capacity, and maximum capacity. It also holds a list of Poolable prefabs, allowing the pool to instantiate a single type or randomly select from multiple types.

* PoolsManager.cs: A MonoBehaviour script that serves as a central hub for managing multiple Pooler instances. It can be attached to a GameObject in your scene to manage all your object pools from a single location. It provides methods to add, generate, retrieve, and destroy pools by their unique names, making it easy to handle your project's pooling needs.

##Features
* Flexible Pooling: Pooler can handle a single type of prefab or randomly instantiate from a list of different prefabs.

* Custom Lifecycle Events: The Poolable component provides UnityEvents for OnCreate, OnGet, and OnRelease, giving you fine-grained control over your pooled objects' behavior.

* Centralized Management: PoolsManager allows you to manage all your object pools from one place, either through the Inspector or via code.

* Performance: By leveraging Unity's native ObjectPool<T>, this package offers a highly optimized solution for reducing garbage collection and instantiation costs.

##Getting Started
###Installation

1. Open the Unity Package Manager.

2. Click the + button in the top-left corner.

3. Select "Add package from git URL..."

4. Paste the following URL: https://github.com/AbolfazlHo/Pooler.git?path=Assets/ir.soor.pooler

5. Click "Add".

###Usage

1. Create a Poolable Prefab:

    * Create a GameObject that you want to pool.

    * Add the Poolable.cs script to it.

    * Set up any necessary UnityEvents in the Inspector (e.g., set onGetEvent to activate a particle effect).

    * Drag the GameObject into your project tab to create a prefab.

2. Set up the PoolsManager:

    * Create a new empty GameObject in your scene.

    * Add the PoolsManager.cs script to it.

    * In the Inspector, add a new Pooler to the All Poolers list.

    * Give the Pooler a unique name.

    * Drag your Poolable prefab into the Objects to Pool list.

    * Adjust the Pool Default Capacity and Pool Max Capacity as needed.

3. Generate the Pool:

    * Check the Generate All Pools on Awake box in the PoolsManager Inspector if you want the pool to be ready when the scene starts.

    * Alternatively, you can call PoolsManager.GenerateObjectPool("YourPoolName") from another script to generate it manually.

4. Use the Pool:

To get an object from the pool:

```csharp
Pooler myPooler = poolsManager.GetPooler("YourPoolName");
Poolable pooledObject = myPooler.ObjectPool.Get();
```

To return an object to the pool:

```csharp
myPooler.ObjectPool.Release(pooledObject);
```
To add a new poolable prefab to a pool that is already generated:

```csharp
myPooler.AddPoolablePrefab(newPoolablePrefab);
```


