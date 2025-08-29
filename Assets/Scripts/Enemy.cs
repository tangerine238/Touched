using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] private GameManager gameManager;
    public float speed = 10f;
    public bool canBeClicked = true;

    public float stopDistance = 0.1f;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isHovered = false;
    public int health = 1;

    public int getHealth()
    {
        return health;
    }

    public void setHealth(int x)
    {
        health = x;
    }

    public void Init(Transform target, GameManager gm, float? speedOverride = null)
    {
        this.target = target;
        this.gameManager = gm;
        if (speedOverride.HasValue) speed = speedOverride.Value;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;

    }


    void FixedUpdate()
    {
        if (!target) return;

        Vector2 toTarget = (Vector2)target.position - rb.position;
        float dist = toTarget.magnitude;
        if (dist <= Mathf.Epsilon) return;

        if (dist > stopDistance)
        {
            Vector2 dir = toTarget / Mathf.Max(dist, 1e-6f);
            float step = speed * Time.fixedDeltaTime;

            float moveLen = Mathf.Min(step, Mathf.Max(0f, dist - stopDistance));
            rb.MovePosition(rb.position + dir * moveLen);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<Player>() != null)
        {
            if (gameManager)
            {
                gameManager.endGame();
            }
        }
    }

}
