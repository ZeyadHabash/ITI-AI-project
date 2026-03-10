using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class TrollChargerAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    private NavMeshAgent navAgent;
    private Animator animator;

    [Header("Health & Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Settings")]
    [SerializeField] private float aggroRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float jumpAttackRange = 5f;
    [SerializeField] private float minPlayerOffset = 2.5f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderWaitTime = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackDuration = 1.5f;

    private Node rootNode;
    private bool hasJumpAttacked = false;
    private float lastAttackTime = -999f;
    private float nextWanderTime = -999f;
    private Vector3 wanderTarget;

    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int jumpAttackHash = Animator.StringToHash("JumpAttack");
    private readonly int dieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        ConstructBehaviorTree();
        GetNewWanderTarget();
    }

    private void Update()
    {
        if (isDead) return;

        rootNode?.Evaluate();

        if (navAgent.isStopped)
        {
            animator.SetFloat(speedHash, 0f);
        }
        else
        {
            animator.SetFloat(speedHash, navAgent.velocity.magnitude / navAgent.speed);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        navAgent.isStopped = true;
        navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;
        navAgent.enabled = false;

        animator.SetTrigger(dieHash);

        Collider coll = GetComponent<Collider>();
        if (coll != null) coll.enabled = false;

        Destroy(gameObject, 5f);
    }

    private void ConstructBehaviorTree()
    {
        var attackLock = new ActionNode(CheckIsAttacking);
        var seqJump = new Sequence(new List<Node> { new ActionNode(CheckJumpAttack), new ActionNode(DoJumpAttack) });
        var seqNormal = new Sequence(new List<Node> { new ActionNode(CheckNormalAttack), new ActionNode(DoNormalAttack) });
        var combatSelector = new Selector(new List<Node> { attackLock, seqJump, seqNormal, new ActionNode(ChasePlayer) });

        var combatSequence = new Sequence(new List<Node> { new ActionNode(CheckAggro), combatSelector });

        rootNode = new Selector(new List<Node> { attackLock, combatSequence, new ActionNode(DoWander) });
    }

    private NodeState CheckIsAttacking()
    {
        if (Time.time < lastAttackTime + attackDuration)
        {
            navAgent.isStopped = true;
            if (navAgent.hasPath) navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;
            return NodeState.Running;
        }
        return NodeState.Failure;
    }

    private NodeState CheckAggro()
    {
        if (playerTransform == null) return NodeState.Failure;
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        return dist <= aggroRange ? NodeState.Success : NodeState.Failure;
    }

    private NodeState CheckJumpAttack()
    {
        if (hasJumpAttacked) return NodeState.Failure;
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        return dist <= jumpAttackRange ? NodeState.Success : NodeState.Failure;
    }

    private NodeState DoJumpAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return NodeState.Failure;
        navAgent.isStopped = true;
        if (navAgent.hasPath) navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;
        animator.SetTrigger(jumpAttackHash);
        hasJumpAttacked = true;
        lastAttackTime = Time.time;
        FacePlayer();
        return NodeState.Success;
    }

    private NodeState CheckNormalAttack()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        return dist <= attackRange ? NodeState.Success : NodeState.Failure;
    }

    private NodeState DoNormalAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return NodeState.Failure;
        navAgent.isStopped = true;
        if (navAgent.hasPath) navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;
        animator.SetTrigger(attackHash);
        lastAttackTime = Time.time;
        FacePlayer();
        return NodeState.Success;
    }

    private NodeState ChasePlayer()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= minPlayerOffset)
        {
            navAgent.isStopped = true;
            if (navAgent.hasPath) navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;
            FacePlayer();
            return NodeState.Success;
        }

        navAgent.isStopped = false;
        navAgent.SetDestination(playerTransform.position);
        return NodeState.Running;
    }

    private NodeState DoWander()
    {
        hasJumpAttacked = false;

        if (Vector3.Distance(transform.position, wanderTarget) < 1.5f)
        {
            navAgent.isStopped = true;
            if (navAgent.hasPath) navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;

            if (Time.time >= nextWanderTime)
            {
                GetNewWanderTarget();
            }
            return NodeState.Success;
        }

        navAgent.isStopped = false;
        navAgent.SetDestination(wanderTarget);
        return NodeState.Running;
    }

    private void GetNewWanderTarget()
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            nextWanderTime = Time.time + wanderWaitTime;
        }
    }

    private void FacePlayer()
    {
        Vector3 dir = (playerTransform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);
    }
}
