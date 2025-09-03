using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawningState : AgentState
{
    public void Enter(AgentController agent)
    {
        agent.SpawningEnter();
    }

    public void Exit(AgentController agent)
    {
        agent.SpawningExit();
    }

    public AgentStateID GetID()
    {
        return AgentStateID.SpawningState;
    }

    public void Update(AgentController agent)
    {
        agent.SpawningUpdate();
    }
}
