using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public abstract class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform player;

    [SerializeField] private EnemyData enemyType;

    [SerializeField] LayerMask whatIsGround, whatIsPlayer;

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


    private void Awake()
    {
        player = GameObject.Find("Bean (Player)").transform;
        agent = GetComponent<NavMeshAgent>();
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

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();

        if (playerInSightRange && !playerInAttackRange) ChasePlayer();

        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
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

            Attack(attackDamage);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    protected abstract void Attack(float dmgAmount);

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
