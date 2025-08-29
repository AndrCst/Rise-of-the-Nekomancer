using System.Collections;
using UnityEngine;

public class SkillshotAbilityHandler : AbilityHandler
{
    public bool IsAimed;
    public override void OnSpawn()
    {
        if (IsAimed)
        {
            StartCoroutine(WaitForCast());
        }
        else
        {
            FireEffect();
        }
            MouseManager = GameManager.Instance.MouseManager;
        transform.forward = new Vector3(MouseManager.WorldMousePosition.x, transform.position.y, MouseManager.WorldMousePosition.z) - transform.position;
    }

    IEnumerator WaitForCast()
    {
        yield return new WaitForSeconds(MyAbility.CastTime);
        FireEffect();
    }

    public virtual void FireEffect()
    {
        OnReturn();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    void Update()
    {
        HandleTargeting();
    }

    void HandleTargeting()
    {
        transform.position = MyAbility.transform.position;

        if (MyAbility.Caster.IsCasting)
        {
            transform.forward = new Vector3(MouseManager.WorldMousePosition.x, transform.position.y, MouseManager.WorldMousePosition.z) - transform.position;
        }
    }
}
