using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isDragging;
    private Vector3 offset;
    private Camera cam;
    private Collider2D col;
    private Vector3 extents;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.15f;
    public int health = 100;

    public int getHealth()
    {
        return health;
    }

    public void setHealth(int k)
    {
        health = k;
    }

    void Awake()
    {
        cam = Camera.main;
        col = GetComponent<Collider2D>();
        if (col == null) Debug.LogError("[SpriteDrag] Add a Collider2D to this sprite.");
        var sr = GetComponent<SpriteRenderer>();
        extents = sr ? sr.bounds.extents : new Vector3(0.5f, 0.5f, 0f);
    }

    void Start()
    {
        Vector3 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        center.z = 0;
        transform.position = center;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;

            if (Physics2D.OverlapPoint(mouse) == col)
            {
                isDragging = true;
                offset = transform.position - mouse;
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;
            Vector3 target = ClampToScreen(mouse + offset);

            // SmoothDamp instead of snapping
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private Vector3 ClampToScreen(Vector3 p)
    {
        // must be orthographic for this math
        if (!cam.orthographic) return p;

        // camera half-height/width in world units
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        // camera center (works even if camera moves)
        Vector3 c = cam.transform.position;

        // sprite half-size in world units (accounts for scale)
        var sr = GetComponent<SpriteRenderer>();
        Vector2 e = sr ? (Vector2)sr.bounds.extents : new Vector2(0.5f, 0.5f);

        float minX = c.x - halfW + e.x;
        float maxX = c.x + halfW - e.x;
        float minY = c.y - halfH + e.y;
        float maxY = c.y + halfH - e.y;

        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        p.z = 0f;
        return p;
    }
}
