using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    public Vector3 TargetPosition;
    public float ProjectileSpeed;
    public float Damage;
    private Vector3 flyDirection;
    public string[] TargetTag;
    public GameObject Owner;
    public bool IsTracking;
    public GameObject Target;
    public float Duration = 10;

    public void OnSpawn()
    {
        HandleStartMethods();
    }

    public void OnReturn()
    {

    }

    private void OnEnable()
    {
        EntityEvents.OnEntityDied += EnemyDied;
    }

    private void OnDisable()
    {
        EntityEvents.OnEntityDied -= EnemyDied;
    }
    // Update is called once per frame
    void Update()
    {
        MoveBullet();       
    }

    void EnemyDied(GameObject deadEnemy)
    {
        if (Target == deadEnemy)
        {
            Destroy(gameObject);
        }
    }

    void HandleStartMethods()
    {
        HandleStartInfo();
        StartCoroutine(DestroyOnTime());
    }

    void HandleStartInfo()
    {
        flyDirection = TargetPosition - transform.position;
    }

    void MoveBullet()
    {
        if (IsTracking)
        {
            flyDirection = Target.transform.position - transform.position;
        }
        transform.position += ProjectileSpeed * Time.deltaTime * new Vector3(flyDirection.x, 0, flyDirection.z).normalized;
    }

    IEnumerator DestroyOnTime()
    {
        yield return new WaitForSeconds(Duration);
        OnReturn();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    public void GetProjectileInfo(GameObject ownerObj, GameObject targetObj, float speed, float damageValue, string[] enemyTag, Vector3 TargetDirection)
    {
        TargetTag = enemyTag;
        Target = targetObj;
        Owner = ownerObj;
        ProjectileSpeed = speed;
        Damage = damageValue;

    }

    private void OnTriggerEnter(Collider other)
    {
        EffectOnCollision(other);
    }

    public virtual void EffectOnCollision(Collider other)
    {
    }
}
