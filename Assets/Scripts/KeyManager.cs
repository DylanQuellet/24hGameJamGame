using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;

    private List<KeyPickup> allKeys = new List<KeyPickup>();

    private void Awake()
    {
        Instance = this;
    }

    // Appelé par chaque clé au Start
    public void RegisterKey(KeyPickup key)
    {
        if (!allKeys.Contains(key))
            allKeys.Add(key);
    }

    public void ResetAllKeys(Vector3 center)
    {
        // 1. Reset inventaire du joueur
        PlayerKey player = FindObjectOfType<PlayerKey>();

        if (player != null)
            player.ResetKeys();

        // 2. Pour chaque clé
        foreach (KeyPickup key in allKeys)
        {
            // Réactiver l'objet
            key.gameObject.SetActive(true);

            // Position aléatoire dans rayon 0.7
            Vector2 random = Random.insideUnitCircle * 0.7f;

            Vector3 newPos = center + new Vector3(random.x, 0, random.y);

            key.transform.position = newPos;

            // Reset interne de la clé
            key.ResetState();
        }

        Debug.Log("Toutes les clés ont été réinitialisées");
    }

    public void ResetOwnedKeys(PlayerKey player, Vector3 center)
    {
        Debug.Log("=== RESET START ===");

        if (player == null)
        {
            Debug.LogError("JOUEUR NON TROUVÉ");
            return;
        }

        List<KeyColor> owned = player.GetOwnedKeys();

        Debug.Log("Clés dans inventaire : " + owned.Count);
        Debug.Log("Clés connues par manager : " + allKeys.Count);

        foreach (KeyPickup key in allKeys)
        {
            Debug.Log("Test de la clé : " + key.color);

            if (owned.Contains(key.color))
            {
                Debug.Log("-> Clé à reset : " + key.color);

                key.gameObject.SetActive(true);

                Vector2 random = Random.insideUnitCircle * 1.2f;
                Vector3 newPos = center + new Vector3(random.x, 0, random.y);

                key.transform.position = newPos;

                float randomY = Random.Range(0f, 360f);
                key.transform.rotation = Quaternion.Euler(0f, randomY, 0f);

                key.ResetState();
            }
        }

        player.ResetKeys();

        Debug.Log("=== RESET FINI ===");
    }

}
