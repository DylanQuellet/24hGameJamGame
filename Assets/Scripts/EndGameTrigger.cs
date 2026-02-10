using UnityEngine;
using TMPro; // pour TextMeshPro

public class EndGameTrigger : MonoBehaviour
{
    [Header("GameManager Reference")]
    public GameManager gameManager;  // assigné depuis l'inspector

    [Header("UI References")]
    public GameObject endScreen;      // Panel/UI à activer
    public TMP_Text endScreenText;    // Texte à mettre à jour

    [Header("Settings")]
    public string playerTag = "Player";

    private bool triggered = false;

    private void Start()
    {
        // Assure que l'écran est désactivé au départ
        if (endScreen != null)
            endScreen.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Vérifie que c'est bien le joueur
        if (!other.CompareTag(playerTag)) return;

        triggered = true;

        // Désactive le contrôle du joueur
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.controlsEnabled = false;

        // Active l'écran de fin
        if (endScreen != null)
            endScreen.SetActive(true);

        // Met à jour le texte avec le nombre de morts
        if (endScreenText != null && gameManager != null)
        {
            int nbMorts = gameManager.deathCount;
            endScreenText.text = $"Au final, il y aura eu...\n{nbMorts} mort(s) lors de cette nuit.";
        }

        // Optionnel : désactive le trigger pour qu'il ne se déclenche plus
        gameObject.SetActive(false);
    }
}
