using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    public bool CanMove { get; private set; } = true;

    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(CanMove){
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            if (movement.x > 0)
                sr.flipX = false;
            else if (movement.x < 0)
                sr.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * speed;
    }
    public void EnableMovement(){
        CanMove = true;
    }
    public void DisableMovement(){
        CanMove = false;
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }
}