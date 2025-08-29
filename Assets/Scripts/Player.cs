using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isDragging;
    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.15f;

    private Camera cam;
    private Collider2D col;
    [SerializeField] private GameManager manager;
    private Vector2 halfSize = new Vector2(0.5f, 0.5f);

    void Awake()
    {
        cam = Camera.main;
        col = GetComponent<Collider2D>();
        var sr = GetComponent<SpriteRenderer>();
        if (sr) halfSize = (Vector2)sr.bounds.extents;

        Vector3 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        center.z = 0f;
        transform.position = center;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



    void Update()
    {
        if (Time.timeScale == 0f || manager.LockedState())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0f;

            if (col.OverlapPoint(mouse))
            {
                isDragging = true;
                offset = transform.position - mouse;
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0f;
            Vector3 target = ClampToScreen(mouse + offset);

            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            velocity = Vector3.zero;
        }
    }

    private Vector3 ClampToScreen(Vector3 p)
    {
        if (!cam.orthographic) return p;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 c = cam.transform.position;

        float minX = c.x - halfW + halfSize.x;
        float maxX = c.x + halfW - halfSize.x;
        float minY = c.y - halfH + halfSize.y;
        float maxY = c.y + halfH - halfSize.y;

        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        p.z = 0f;
        return p;
    }
}
