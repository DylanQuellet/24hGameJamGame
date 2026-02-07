using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("GameStates")]
    public bool GamePlaying = true;
    public bool GamePaused = false;
    public bool GameOver = false;

    [Header("Player")]
    public GameObject player;
    public GameObject spawnPoint = null;

    [Header("Screamers")]
    public GameObject BananaScreamer;
    public float duration = 1f;       // durée totale de l'effet
    public float shakeInterval = 0.05f; // intervalle entre les rotations
    public float minZ = -17f;
    public float maxZ = -13f;

    private Coroutine screamerCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BananaScreamer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Screamer()
    {
        Debug.Log("Bouuuu!!!");

        if (screamerCoroutine != null)
            StopCoroutine(screamerCoroutine);

        screamerCoroutine = StartCoroutine(ScreamerRoutine(BananaScreamer));
    }

    private IEnumerator ScreamerRoutine(GameObject screamerObject)
    {
        // Active l'objet
        screamerObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Rotation aléatoire sur Z
            float randomZ = Random.Range(minZ, maxZ);
            screamerObject.transform.localRotation = Quaternion.Euler(0f, 180f, randomZ);

            // Attend un petit intervalle
            yield return new WaitForSeconds(shakeInterval);

            elapsed += shakeInterval;
        }

        // Désactive l'objet
        screamerObject.SetActive(false);
        player.transform.position = spawnPoint.transform.position;
        // réinitialise tous les mobs
        yield return new WaitForSeconds(0.1f);
        MobChasePlayer.ResetAllMobsPositions();
        yield return new WaitForSeconds(0.1f);
        MobChasePlayer.ResetAllMobsStates();
        InitGameOver();
        screamerCoroutine = null;
    }

    public void InitGameOver()
    {
        Debug.Log("ResetMobs");
    }

}
