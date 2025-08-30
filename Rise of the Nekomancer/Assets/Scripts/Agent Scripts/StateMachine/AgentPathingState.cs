using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPathingState : AgentState
{
    public void Enter(AgentController agent)
    {
        agent.ChooseNextAbility();
        agent.NavAgent.stoppingDistance = agent.GetStoppingDistance();
    }

    public void Exit(AgentController agent)
    {
    }

    public void Update(AgentController agent)
    {
        agent.TrackTarget(agent.Target.transform.position);
        agent.ChangeToAttack();
    }


    public AgentStateID GetID()
    {
        return AgentStateID.PathingState;
    }
}
