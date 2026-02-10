using UnityEngine;
using TMPro;

public class PaperRead : MonoBehaviour
{
    public Transform player;
    public float distance = 2f;

    public GameObject paperUI;
    public TextMeshProUGUI paperUIText;

    [TextArea(5,10)]
    public string paperContent;

    private bool isOpen;

    void Update()
    {
        float d = Vector3.Distance(player.position, transform.position);

        if (d <= distance && !isOpen)
        {
            Open();
        }
        else if (d > distance && isOpen)
        {
            Close();
        }
    }

    void Open()
    {
        isOpen = true;
        paperUI.SetActive(true);
        paperUIText.text = paperContent;
    }

    void Close()
    {
        isOpen = false;
        paperUI.SetActive(false);
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
