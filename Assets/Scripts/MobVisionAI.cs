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

    [Header("Audio")]
    public AudioSource audioSFX;      // pour marche, course, sniff
    public AudioSource audioMusic;    // pour la musique de course

    [Header("Sons")]
    public AudioClip walkClip;        // vitesse 0.5
    public AudioClip runClip;         // vitesse 1
    public AudioClip sniffClip;       // quand le mob perd le joueur

    private string currentAudioState = "";

    private bool pursuing = false;
    private bool lost = false;

    private Vector3 lastSeenPosition;
    private float lostTimer = 0f;

    private bool playerInDeath = false;

    private bool isSniffing = false;

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
        HandleAnimationAudio();
    }


    // ===================================================
    // AUDIO
    // ===================================================

    private void HandleAnimationAudio()
    {
        if (animator == null || audioSFX == null || audioMusic == null)
            return;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // ===== SI ON ÉTAIT EN SNIFF ET QU'ON CHANGE D'ÉTAT =====
        if (isSniffing)
        {
            if (state.IsName("Walk") || state.IsName("Run"))
            {
                // on coupe DIRECT le sniff
                audioSFX.Stop();
                isSniffing = false;
                currentAudioState = "";
            }
            else
            {
                // on reste en sniff tant que pas de marche/course
                return;
            }
        }


        // ===== RUN =====
        if (state.IsName("Run"))
        {
            if (currentAudioState != "run")
            {
                audioSFX.clip = runClip;
                audioSFX.pitch = 1f;
                audioSFX.Play();

                audioMusic.Play();

                currentAudioState = "run";
            }
            return;
        }

        // ===== WALK =====
        if (state.IsName("Walk"))
        {
            if (currentAudioState != "walk")
            {
                audioSFX.clip = walkClip;
                audioSFX.pitch = 0.5f;
                audioSFX.Play();

                audioMusic.Stop();

                currentAudioState = "walk";
            }
            return;
        }

        // ===== IDLE / AUTRE =====
        if (currentAudioState != "idle")
        {
            audioSFX.Stop();
            audioMusic.Stop();

            currentAudioState = "idle";
        }
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

        if (distance <= DeathDistance && !playerInDeath)
        {
            playerInDeath = true; // pour éviter de déclencher plusieurs fois le screamer
            gameManager.Screamer(2); // ou 1 ou 2 selon le mob
            return false; // on ne veut pas que le mob continue à poursuivre après avoir “tué” le joueur
        }

        if (distance > currentRadius)
        {
            playerInDeath = false; // réinitialise l'état de “mort” si le joueur s'éloigne suffisamment
            return false;
        }

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

        // FORCE le système audio à se réévaluer
        currentAudioState = "";

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
            if (!lost) // joue le son sniff une seule fois
            {
                lost = true;
                lostTimer = 0f;

                if (sniffClip != null)
                {
                    isSniffing = true;
                    audioSFX.Stop();
                    audioMusic.Stop();

                    // Puis on joue le sniff SEUL
                    audioSFX.clip = sniffClip;
                    audioSFX.pitch = 1f;
                    audioSFX.Play();

                    // on réautorise l’audio après la durée du clip
                    Invoke(nameof(EndSniff), sniffClip.length);
                }

                if (audioMusic.isPlaying)
                    audioMusic.Stop();
            }
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

    void EndSniff()
    {
        isSniffing = false;

        // Forcer retour vers marche
        currentAudioState = "";

        // On remet la vitesse de patrouille
        //agent.speed = patrolSpeed;
    }
}
