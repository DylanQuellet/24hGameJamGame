using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public KeyColor color;

    [Header("Références")]
    public Transform doorModel;
    public NavMeshObstacle navObstacle;

    [Header("Rotation")]
    [Tooltip("Rotation locale quand la porte est ouverte")]
    public Vector3 openRotationEuler;

    public float openSpeed = 2f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;

    void Start()
    {
        // On mémorise la rotation initiale comme position fermée
        closedRotation = doorModel.localRotation;

        // On calcule la rotation ouverte à partir de l'euler donné
        openRotation = Quaternion.Euler(openRotationEuler);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen) return;

        if (other.CompareTag("Player"))
        {
            PlayerKey inventory = other.GetComponent<PlayerKey>();

            if (inventory != null && inventory.HasKey(color))
            {
                OpenDoor();
            }
            else
            {
                Debug.Log("Il faut la clé : " + color);
            }
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        
        StartCoroutine(OpenAnimation());
    }

    System.Collections.IEnumerator OpenAnimation()
    {
        while (Quaternion.Angle(doorModel.localRotation, openRotation) > 0.1f)
        {
            doorModel.localRotation = Quaternion.Lerp(
                doorModel.localRotation,
                openRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        doorModel.localRotation = openRotation;
        navObstacle.enabled = false; // Désactive l'obstacle de navigation pour permettre le passage
    }
}
