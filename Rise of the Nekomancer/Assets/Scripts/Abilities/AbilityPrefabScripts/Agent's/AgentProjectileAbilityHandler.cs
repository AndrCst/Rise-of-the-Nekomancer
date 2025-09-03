using UnityEngine;
using System.Collections;

public class AgentProjectileAbilityHandler : AbilityHandler
{
    [SerializeField] private bool locksIn;
    [SerializeField] private Projectile projectilePrefab;
    private AgentController controller;
    private Vector3 originalPos;
    public override void OnSpawn()
    {
        controller = MyAbility.Caster.CastingObject.GetComponent<AgentController>();

        originalPos = controller.Target.transform.position;

        transform.forward = originalPos - transform.position;

        base.OnSpawn();
    }
    public override void FireEffect()
    {
        var instantiated = ObjectPoolManager.SpawnObject(projectilePrefab, transform.position, Quaternion.identity);

        if (locksIn)
        {
            instantiated.TargetPosition = controller.Target.transform.position;
        }
        else
        {
            instantiated.TargetPosition = originalPos;
        }

            instantiated.OnSpawn();

        base.FireEffect();
    }

    private void Update()
    {
        if (locksIn)
        {
            HandleTargeting();
        }
    }

    void HandleTargeting()
    {
        transform.position = MyAbility.transform.position;

        if (MyAbility.Caster.IsCasting)
        {
            transform.forward = controller.Target.transform.position - transform.position;
        }
    }

    public override void HandleInterruption(GameObject obj)
    {
        base.HandleInterruption(obj);
        HandleTargetDied(obj);
    }
 

    void HandleTargetDied(GameObject obj)
    {
        if (obj = controller.Target)
        {
            HandleStop();
        }
    }
}
