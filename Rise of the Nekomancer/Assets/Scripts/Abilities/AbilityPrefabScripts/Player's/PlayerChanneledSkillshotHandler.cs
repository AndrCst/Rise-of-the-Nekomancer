using System.Collections;
using UnityEngine;

public class PlayerChanneledSkillshot : AbilityHandler
{
    public MouseManager MouseManager;
    [SerializeField] private GameObject channeledEffectObj;
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

    public override void FireEffect()
    {
        channelingCoroutine = StartCoroutine(HandleChanneling());
    }

    private Coroutine channelingCoroutine;
    IEnumerator HandleChanneling()
    {
        channeledEffectObj.SetActive(true);
        yield return new WaitForSeconds(MyAbility.ChanneledTime);
        channeledEffectObj.SetActive(false);
        channelingCoroutine = null;
    }

    public override void HandleInterruption(GameObject obj)
    {
        if (obj == MyAbility.Caster.CastingObject)
        {
            if (channelingCoroutine != null)
            {
                StopCoroutine(channelingCoroutine);
                channelingCoroutine = null;
                channeledEffectObj.SetActive(false);
            }
        }
        base.HandleInterruption(obj);
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
