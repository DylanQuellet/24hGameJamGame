using UnityEngine;
using UnityEngine.AI;

public class MobChasePlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LightReceiver lightReceiver;
    public NavMeshAgent agent;

    [Header("Behaviour")]
    public float stopDistance = 1.5f;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null || lightReceiver == null)
            return;

        // S'IL EST ÉCLAIRÉ -> IL NE BOUGE PAS
        if (lightReceiver.isIlluminated)
        {
            agent.isStopped = true;
            return;
        }

        // PAS ÉCLAIRÉ -> IL ATTAQUE
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Optionnel : stop à distance
        if (Vector3.Distance(transform.position, player.position) <= stopDistance)
        {
            agent.isStopped = true;
        }
    }
}
