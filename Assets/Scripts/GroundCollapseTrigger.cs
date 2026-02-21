using System.Collections;
using UnityEngine;

public class GroundCollapseTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject normalGround;
    public GameObject brokenGround;

    [Header("Shake Settings")]
    public float shakeDuration = 2.5f;
    public float shakeStrength = 0.08f;
    public float shakeSpeed = 25f;

    [Header("Lightning Settings")]
    public Light lightningLight;
    public AudioSource lightningAudio;

    public float minLightningInterval = 0.2f;
    public float maxLightningInterval = 0.8f;

    public float flashMinIntensity = 2f;
    public float flashMaxIntensity = 6f;
    public float flashDuration = 0.08f;

    private bool triggered = false;

    private void Start()
    {
        if (normalGround != null) normalGround.SetActive(true);
        if (brokenGround != null) brokenGround.SetActive(false);

        if (lightningLight != null)
            lightningLight.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            triggered = true;
            StartCoroutine(CollapseSequence(player));
        }
    }

    private IEnumerator CollapseSequence(PlayerMovement player)
    {
        player.controlsEnabled = false;

        float elapsed = 0f;
        Vector3 basePos = normalGround.transform.localPosition;

        CharacterController cc = player.GetComponent<CharacterController>();

        // Lance les éclairs en parallèle
        StartCoroutine(LightningRoutine());

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeStrength;
            float offsetZ = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * shakeStrength;

            Vector3 delta = new Vector3(offsetX, 0, offsetZ);

            normalGround.transform.localPosition = basePos + delta;

            cc.Move(delta);

            yield return null;
        }

        normalGround.transform.localPosition = basePos;

        normalGround.SetActive(false);
        brokenGround.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        player.controlsEnabled = true;

        Destroy(gameObject);
    }

    // =============================
    // SYSTÈME D’ÉCLAIRS
    // =============================

    private IEnumerator LightningRoutine()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            // Attente aléatoire entre éclairs
            float wait = Random.Range(minLightningInterval, maxLightningInterval);
            yield return new WaitForSeconds(wait);

            yield return StartCoroutine(DoFlash());

            timer += wait;
        }
    }

    private IEnumerator DoFlash()
    {
        if (lightningLight == null)
            yield break;

        // Son
        if (lightningAudio != null)
        {
            lightningAudio.pitch = Random.Range(0.95f, 1.05f);
            lightningAudio.PlayOneShot(lightningAudio.clip);
        }

        // Flash
        lightningLight.enabled = true;
        lightningLight.intensity = Random.Range(flashMinIntensity, flashMaxIntensity);

        yield return new WaitForSeconds(flashDuration);

        lightningLight.enabled = false;
    }
}
