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
    public GameObject GaryScreamer;
    public GameObject CochonScreamer;
    public float duration = 1f;       // durée totale de l'effet
    public float shakeInterval = 0.05f; // intervalle entre les rotations
    public float minZ = -17f;
    public float maxZ = -13f;

    [Header("Start Screen")]
    public GameObject startScreen;  // UI Canvas ou Panel qui explique l'histoire
    public float startScreenDuration = 5f; // durée d'affichage avant de commencer le jeu

    [Header("Death")]
    public GameObject deathPrefab;
    public int deathCount = 0;

    private Coroutine screamerCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BananaScreamer.SetActive(false);
        GaryScreamer.SetActive(false);
        CochonScreamer.SetActive(false);

        // Désactive le joueur au départ
        player.GetComponent<PlayerMovement>().controlsEnabled = false;

        // Affiche l'écran de début
        if (startScreen != null)
            startScreen.SetActive(true);

        // Lance la coroutine pour cacher l'écran et activer le jeu
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        // Attend la durée d'affichage
        yield return new WaitForSeconds(startScreenDuration);

        // Cache l'écran de début
        if (startScreen != null)
            startScreen.SetActive(false);

        // Active le contrôle du joueur
        player.GetComponent<PlayerMovement>().controlsEnabled = true;

        // Optionnel : tu peux activer d'autres systèmes de jeu ici
        GamePlaying = true;
    }

    public void Screamer(int WhichScreamer)
    {
        Debug.Log("Bouuuu!!!");

        player.GetComponentInParent<PlayerMovement>().controlsEnabled = false; // désactive le script de mouvement du joueur

        if (screamerCoroutine != null)
            StopCoroutine(screamerCoroutine);

        
        switch (WhichScreamer)
        {
            case 0:
                screamerCoroutine = StartCoroutine(ScreamerRoutine(BananaScreamer));
                break;
            case 1:
                screamerCoroutine = StartCoroutine(ScreamerRoutine(GaryScreamer));
                break;
            case 2:
                screamerCoroutine = StartCoroutine(ScreamerRoutine(CochonScreamer));
                break;
            default:
                Debug.LogWarning("Invalid screamer index: " + WhichScreamer);
                break;
        }
        
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
        player.SetActive(false);
        Vector3 DeathPoint = player.transform.position;
        float randomY = Random.Range(0f, 360f);
        Quaternion randomRot = Quaternion.Euler(0f, randomY, 0f);
        if (deathPrefab != null)
        {
            Instantiate(deathPrefab, DeathPoint, randomRot);
        }
        PlayerKey pKeys = player.GetComponent<PlayerKey>();
        KeyManager.Instance.ResetOwnedKeys(pKeys, DeathPoint);
        deathCount++;
        player.transform.position = spawnPoint.transform.position;
        player.SetActive(true);
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
        player.GetComponentInParent<PlayerMovement>().controlsEnabled = true; // active le script de mouvement du joueur
    }

}
