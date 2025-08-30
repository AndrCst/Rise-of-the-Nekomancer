public class AgentStateMachine
{
    public AgentState[] states;
    public AgentController agent;
    public AgentStateID currentState;


    public AgentStateMachine(AgentController agent)
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames(typeof(AgentStateID)).Length;
        states = new AgentState[numStates];
    }

    public void RegisterState(AgentState state)
    {
        int index = (int)state.GetID();
        states[index] = state;
    }

    public AgentState GetState(AgentStateID stateID)
    {
        int index = (int)stateID;
        return states[index];
    }
    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeStates(AgentStateID newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}
