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
        Target = PlayerController.gameObject;
    }
}
