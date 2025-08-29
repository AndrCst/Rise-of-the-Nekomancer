using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public enum EnemyStateID
    {
        PeacefulState,
        IdleState,
        PathingState,
        AttackState
    }
    public interface EnemyState
    {
        EnemyStateID GetID();
        void Enter(EnemyController agent);
        void Update(EnemyController agent);
        void Exit(EnemyController agent);
    }

