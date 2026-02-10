using System.Collections;
using UnityEngine;

public class LightningTrigger : MonoBehaviour
{
    [Header("Lightning Settings")]
    public Light lightningLight;
    public AudioSource lightningAudio;

    public int lightningCount = 5;

    public float minInterval = 0.2f;
    public float maxInterval = 0.8f;

    public float flashMinIntensity = 500f;
    public float flashMaxIntensity = 1500f;
    public float flashDuration = 0.08f;

    private bool triggered = false;

    private void Start()
    {
        if (lightningLight != null)
            lightningLight.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // On vérifie que c’est bien le joueur
        if (other.GetComponent<PlayerMovement>() != null)
        {
            triggered = true;
            StartCoroutine(LightningSequence());
        }
    }

    private IEnumerator LightningSequence()
    {
        for (int i = 0; i < lightningCount; i++)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            yield return StartCoroutine(DoFlash());
        }

        // Désactive le trigger à la fin
        Destroy(gameObject);
    }

    private IEnumerator DoFlash()
    {
        if (lightningLight == null)
            yield break;

        // ----- SON -----
        if (lightningAudio != null)
        {
            lightningAudio.pitch = Random.Range(0.95f, 1.05f);
            lightningAudio.PlayOneShot(lightningAudio.clip);
        }

        // ----- FLASH VISUEL -----
        lightningLight.enabled = true;
        lightningLight.intensity =
            Random.Range(flashMinIntensity, flashMaxIntensity);

        yield return new WaitForSeconds(flashDuration);

        lightningLight.enabled = false;
    }
}
