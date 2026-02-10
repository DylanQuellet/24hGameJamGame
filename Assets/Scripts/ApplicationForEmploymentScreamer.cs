using UnityEngine;
using System.Collections;

public class ApplicationForEmploymentScreamer : MonoBehaviour
{

    [Header("Player")]
    public GameObject player;

    [Header("Screamers")]
    public GameObject ApplicationScreamer;
    public float duration = 1f;       // durée totale de l'effet
    public float shakeInterval = 0.05f; // intervalle entre les rotations
    public float minZ = -17f;
    public float maxZ = -13f;

    [Header("AplicationForEmploymentObject")]
    public GameObject ApplicationForEmploymentObject;

    private Coroutine screamerCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplicationScreamer.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Screamer();
        }
    }

    public void Screamer()
    {
        Debug.Log("Bouuuu!!!");

        player.GetComponentInParent<PlayerMovement>().controlsEnabled = false; // désactive le script de mouvement du joueur

        if (screamerCoroutine != null)
            StopCoroutine(screamerCoroutine);

        screamerCoroutine = StartCoroutine(ScreamerRoutine(ApplicationScreamer));
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
        ApplicationForEmploymentObject.SetActive(false); // Désactive l'objet de l'application pour l'emploi
        screamerObject.SetActive(false);
        ContinuePlaying();

        screamerCoroutine = null;
    }

    public void ContinuePlaying()
    {
        player.GetComponentInParent<PlayerMovement>().controlsEnabled = true; // active le script de mouvement du joueur
    }
}
    
