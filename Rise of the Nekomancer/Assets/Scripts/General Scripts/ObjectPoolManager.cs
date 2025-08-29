using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool _addToDontDestroyOnLoad = false;

    private static GameObject _emptyHolder;

    private static Dictionary<GameObject, GameObject> _empties;

    private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

    public readonly Vector3 POOLING_POSITION = new(10000, 10000, 10000);

    private const int MANAGER_DEFAULT_CAPACITY = 10;

    private const int MANAGER_DEFAULT_MAX_SIZE = 10000;

    private void Awake()
    {
        _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

        SetupEmpties();
    }
    void SetupEmpties()
    {
        _emptyHolder = new GameObject("Object Pools");

        _empties = new Dictionary<GameObject, GameObject>();

        if(_addToDontDestroyOnLoad)
        {
            DontDestroyOnLoad(_emptyHolder); 
        }
    }

    static GameObject HandleEmpty(GameObject prefab)
    {
        if (_empties.ContainsKey(prefab))
        {
            return _empties[prefab];
        }
        else
        {
            GameObject emptyParent = new GameObject(prefab.name);
            _empties.Add(prefab, emptyParent);
            emptyParent.transform.SetParent(_emptyHolder.transform);
            return emptyParent;
        }
    }

    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot,bool collectionCheck = true, int defaultCapacity = MANAGER_DEFAULT_CAPACITY, int maxSize = MANAGER_DEFAULT_MAX_SIZE) 
    {
        ObjectPool<GameObject> pool = new(
            createFunc: () => CreateObject(prefab, pos, rot),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject,
            collectionCheck,
            defaultCapacity,
            maxSize
            );

        _objectPools.Add(prefab, pool);
    }

    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        //THIS MIGHT BREAK. I'm setting the PREFAB to false, if something else tries to summon it, it might come out inactive.
        //Leaving this here commented because the original script I read had this
        //prefab.SetActive(false); 

        GameObject obj = Instantiate(prefab, pos, rot);

        obj.SetActive(false);

        //prefab.SetActive(true);

        GameObject parent = SetParentObject(prefab);

        obj.transform.SetParent(parent.transform);

        return obj;
    }

    private static void OnGetObject(GameObject obj)
    {

    }

    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);

    }

    private static void OnDestroyObject(GameObject obj)
    {
        if (_cloneToPrefabMap.ContainsKey(obj))
        {
            _cloneToPrefabMap.Remove(obj);
        }
    }

    private static GameObject SetParentObject(GameObject prefab)
    {
        return HandleEmpty(prefab);
    }

    private static T SpawnObject<T>(GameObject objToSpawn, Vector3 pos, Quaternion rot) where T : Object
    {
        if (!_objectPools.ContainsKey(objToSpawn))
        {
            CreatePool(objToSpawn, pos, rot);
        }

        GameObject obj = _objectPools[objToSpawn].Get();

        if (obj != null)
        {
            if (!_cloneToPrefabMap.ContainsKey(obj))
            {
                _cloneToPrefabMap.Add(obj, objToSpawn);
            }

            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError($"Object {objToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }

            return component;
        }

        return null;
    }

    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRot) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRot);
    }

    public static GameObject SpawnObject(GameObject objToSpawn, Vector3 pos, Quaternion rot)
    {
        return SpawnObject<GameObject>(objToSpawn, pos, rot);
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        if (_cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObj = SetParentObject(prefab);

            if (obj.transform.parent != parentObj.transform)
            {
                obj.transform.SetParent(parentObj.transform);
            }

            if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
        }
        else
        {
            Debug.LogWarning("Trying to return an object that isn't pooled");
        }
    }

    private static void PrewarmPool<T>(GameObject obj, Vector3 pos, Quaternion rot, int quantity) where T : Object
    {
        List<GameObject> prewarmeds = new(quantity);

        for (int i = 0; i < quantity; i++)
        {
            GameObject instantiated = SpawnObject(obj, pos, rot);
            
            prewarmeds.Add(instantiated);
        }

        foreach (GameObject prewarmed in prewarmeds)
        {
            ReturnObjectToPool(prewarmed);
        }
    }

    public static void PrewarmPool(GameObject obj, Vector3 pos, Quaternion rot, int quantity)
    {
        PrewarmPool<GameObject>(obj, pos, rot, quantity);
    }

    public static void PrewarmPool<T>(T obj, Vector3 pos, Quaternion rot, int quantity) where T : Component
    {
        PrewarmPool(obj.gameObject, pos, rot, quantity);
    }
}


public interface IPoolable
{
    public void OnSpawn();

    public void OnReturn();
}
    