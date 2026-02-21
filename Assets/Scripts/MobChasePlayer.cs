using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobChasePlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LightReceiver lightReceiver;
    public NavMeshAgent agent;
    public GameManager gameManager;

    [Header("Behaviour")]
    public float DeathDistance = 0.5f;
    public float chaseDistance = 15f;

    private bool alerted = false; // le mob a été éclairé au moins une fois
    private Vector3 spawnPosition; // position d'origine

    private static List<MobChasePlayer> allMobs = new List<MobChasePlayer>();

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        // Sauvegarde la position de spawn
        spawnPosition = transform.position;

        allMobs.Add(this); // Ajoute ce mob à la liste globale
    }

    void Update()
    {
        if (player == null || lightReceiver == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si le mob est éclairé -> il ne bouge pas mais devient alerté
        if (lightReceiver.isIlluminated)
        {
            alerted = true;
            agent.isStopped = true; // reste immobile
            return;
        }

        // Si le mob n’a jamais été alerté -> ne rien faire
        if (!alerted)
        {
            agent.isStopped = true;
            return;
        }

        // Si le joueur est trop loin -> arrête la poursuite et on rénitialise l'état
        if (distanceToPlayer > chaseDistance)
        {
            agent.isStopped = true;
            alerted = false;
            return;
        }

        // Si alerté et pas éclairé -> poursuit le joueur
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Stop à courte distance
        if (distanceToPlayer <= DeathDistance)
        {
            agent.isStopped = true;
            alerted = false;
            gameManager.Screamer(0);
        }
    }

    // Fonctions pour réinitialiser le mob au GameOver
    public void ResetMobStates()
    {
        agent.isStopped = true;
        alerted = false;
    }
    public void ResetMobPosition()
    {
        agent.Warp(spawnPosition); // repositionne instantanément le 
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
