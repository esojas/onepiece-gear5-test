using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public abstract class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform player;

    [SerializeField] private EnemyData enemyType;

    [SerializeField] LayerMask whatIsGround, whatIsPlayer;

    private AnimationScript animationScript;
    private string currentAnimation;
    protected bool isAttacking = false;

    // Patrolling
    private Vector3 walkPoint;
    bool walkPointSet;
    private float walkPointRange;

    //Attacking
    private float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    private float sightRange, attackRange, attackDamage;
    private bool playerInSightRange, playerInAttackRange;

    //Idle
    private float idleTime = 2f;        
    private float idleTimer = 0f;
    private bool isIdling = false;

    private void Awake()
    {
        player = GameObject.Find("luffy-test-backup").transform;
        agent = GetComponent<NavMeshAgent>();
        animationScript = GetComponent<AnimationScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sightRange = enemyType.sightRange;
        attackRange = enemyType.attackRange;
        walkPointRange = enemyType.walkPointRange;
        timeBetweenAttacks = enemyType.timeBetweenAttacks;
        attackDamage = enemyType.attackDamage;
    }

    private void OnEnable()
    {
        animationScript.CurrentAnimationEvent += SetAnimation;
    }

    private void OnDisable()
    {
        animationScript.CurrentAnimationEvent -= SetAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        CheckAnimation();
        if(player != null) TrackPlayer(player);

        if (!playerInSightRange && !playerInAttackRange)
        {
            OnPlayerLeftRange();
            Patrolling();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            OnPlayerLeftRange();
            ChasePlayer();
        }
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void SetAnimation(string animation)
    {
        currentAnimation = animation;
        if (animation == "enemy_idle" || animation == "enemy_walk")
        {
            isAttacking = false;
        }
    } 

    private void CheckAnimation()
    {
        if (isAttacking) return;

        if (agent.velocity.magnitude > 0.1f)
        {
            animationScript.ChangeAnimation("enemy_walk");
        }
        else
        {
            animationScript.ChangeAnimation("enemy_idle");
        }
    }

    private void Patrolling()
    {
        if (isIdling)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdling = false;
            }
            return; 
        }

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
            isIdling = true;            
            idleTimer = idleTime;        
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);


        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(playerPos);

        if (!alreadyAttacked)
        {
            Attack(attackDamage, animationScript);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    protected abstract void Attack(float dmgAmount, AnimationScript animation);

    protected virtual void OnPlayerLeftRange() { }

    protected virtual void TrackPlayer(Transform player) { }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
