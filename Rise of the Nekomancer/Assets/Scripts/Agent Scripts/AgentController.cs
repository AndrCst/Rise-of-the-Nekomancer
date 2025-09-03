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
    public LayerMask VisibilityMask;
    private AgentStateMachine stateMachine;
    [SerializeField] private AgentStateID startingState;
    private float trackDelay = 0.1f;
    public CapsuleCollider Collider;
    public Ability CurrentAbility;
    public List<Ability> Abilities;
    public Damageable Damageable;
    public PlayerController PlayerController;
    public float FollowOffset;
    public float DeAggroRange;
    private bool recentlyDeaggroed;
    public const float DE_AGGRO_DURATION = 5;
    public const float DE_AGGRO_GRACE_TIME = 1.5f;

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

    private void OnEnable()
    {
        EntityEvents.OnSpellInterrupted += HandleInterruption;
        EntityEvents.OnEntityDied += HandleInterruption;
        EntityEvents.OnEntityDied += Retarget;
    }

    private void OnDisable()
    {
        EntityEvents.OnSpellInterrupted -= HandleInterruption;
        EntityEvents.OnEntityDied -= HandleInterruption;
        EntityEvents.OnEntityDied -= Retarget;
    }

    void GetStartInfo()
    {
        CurrentAbility = Abilities[0];

        NavAgent.speed = Damageable.BaseMovementSpeed;

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

        if (Damageable == null)
        {
            Damageable = GetComponent<Damageable>();
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
    }

    public void DetectEnemy()
    {
        if (recentlyDeaggroed)
        {
            return;
        }
       var targets = ProjectTools.GetValidObjectsOnRange(transform.position, DetectionRange, DetectedStrings, SearchedMask);

        if (targets.Count > 0)
        {
            var foundTarget = ProjectTools.GetNearestObject(targets, transform.position);            
            Target = foundTarget;
            stateMachine.ChangeStates(AgentStateID.PathingState);          
        }
    }

    public float GetStoppingDistance(GameObject target)
    {
        float targetRadius = target.GetComponent<CapsuleCollider>().radius;

        float myRadius = Collider.radius;

        float minDistance = targetRadius + myRadius;

        if (CurrentAbility != null)
        {
            if (stateMachine.currentState == AgentStateID.PathingState || stateMachine.currentState == AgentStateID.AttackState)
            {
                if (ProjectTools.GetTargetOnSight(Target, transform.position, CurrentAbility.Range, VisibilityMask))
                    minDistance += CurrentAbility.Range;
            }
           
        }
        else if (stateMachine.currentState == AgentStateID.IdleState)
        {
            minDistance += FollowOffset;
        }

            return minDistance;
    }

    private bool CheckTargetOnRange()
    {
        if (ProjectTools.GetObjectOnRange(transform.position, Target, GetStoppingDistance(Target)))
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
            attackCoroutine ??= StartCoroutine(AttackingCoroutine());
        }
        else
        {
            stateMachine.ChangeStates(AgentStateID.IdleState);
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
        if (NavAgent.enabled)
        {
            NavAgent.SetDestination(target);
        }
        yield return new WaitForSeconds(trackDelay);
        trackingCoroutine = null;    
    }

    public void CheckForDeAggro(GameObject checkedTarget)
    {
        if (!ProjectTools.GetTargetOnRange(transform.position, checkedTarget.transform.position, DeAggroRange) && !recentlyDeaggroed)
        {
            StartCoroutine(AggroTimeCoroutine(checkedTarget));
        }
    }

    public void Retarget(GameObject obj)
    {
        if (obj == Target)
        {
            EntityEvents.SpellInterrupted(obj);
            stateMachine.ChangeStates(AgentStateID.IdleState);
        }
    }


    IEnumerator AggroTimeCoroutine(GameObject checkedTarget)
    {
        yield return new WaitForSeconds(DE_AGGRO_GRACE_TIME);
        if (!ProjectTools.GetTargetOnRange(transform.position, checkedTarget.transform.position, DeAggroRange))
        {
            HandleDeaggro();
        }
    }

    public virtual void HandleDeaggro()
    {
        deaggroCoroutine ??= StartCoroutine(DeAggroCoroutine());
    }

    private Coroutine deaggroCoroutine;
    public IEnumerator DeAggroCoroutine()
    {
        recentlyDeaggroed = true;
        if (NavAgent.enabled)
        {
            NavAgent.ResetPath();
        }
        EntityEvents.SpellInterrupted(gameObject);
        stateMachine.ChangeStates(AgentStateID.IdleState);
        yield return new WaitForSeconds(DE_AGGRO_DURATION);
        recentlyDeaggroed = false;
        deaggroCoroutine = null;
    }

    public void HandleDeath()
    {
        stateMachine.ChangeStates(AgentStateID.DeathState);
    }

    void HandleInterruption(GameObject obj)
    {
        if (obj == gameObject)
        {
            InterruptSpell();
        }    

        if (obj == Target)
        {
            InterruptSpell();
        }
    }


    void InterruptSpell()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            HandleDoneAttacking();
        }    
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
        CheckForDeAggro(Target);
    }

    public virtual void PathingEnter()
    {
        NavAgent.stoppingDistance = GetStoppingDistance(Target);
        ChooseNextAbility();
    }

    public virtual void PathingExit()
    {

    }

    public virtual void PathingUpdate()
    {
        TrackTarget(Target.transform.position);
        CheckForDeAggro(Target);
        ChangeToAttack();
    }

    public virtual void DeathEnter()
    {
        //TEMPORARY!!
        EntityEvents.SpellInterrupted(gameObject);
        Destroy(gameObject);
        //var visuals = GetComponent<MeshRenderer>().enabled = false;
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
