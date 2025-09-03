using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPeacefulState : AgentState
{
    public void Enter(AgentController agent)
    {
        agent.PeacefulEnter();
    }

    public void Exit(AgentController agent)
    {
        agent.PeacefulExit();
    }

    public AgentStateID GetID()
    {
        return AgentStateID.PeacefulState;
    }

    public void Update(AgentController agent)
    {
        agent.PeacefulUpdate();
    }
}
