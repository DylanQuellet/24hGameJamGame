using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public KeyColor color;

    private bool isCollected = false;

    private void Start()
    {
        KeyManager.Instance.RegisterKey(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            PlayerKey inventory = other.GetComponent<PlayerKey>();

            if (inventory != null)
            {
                inventory.AddKey(color);

                isCollected = true;

                // On désactive au lieu de détruire
                gameObject.SetActive(false);
            }
        }
    }

    public void ResetState()
    {
        isCollected = false;
    }
}
