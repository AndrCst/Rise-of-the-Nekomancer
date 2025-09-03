using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour, ICaster
{
    public NavMeshAgent NavAgent;
    public GameObject Target;
    public string[] DetectedStrings;
    public float DetectionRange;
    public LayerMask SearchedMask;
    private AgentStateMachine stateMachine;
    [SerializeField] private AgentStateID startingState;
    private float trackDelay = 0.1f;
    public CapsuleCollider Collider;
    public Ability CurrentAbility;
    public List<Ability> Abilities;
    [SerializeField] private Damageable damageable;
    public PlayerController PlayerController;

    //ICaster
    public bool IsCasting
    {
        get { return isCasting; }
        set { isCasting = value; }
    }
    private bool isCasting;

    public bool ShouldBeStoppingForCasting
    {
        get { return shouldStopForCasting; }
        set { shouldStopForCasting = value; }
    }
    private bool shouldStopForCasting;
    public GameObject CastingObject
    {
        get { return gameObject; }
    }

    private void Start()
    {
        GetComponents();
        GetStartInfo();
    }

    void GetStartInfo()
    {
        CurrentAbility = Abilities[0];

        NavAgent.speed = damageable.BaseMovementSpeed;

        foreach(Ability ability in Abilities)
        {
            ability.Caster = this;
        }
    }
    private void GetComponents()
    {
        if (NavAgent == null)
        {
            NavAgent = GetComponent<NavMeshAgent>();
        }

        if (Collider == null)
        {
            Collider = GetComponent<CapsuleCollider>();
        }

        if (damageable == null)
        {
            damageable = GetComponent<Damageable>();
        }

        if (PlayerController == null)
        {
            PlayerController = GameManager.Instance.PlayerController;
        }

        CreateStateMachine();      
    }

    void CreateStateMachine()
    {
        stateMachine = new(this);
        stateMachine.RegisterState(new AgentPathingState());
        stateMachine.RegisterState(new AgentDeathState());
        stateMachine.RegisterState(new AgentIdleState());
        stateMachine.RegisterState(new AgentPeacefulState());
        stateMachine.RegisterState(new AgentSpawningState());
        stateMachine.RegisterState(new AgentAttackState());
        stateMachine.ChangeStates(startingState);
    }

    private void FixedUpdate()
    {
        stateMachine.Update();
        Debug.Log(stateMachine.currentState);
    }

    public void DetectEnemy()
    {
       var targets = ProjectTools.GetValidObjectsOnRange(transform.position, DetectionRange, DetectedStrings, SearchedMask);

        if (targets.Count > 0)
        {
            Target = ProjectTools.GetNearestObject(targets, transform.position);         

            stateMachine.ChangeStates(AgentStateID.PathingState);
        }
    }

    public float GetStoppingDistance()
    {
        float targetRadius = Target.GetComponent<CapsuleCollider>().radius;

        float myRadius = Collider.radius;

        float minDistance = targetRadius + myRadius;

        if (CurrentAbility != null && stateMachine.currentState == AgentStateID.PathingState)
        {
            minDistance += CurrentAbility.Range;
        }

        return minDistance;
    }

    private bool CheckTargetOnRange()
    {
        if (ProjectTools.GetObjectOnRange(transform.position, Target, GetStoppingDistance()))
        {
            return true;
        }

        return false;
    }

    public void ChangeToAttack()
    {
        if (CheckTargetOnRange())
        {
            stateMachine.ChangeStates(AgentStateID.AttackState);
        }
    }

    private Coroutine attackCoroutine;
    private IEnumerator AttackingCoroutine()
    {   
        if (CurrentAbility.CastingCoroutine != null) 
        yield return CurrentAbility.CastingCoroutine; //Waits for current coroutine if not null

        CurrentAbility.Execute();           

        yield return new WaitForSeconds(CurrentAbility.CastTime + CurrentAbility.ChanneledTime);
        attackCoroutine = null;
        HandleDoneAttacking();
    }

    public void HandleDoneAttacking()
    {
        if (Target == null)
        {
            stateMachine.ChangeStates(AgentStateID.IdleState);
        }
        else if (CheckTargetOnRange())
        {
            attackCoroutine??= StartCoroutine(AttackingCoroutine());
        }
        else
        {
            stateMachine.ChangeStates(AgentStateID.PathingState);
        }
    }

    public void StartAttacking()
    {
        attackCoroutine ??= StartCoroutine(AttackingCoroutine());
    }

    public virtual void ChooseNextAbility() //Decision logic can be added here. Overwrite if you don't want random
    {
        int random = Random.Range(0, Abilities.Count);
        CurrentAbility = Abilities[random];
    }

    public void TrackTarget(Vector3 target)
    {
        trackingCoroutine??= StartCoroutine(TrackTargetCoroutine(target));
    }

    private Coroutine trackingCoroutine;
    private IEnumerator TrackTargetCoroutine(Vector3 target)
    {
        NavAgent.stoppingDistance = GetStoppingDistance();

        if (NavAgent.enabled)
        {
            NavAgent.SetDestination(target);
        }
        yield return new WaitForSeconds(trackDelay);
        trackingCoroutine = null;    
    }

    public void HandleDeath()
    {
        stateMachine.ChangeStates(AgentStateID.DeathState);
    }

    public virtual void IdleEnter()
    {

    }

    public virtual void IdleExit()
    {

    }

    public virtual void IdleUpdate()
    {
        DetectEnemy();
    }

    public virtual void AttackEnter()
    {
        NavAgent.enabled = false;
        StartAttacking();
    }

    public virtual void AttackExit()
    {
        NavAgent.enabled = true;
    }

    public virtual void AttackUpdate()
    {

    }

    public virtual void PathingEnter()
    {
        ChooseNextAbility();

    }

    public virtual void PathingExit()
    {

    }

    public virtual void PathingUpdate()
    {
        TrackTarget(Target.transform.position);
        ChangeToAttack();
    }

    public virtual void DeathEnter()
    {

    }

    public virtual void DeathExit()
    {

    }

    public virtual void DeathUpdate()
    {

    }

    public virtual void PeacefulEnter()
    {

    }

    public virtual void PeacefulExit()
    {

    }

    public virtual void PeacefulUpdate()
    {

    }

    public virtual void SpawningEnter()
    {

    }

    public virtual void SpawningExit()
    {

    }

    public virtual void SpawningUpdate()
    {

    }
 
}
