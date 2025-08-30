using System.Collections;
using UnityEngine;

public class SkillshotAbilityHandler : AbilityHandler
{
    public MouseManager MouseManager;

    public override void OnSpawn()
    {
        MouseManager = GameManager.Instance.MouseManager;
        transform.forward = new Vector3(MouseManager.WorldMousePosition.x, transform.position.y, MouseManager.WorldMousePosition.z) - transform.position;
        base.OnSpawn();     
    }

    void FixedUpdate()
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
