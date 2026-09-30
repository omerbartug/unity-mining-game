using UnityEngine;

// Oyuncunun 2D fizik tabanlı hareketini ve sprite yönünü yönetir.
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

    // Girdi yöneticisinden hareket vektörünü alır ve sprite yönünü günceller.
    private void Update()
    {
        if (CanMove)
        {
            movement = PlayerInputManager.Instance != null ? PlayerInputManager.Instance.MovementInput : Vector2.zero;
            if (movement.x > 0)
                sr.flipX = false;
            else if (movement.x < 0)
                sr.flipX = true;
        }
    }

    // Fizik hızını uygular.
    private void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * speed;
    }

    // Oyuncu hareketini serbest bırakır.
    public void EnableMovement()
    {
        CanMove = true;
    }

    // Oyuncu hareketini kilitler ve anlık hızı sıfırlar.
    public void DisableMovement()
    {
        CanMove = false;
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }
}