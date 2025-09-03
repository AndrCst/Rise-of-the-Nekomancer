using UnityEngine;

public class AllyController : AgentController
{
    public override void IdleUpdate()
    {
        base.IdleUpdate();
        TrackTarget(PlayerController.transform.position);
    }
}
