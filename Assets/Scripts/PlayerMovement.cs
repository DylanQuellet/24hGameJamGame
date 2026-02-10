
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 10f;

    [Header("References")]
    public Animator animator;
    public Light lightShow;
    public Light lightHidden;

    private CharacterController controller;
    private float idleTimer = 0f;

    public bool BountyFound = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Initial state of lights
        lightShow.enabled = true;
        lightHidden.enabled = false;
    }

    private void Update()
    {
        // Input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        bool isMovingInput = move.magnitude > 0.1f;

        // Lights & idle timer
        if (!isMovingInput)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= 0.01f)
                animator.SetBool("isMoving", false);

            if (idleTimer >= 0.3f)
            {
                lightShow.enabled = true;
                lightHidden.enabled = false;
            }
        }
        else
        {
            idleTimer = 0f;

            lightShow.enabled = false;
            lightHidden.enabled = true;
        }

        // Animation
        animator.SetFloat("Speed", Mathf.Clamp01(move.magnitude / speed));

        // Rotation vers la direction du mouvement
        if (isMovingInput)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Movement via CharacterController
        Vector3 moveDir = move.normalized * speed;
        // On applique la gravité pour rester collé au sol
        moveDir.y = -9.81f * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);

        // Détecte que le joueur bouge pour l'animation
        animator.SetBool("isMoving", isMovingInput);
    }
}
