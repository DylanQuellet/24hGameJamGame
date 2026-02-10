using UnityEngine;
using UnityEngine.AI;

public class MobVisionAI : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public Animator animator;
    public NavMeshAgent agent;
    public GameManager gameManager;

    [Header("Vision")]
    public float viewRadiusMoving = 10f;
    public float viewRadiusIdle = 4f;
    public LayerMask obstacleMask;

    [Header("Vitesse")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Patrouille")]
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Recherche")]
    public float lostDelay = 5f;

    [Header("Behaviour")]
    public float DeathDistance = 0.5f;

    private bool pursuing = false;
    private bool lost = false;

    private Vector3 lastSeenPosition;
    private float lostTimer = 0f;

    void Start()
    {
        agent.speed = patrolSpeed;
        GoToNextWaypoint();
    }

    void Update()
    {
        bool canSee = CanSeePlayer();

        // ===== SI ON VOIT LE JOUEUR =====
        if (canSee)
        {
            lastSeenPosition = player.position;

            if (!pursuing)
                StartPursuit();

            lost = false;
            lostTimer = 0f;

            agent.destination = player.position;
        }

        // ===== SI ON LE VOYAIT MAIS PLUS MAINTENANT =====
        else if (pursuing)
        {
            HandleLostState();
        }

        // ===== MODE PATROUILLE =====
        else
        {
            PatrolBehaviour();
        }

        UpdateAnimator();
    }

    // ===================================================
    // VISION
    // ===================================================

    bool CanSeePlayer()
    {
        // 1. Récupérer l'état de mouvement depuis TON script
        PlayerMovement pm = player.GetComponent<PlayerMovement>();

        bool isMoving = false;

        if (pm != null)
            isMoving = pm.IsMoving;

        // 2. Choix du rayon selon déplacement
        float currentRadius = isMoving ? viewRadiusMoving : viewRadiusIdle;

        // 3. Test distance
        Vector3 dir = player.position - transform.position;
        float distance = dir.magnitude;

        if (distance <= DeathDistance)
        {
            gameManager.Screamer(2); // ou 1 ou 2 selon le mob
            return false; // on ne veut pas que le mob continue à poursuivre après avoir “tué” le joueur
        }

        if (distance > currentRadius)
            return false;

        dir.Normalize();

        // 4. Test obstacles
        if (Physics.Raycast(
            transform.position + Vector3.up * 1.5f,
            dir,
            distance,
            obstacleMask))
        {
            return false;
        }

        return true;
    }

    // ===================================================
    // ÉTATS
    // ===================================================

    void StartPursuit()
    {
        pursuing = true;
        agent.speed = chaseSpeed;
    }

    void StopPursuit()
    {
        pursuing = false;
        lost = false;
        agent.speed = patrolSpeed;
        GoToNextWaypoint();
    }

    void HandleLostState()
    {
        // Aller au dernier point vu
        agent.destination = lastSeenPosition;

        // Si on est arrivé
        if (!agent.pathPending &&
            agent.remainingDistance < 0.5f)
        {
            lost = true;
            lostTimer += Time.deltaTime;

            // Après 5s on abandonne
            if (lostTimer > lostDelay)
            {
                StopPursuit();
            }
        }
    }

    // ===================================================
    // PATROUILLE
    // ===================================================

    void PatrolBehaviour()
    {
        if (waypoints.Length == 0) return;

        if (!agent.pathPending &&
            agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.destination = waypoints[currentWaypoint].position;

        currentWaypoint =
            (currentWaypoint + 1) % waypoints.Length;
    }

    // ===================================================
    // ANIMATOR
    // ===================================================

    void UpdateAnimator()
    {
        animator.SetBool("Poursuite", pursuing);
        animator.SetBool("Lost", lost);
    }

    // ===================================================
    // DEBUG
    // ===================================================

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadiusIdle);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadiusMoving);

        if (pursuing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastSeenPosition, 0.2f);
        }
    }
}
