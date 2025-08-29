using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectTools
{   
    public static List<GameObject> GetValidObjectsOnRange(Vector3 origin, float radius, string searchedTag, LayerMask layerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, radius, layerMask);
        List<GameObject> objects = new();

        foreach(var collider in colliders)
        {
            if (collider != null && collider.CompareTag(searchedTag))
            objects.Add(collider.gameObject);
        }

        return objects;
    }

    public static bool GetTargetOnSight(GameObject target, Vector3 origin, float range,LayerMask layerMask)
    {
        Vector3 direction = GetDirection(origin, target.transform.position);
        if (Physics.Raycast(origin, direction, out RaycastHit hit, range,layerMask))
        {
            if (hit.collider.gameObject == target)
            {
                Debug.DrawRay(origin, direction * 1000, Color.red);
                return true;
            }
        }

        Debug.DrawRay(origin, direction * hit.distance, Color.white);
        return false;
    }

    public static Vector3 GetDirection(Vector3 origin, Vector3 targetPos)
    {
        Vector3 direction = targetPos - origin;
        return direction;
    }


    public static GameObject GetNearestObject(GameObject[] objects, Vector3 origin)
    {
        float nearestDistance = float.MaxValue;
        float currentDistance;
        GameObject nearestObject = null;

        foreach (var obj in objects)
        {
            if (obj == null)
            continue;

            //currentDistance = Vector3.Distance(origin, obj.transform.position);
            currentDistance = (obj.transform.position - origin).sqrMagnitude;

            if (currentDistance < nearestDistance)
            {
                nearestDistance = currentDistance;
                nearestObject = obj;
            }
        }

        return nearestObject;
    }


    public static bool GetTargetOnRange(Vector3 origin, Vector3 targetPos, float range)
    {
        if (Vector3.Distance(origin, targetPos) <= range) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool GetObjectOnRange(Vector3 origin, GameObject targetObj, float range)
    {
        if (targetObj.TryGetComponent<Collider>(out Collider targetCollider))
        {
            if (Vector3.Distance(origin, targetCollider.ClosestPoint(origin)) <= range)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return GetTargetOnRange(origin, targetObj.transform.position, range);
        }
    }


    public static GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
