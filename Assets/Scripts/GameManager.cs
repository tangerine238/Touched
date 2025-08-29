using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private bool isPaused = false;

    [SerializeField] public Canvas pause;
    [SerializeField] public Canvas End;
    [SerializeField] public GameObject objects;
    [SerializeField] public Canvas Score;
    [SerializeField] public Canvas LockScreen;

    public Transform player;
    public GameObject enemyPrefab;
    public GameObject fastPrefab;
    public GameObject slowPrefab;

    public float minRadius = 5f;
    public float maxRadius = 5f;

    bool isAlive = true;
    float score = 0f;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI FinalScore;
    private float wait;

    [Header("Lock Settings")]
    [SerializeField] float lockedFlashSeconds = 3.0f;

    bool locked = false;
    Coroutine lockRoutine;

    public void SpawnEnemy()
    {
        float r = Random.Range(minRadius, maxRadius);
        float angle = Random.value * Mathf.PI * 2f;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
        Vector3 pos = (Vector2)player.position + offset;
        pos.z = 0f;
        var go = Instantiate(enemyPrefab, pos, Quaternion.identity);

        var e = go.GetComponent<Enemy>();
        if (e != null)
        {
            e.Init(player, this);
        }

    }

    public void SpawnFast()
    {
        float r = Random.Range(minRadius, maxRadius);
        float angle = Random.value * Mathf.PI * 2f;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
        Vector3 pos = (Vector2)player.position + offset;
        pos.z = 0f;
        var go = Instantiate(fastPrefab, pos, Quaternion.identity);

        var e = go.GetComponent<Enemy>();
        if (e != null)
        {
            e.Init(player, this);
        }
    }

    public void SpawnSlow()
    {
        float r = Random.Range(minRadius, maxRadius);
        float angle = Random.value * Mathf.PI * 2f;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
        Vector3 pos = (Vector2)player.position + offset;
        pos.z = 0f;
        var go = Instantiate(slowPrefab, pos, Quaternion.identity);

        var e = go.GetComponent<Enemy>();
        if (e != null)
        {
            e.Init(player, this);
        }
    }



    void Start()
    {
        objects.gameObject.SetActive(true);
        End.gameObject.SetActive(false);
        pause.gameObject.SetActive(false);
        Score.gameObject.SetActive(true);
        if (LockScreen) LockScreen.gameObject.SetActive(false);
        wait = 0;


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        InvokeRepeating("SpawnEnemy", 0f, 15f);
        InvokeRepeating("SpawnFast", 3f, 15f);
        InvokeRepeating("SpawnSlow", 6f, 15f);
        InvokeRepeating("SpawnEnemy", 41f, 10f);
        InvokeRepeating("SpawnFast", 42f, 10f);
        InvokeRepeating("SpawnSlow", 43f, 10f);
        InvokeRepeating("SpawnEnemy", 94f, 5f);
        InvokeRepeating("SpawnFast", 95f, 5f);
        InvokeRepeating("SpawnSlow", 96f, 5f);

    }

    string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        int h = total / 3600;
        int m = (total % 3600) / 60;
        int s = total % 60;
        return h > 0 ? $"{h}:{m:00}:{s:00}" : $"{m:00}:{s:00}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume(); else Pause();
        }

        if (isAlive)
        {
            score += Time.deltaTime;
            if (ScoreText) ScoreText.text = $"Time Alive : {FormatTime(score)}";
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!locked)
            {
                bool deleted = TryDeleteEnemyUnderCursor();
                if (!deleted) BeginLock(lockedFlashSeconds);
            }
        }
    }

    void BeginLock(float seconds)
    {
        if (lockRoutine != null) StopCoroutine(lockRoutine);
        lockRoutine = StartCoroutine(LockRoutine(seconds));
    }

    IEnumerator LockRoutine(float seconds)
    {
        locked = true;
        if (LockScreen) LockScreen.gameObject.SetActive(true);

        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        locked = false;
        if (LockScreen) LockScreen.gameObject.SetActive(false);
        lockRoutine = null;
    }

    public bool LockedState() => locked;

    void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pause.gameObject.SetActive(true);
    }

    void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pause.gameObject.SetActive(false);
    }

    private void ShowGameOver()
    {
        if (FinalScore) FinalScore.text = $"Time Alive : {FormatTime(score)}";
    }

    public void endGame()
    {
        // cancel lock if active
        if (lockRoutine != null) { StopCoroutine(lockRoutine); lockRoutine = null; }
        locked = false;
        if (LockScreen) LockScreen.gameObject.SetActive(false);

        ShowGameOver();
        Score.gameObject.SetActive(false);
        End.gameObject.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        objects.gameObject.SetActive(false);
    }

    bool TryDeleteEnemyUnderCursor()
    {
        var cam = Camera.main;
        if (!cam) return false;

        Vector3 m = cam.ScreenToWorldPoint(Input.mousePosition);
        m.z = 0f;

        var hit = Physics2D.OverlapPoint((Vector2)m);
        if (!hit) return false;

        var enemy = hit.GetComponent<Enemy>() ?? hit.GetComponentInParent<Enemy>();
        if (!enemy) return false;

        if (enemy.getHealth() > 1)
        {
            enemy.setHealth(1);
        }
        else
        {
            Destroy(enemy.gameObject);
        }
        return true;
    }
}
