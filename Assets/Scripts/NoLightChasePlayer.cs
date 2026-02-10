using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NoLightChasePlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LightReceiver lightReceiver;
    public NavMeshAgent agent;
    public GameManager gameManager;

    [Header("Behaviour")]
    public float stopDistance = 1.5f;
    public float chaseDistance = 15f;

    private Vector3 spawnPosition; // position d'origine

    private static List<NoLightChasePlayer> allMobs = new List<NoLightChasePlayer>();

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        // Sauvegarde la position de spawn
        spawnPosition = transform.position;

        allMobs.Add(this); // Ajoute ce mob � la liste globale
    }

    void Update()
    {
        if (player == null || lightReceiver == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si le mob est �clair� -> il ne bouge pas mais devient alert�
        if (lightReceiver.isIlluminated)
        {
            agent.speed = 100;
            return;
        }
        else
        {
            agent.speed = 0.2f;
        }


        // Si alert� et pas �clair� -> poursuit le joueur
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Stop � courte distance
        if (distanceToPlayer <= stopDistance)
        {
            agent.isStopped = true;
            gameManager.Screamer();
        }
    }

    // Fonctions pour r�initialiser le mob au GameOver
    public void ResetMobStates()
    {
        agent.isStopped = true;
    }
    public void ResetMobPosition()
    {
        agent.Warp(spawnPosition); // repositionne instantan�ment le 
    }

    // Fonction statique pour reset tous les mobs
    public static void ResetAllMobsStates()
    {
        foreach (var mob in allMobs)
        {
            mob.ResetMobStates();
        }
    }
    public static void ResetAllMobsPositions()
    {
        foreach (var mob in allMobs)
        {
            mob.ResetMobPosition();
        }
    }
}
