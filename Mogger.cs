using UnityEngine;
using UnityEngine.AI;

public class Mogger : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;

    [Header("Movement Settings")]
    public float wanderRadius = 15f;
    public float wanderInterval = 3f; // Pick new destination (not lplr) every 3 seconds
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;

    [Header("Combat & Detection")]
    public float detectionRadius = 50f; // detections ig
    public float attackDistance = 18f;
    public float ultimateDistance = 0.5f;

    // health
    [Header("HP")]
    public int maxHP = 10;        
    public int currentHP;

    [Header("Vision Settings")]
    public float fieldOfView = 160f;   // in degrees
    public float loseSightRadius = 60f; // lose sight of player

    // FSM States
    enum State { 
        Roam, 
        Chase, 
        Attack, 
        Ultimate 
    }

    State state;

    Vector3 roamTarget;
    float roamTimer = 0f;
    bool hasInitialized = false;

    // crouch bool
    bool crouchDelayActive = false;
    float crouchDelayTimer = 0f;

    // crouch delay so AI will take 3 seconds to actually chase u if ur crouching
    // perfect for escape ig
    public float crouchDelayDuration = 3f;

    public PlayerMovement pm;

    void Start()
    {
        // max health
        currentHP = maxHP;

        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.stoppingDistance = 0f; // 0 stopping distance
        agent.autoBraking = false; // never stops

        state = State.Roam;
    }

    void Update()
    {
        // if no lplr then return
        if (!player) return;

        // Force initialization on first frame
        if (!hasInitialized)
        {
            agent.isStopped = false;
            SetNewRoamTarget();
            hasInitialized = true;
        }
        // every frame runs these so continous
        EvaluateState();
        TickState();
    }

    // evaluate states based off distance from AI to lplr
    void EvaluateState()
    {
        bool sees = CanSeePlayer();
        bool isCrouching = pm != null && pm.isCrouching; // your crouch bool

        if (sees)
        {
            // if crouching and haven't activated the delay yet
            if (isCrouching && !crouchDelayActive)
            {
                crouchDelayActive = true;
                crouchDelayTimer = crouchDelayDuration;
            }

            // while delayed, countdown
            if (crouchDelayActive)
            {
                crouchDelayTimer -= Time.deltaTime;
                if (crouchDelayTimer > 0f)
                {
                    // AI sees player but is hesitating because crouch stealth
                    return;
                }
                else
                {
                    crouchDelayActive = false; // delay finished, we can react now
                }
            }

            float dist = Vector3.Distance(transform.position, player.position);

            if (dist <= ultimateDistance)
                state = State.Ultimate;
            else if (dist <= attackDistance)
                state = State.Attack;
            else
                state = State.Chase;
        }
        else
        {
            // reset if lose sight
            crouchDelayActive = false;
            state = State.Roam;
        }
    }


    void TickState()
    {
        // switches state
        switch (state)
        {
            case State.Roam:
                Roam();
                UpdateAnimator();
                break;

            case State.Chase:
                ChasePlayer();
                UpdateAnimator();
                break;

            case State.Attack:
                AttackPlayer();
                UpdateAnimator();
                break;

            case State.Ultimate:
                UltimateAttack();
                UpdateAnimator();
                break;
        }
    }

    // === States ===
    void Roam()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.autoBraking = false;

        roamTimer += Time.deltaTime;

        // Pick new target every 3 seconds NO MATTER WHAT
        if (roamTimer >= wanderInterval)
        {
            SetNewRoamTarget();
            roamTimer = 0f;
        }
    }

    // chase player
    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
        LookAtTarget(player.position);
    }
//updates the animator 
    void UpdateAnimator()
{
    if (!animator) return;

    animator.SetBool("isWalking", state == State.Roam);
    animator.SetBool("isRunning", state == State.Chase);

    if (state == State.Attack)
        animator.SetTrigger("attack");

    if (state == State.Ultimate)
        animator.SetTrigger("ultimate");
}

    // attack player
    void AttackPlayer()
    {
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
        LookAtTarget(player.position);
        // should play anim but idk how to do it
    }

    // "eats" player
    void UltimateAttack()
    {
        agent.isStopped = true;
        LookAtTarget(player.position);

        // this should call the game over screen idk what the others are doing
    }

    // === Helpers ===
    void SetNewRoamTarget()
    {
        // sets the pos by calling the pos function, and AI goes to that random pos
        roamTarget = GetRandomRoamPosition();
        agent.SetDestination(roamTarget);
    }

    Vector3 GetRandomRoamPosition()
    {
        // random pos to roam
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius + transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }


    // face target
    private void LookAtTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0; // Keep rotation on horizontal plane only

        // if target extreme close then no rotate otherwise face target ig
        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 7f);
        }
    }

    // Returns true if Mogger can see the player
    bool CanSeePlayer()
    {
        if (!player) return false;

        Vector3 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        // Out of sight range
        if (distanceToPlayer > loseSightRadius) return false;

        // Outside field of view
        if (fieldOfView < 180f)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, toPlayer.normalized);
            if (angleToPlayer > fieldOfView * 0.5f) return false;
        }

        // Check for obstacles
        return HasLineOfSight(player.position);
    }

    // Checks if there are no obstacles blocking vision
    bool HasLineOfSight(Vector3 targetPos)
    {
        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        Vector3 targetEye = targetPos + Vector3.up * 1.5f;
        Vector3 direction = targetEye - eyePosition;

        if (!Physics.Raycast(eyePosition, direction.normalized, out RaycastHit hit, direction.magnitude, ~0, QueryTriggerInteraction.Ignore))
            return true; // nothing blocking

        return hit.collider.CompareTag("Player");
    }

    public void TakeDamage(int amount)
    {
        // simple - you minus the current hp, hp loss ig
        currentHP -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage! HP: " + currentHP);

        // dies if hp is less than 0
        if (animator) animator.SetTrigger("damaged"); // optional death animation
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");

        //  disable AI
        if (agent != null) agent.isStopped = true;
        enabled = false;
        if (animator) animator.SetTrigger("die"); // optional death animation

        // place holder

        // kills AI ggwp
        Destroy(gameObject, 5f);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if collided object is a child of "Rock"
        Transform t = other.transform;
        while (t != null)
        {
            if (t.name == "Rock")
            {
                TakeDamage(2); // or whatever damage amount
                break;
            }
            t = t.parent;
        }
    }

    // === Animation ===
    void PlayRoamAnimation()
    {
        if (!animator) return;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
    }

    void PlayRunAnimation()
    {
        if (!animator) return;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
    }

    void PlayAttackAnimation()
    {
        if (!animator) return;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetTrigger("attack");
    }

    void PlayUltimateAnimation()
    {
        if (!animator) return;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetTrigger("ultimate");
    }

}
