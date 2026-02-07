using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 10f;

    [Header("References")]
    public Animator animator;
    public Light lightShow;
    public Light lightHidden;

    private Rigidbody rb;
    private float idleTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // Initial state of lights
        lightShow.enabled = true;
        lightHidden.enabled = false;
    }

    private void Update()
    {
        // Gestion des lights et idle timer
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        bool isMovingInput = move.magnitude > 0.1f;

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
    }

    private void FixedUpdate()
    {
        // Mouvement physique
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);

        if (move.magnitude > 0.1f)
        {
            // Déplacement via Rigidbody (collisions gérées)
            rb.MovePosition(rb.position + move.normalized * speed * Time.fixedDeltaTime);

            // Rotation vers la direction du mouvement
            Quaternion targetRotation = Quaternion.LookRotation(move);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            // Animation
            animator.SetFloat("Speed", move.magnitude / speed);
            animator.SetBool("isMoving", true);
        }
    }
}
