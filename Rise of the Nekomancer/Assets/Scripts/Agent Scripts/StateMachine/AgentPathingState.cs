using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPathingState : AgentState
{
    public void Enter(AgentController agent)
    {
        agent.PathingEnter();
    }

    public void Exit(AgentController agent)
    {
        agent.PathingExit();
    }

    public void Update(AgentController agent)
    {
        agent.PathingUpdate();
    }


    public AgentStateID GetID()
    {
        return AgentStateID.PathingState;
    }
}
