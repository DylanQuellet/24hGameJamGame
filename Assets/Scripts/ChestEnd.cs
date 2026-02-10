using UnityEngine;
using TMPro;

public class ChestEnd : MonoBehaviour
{
    public GameObject ChestUI;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ChestUI.SetActive(true);
    }
}
    // }
    // void Update()
    // {
    //     // Press E to toggle
    //     if (playerInRange && Input.GetKeyDown(KeyCode.E))
    //     {
    //         if (isOpen)
    //             ClosePaper();
    //         else
    //             OpenPaper();
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (!other.CompareTag("Player")) return;

    //     playerInRange = true;
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (!other.CompareTag("Player")) return;

    //     playerInRange = false;
    //     ClosePaper();
    // }

    // void OpenPaper()
    // {
    //     if (isOpen) return;

    //     isOpen = true;

    //     paperUI.SetActive(true);
    //     paperUIText.text = paperContent;
    // }

    // void ClosePaper()
    // {
    //     if (!isOpen) return;

    //     isOpen = false;

    //     paperUI.SetActive(false);
    // }
