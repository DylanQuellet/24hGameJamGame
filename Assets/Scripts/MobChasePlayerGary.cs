using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobChasePlayerGary : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LightReceiver lightReceiver;
    public NavMeshAgent agent;
    public GameManager gameManager;

    [Header("Behaviour")]
    public float DeathDistance = 1.5f;

    private bool alerted = false; // le mob a été alerté au moins une fois

   
    void Start()
    {
        
        agent.isStopped = true; // départ immobile
    }

    void Update()
    {
        if (player == null || lightReceiver == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ----- Si le mob est éclairé : il devient alerté mais ne bouge pas -----
        if (lightReceiver.isIlluminated)
        {
            alerted = true;
            //agent.isStopped = true;
            return;
        }

        // ----- Si jamais alerté : reste immobile -----
        if (!alerted)
        {
            agent.isStopped = true;
            return;
        }

        // ----- Dès que alerté : poursuit toujours le joueur -----
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // ----- Stop si proche du joueur -----
        if (distanceToPlayer <= DeathDistance)
        {
            agent.isStopped = true;
            gameManager.Screamer(1);
            // L'état alerté reste actif, Gary ne “oublie” jamais
        }
    }

    // ----- Fonctions pour réinitialiser le mob au GameOver -----
    public void ResetMobStates()
    {
        agent.isStopped = true;
        alerted = false;
    }

}
