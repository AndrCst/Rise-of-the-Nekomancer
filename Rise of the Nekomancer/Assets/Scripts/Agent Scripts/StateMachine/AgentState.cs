using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public enum AgentStateID
    {
        PeacefulState,
        IdleState,
        PathingState,
        AttackState,
        SpawningState,
        DeathState
    }
    public interface AgentState
    {
        AgentStateID GetID();
        void Enter(AgentController agent);
        void Update(AgentController agent);
        void Exit(AgentController agent);
    }

