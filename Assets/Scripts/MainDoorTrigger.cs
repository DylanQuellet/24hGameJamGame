using System.Collections;
using UnityEngine;

public class MainDoorTrigger : MonoBehaviour
{
    [Header("Doors")]
    public Transform doorLeft;   // porte à tourner vers -40°
    public Transform doorRight;  // porte à tourner vers +40°
    public float openAngleLeft = -40f;
    public float openAngleRight = 40f;
    public float openDuration = 3f; // durée de l'ouverture

    [Header("Audio")]
    public AudioSource doorAudio; // son de grincement

    [Header("Settings")]
    public string playerTag = "Player";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag(playerTag)) return;

        triggered = true;

        // Joue le son de porte
        if (doorAudio != null)
        {
            doorAudio.pitch = Random.Range(0.4f, 0.5f); // légère variation
            doorAudio.Play();
        }

        // Lance l'animation des portes
        StartCoroutine(OpenDoors());
    }

    private IEnumerator OpenDoors()
    {
        if (doorLeft == null || doorRight == null)
            yield break;

        Quaternion startLeft = doorLeft.localRotation;
        Quaternion startRight = doorRight.localRotation;

        Quaternion targetLeft = Quaternion.Euler(doorLeft.localEulerAngles.x, openAngleLeft, doorLeft.localEulerAngles.z);
        Quaternion targetRight = Quaternion.Euler(doorRight.localEulerAngles.x, openAngleRight, doorRight.localEulerAngles.z);

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);

            // Interpolation lisse
            doorLeft.localRotation = Quaternion.Slerp(startLeft, targetLeft, t);
            doorRight.localRotation = Quaternion.Slerp(startRight, targetRight, t);

            yield return null;
        }

        // S'assure que les portes sont exactement à la cible
        doorLeft.localRotation = targetLeft;
        doorRight.localRotation = targetRight;
    }
}
