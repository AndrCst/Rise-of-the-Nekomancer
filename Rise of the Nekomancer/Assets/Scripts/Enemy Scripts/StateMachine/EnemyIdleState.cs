using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public void Enter(EnemyController agent)
    {
    }

    public void Exit(EnemyController agent)
    {
    }

    public void Update(EnemyController agent)
    {
    }




    public EnemyStateID GetID()
    {
        return EnemyStateID.IdleState;
    }
}
