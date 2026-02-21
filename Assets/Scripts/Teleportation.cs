using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [Tooltip("Point où le joueur sera téléporté")]
    public Transform teleportPoint;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        // Vérifie que c'est bien le joueur
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();

            // Désactiver le CharacterController évite les problèmes de collision pendant le TP
            if (cc != null)
                cc.enabled = false;

            other.transform.position = teleportPoint.position;

            if (cc != null)
                cc.enabled = true;
        }
    }
}
