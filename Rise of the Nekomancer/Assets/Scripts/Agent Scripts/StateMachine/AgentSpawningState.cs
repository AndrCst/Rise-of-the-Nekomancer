using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawningState : AgentState
{
    public void Enter(AgentController agent)
    {
    }

    public void Exit(AgentController agent)
    {
    }

    public AgentStateID GetID()
    {
        return AgentStateID.SpawningState;
    }

    public void Update(AgentController agent)
    {

    }
}
