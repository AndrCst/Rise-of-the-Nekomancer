using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerChanneledSkillshot : AbilityHandler
{
    public MouseManager MouseManager;
    [SerializeField] private GameObject channeledEffectObj;
    [SerializeField] private float followSpeed;
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
            Vector3 mousePos = new Vector3(MouseManager.WorldMousePosition.x, transform.position.y, MouseManager.WorldMousePosition.z) - transform.position;

            Quaternion targetRot = Quaternion.LookRotation(mousePos, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, followSpeed * Time.deltaTime);
        }
    }
}
