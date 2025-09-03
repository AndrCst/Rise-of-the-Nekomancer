using System.Collections;
using UnityEngine;

public class AllyController : AgentController
{
    public override void IdleUpdate()
    {
        base.IdleUpdate();
        TrackTarget(PlayerController.transform.position);
    }

    public override void IdleEnter()
    {
        base.IdleEnter();
        NavAgent.stoppingDistance = GetStoppingDistance(PlayerController.gameObject);
    }

    public override void PathingUpdate()
    {
        base.PathingUpdate();
        CheckForDeAggro(PlayerController.gameObject);
    }

    public override void AttackUpdate()
    {
        base.AttackUpdate();
        CheckForDeAggro(PlayerController.gameObject);
    }

    public override void HandleDeaggro()
    {
        base.HandleDeaggro();
        StartCoroutine(PickBackUpWithPlayer());
    }
    private IEnumerator PickBackUpWithPlayer()
    {
        NavAgent.speed = 20;
        yield return new WaitForSeconds(DE_AGGRO_DURATION);
        NavAgent.speed = Damageable.BaseMovementSpeed;
    }
}
