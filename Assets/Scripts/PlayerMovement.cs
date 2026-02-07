using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 3f;
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h,0, v);
        bool isMoving = move.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            transform.Translate(move.normalized * speed * Time.deltaTime);
        }
    }
}
