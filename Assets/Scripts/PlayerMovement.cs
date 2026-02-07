using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 10f;
    public Animator animator;
    public Light lightShow;
    public Light lightHidden;

    private float idleTimer = 0f;

    private void Start()
    {
        lightShow.enabled = true;
        lightHidden.enabled = false;
    }

    void Update()
    {

        


        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        bool isMovingInput = move.magnitude > 0.1f;


        if (isMovingInput)
        {
            lightShow.enabled = false;
            lightHidden.enabled = true;
            // Déplacement
            transform.Translate(move.normalized * speed * Time.deltaTime, Space.World);

            float currentSpeed = move.magnitude;
            float normalizedSpeed = currentSpeed / speed;
            animator.SetFloat("Speed", normalizedSpeed);


            // Rotation vers la direction du mouvement
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Animation
            animator.SetBool("isMoving", true);

            // Reset du timer
            idleTimer = 0f;
        }
        else
        {
            // On compte le temps d'immobilité
            idleTimer += Time.deltaTime;

            if (idleTimer >= 0.01f)
            {
                animator.SetBool("isMoving", false);

            }
            if (idleTimer >= 0.3f)
            {
                lightShow.enabled = true;
                lightHidden.enabled = false;
            }
        }
    }
}
