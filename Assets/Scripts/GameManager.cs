using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private bool isPaused = false;
    [SerializeField] public Canvas pause;
    [SerializeField] public Canvas End;
    [SerializeField] public GameObject objects;
    [SerializeField] public Canvas Score;
    [SerializeField] public Canvas Locked;

    public Transform player;
    public GameObject enemyPrefab;

    public float minRadius = 4f;
    public float maxRadius = 12f;
    bool isAlive = true;
    float score = 0;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI FinalScore;
    [SerializeField] float lockedFlashSeconds = 1.0f;


    public void SpawnEnemyOutsideRadiusInWorld()
    {
        float r = Random.Range(minRadius, maxRadius);
        float angle = Random.value * Mathf.PI * 2f;

        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
        Vector3 pos = (Vector2)player.position + offset;
        pos.z = 0f;

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }

    void Start()
    {
        objects.gameObject.SetActive(true);
        End.gameObject.SetActive(false);
        pause.gameObject.SetActive(false);
        Score.gameObject.SetActive(true);
        Locked.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        InvokeRepeating("SpawnEnemyOutsideRadiusInWorld", 0f, 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }

        if (isAlive)
        {
            score += Time.deltaTime * 4;
            ScoreText.text = "Score : " + score.ToString("F");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool deleted = TryDeleteEnemyUnderCursor();
            if (!deleted && Locked)
            {
                Locked.gameObject.SetActive(true);
                CancelInvoke(nameof(HideLocked));
                Invoke(nameof(HideLocked), lockedFlashSeconds);
            }
        }
    }

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
        FinalScore.text = "Final Score: " + score.ToString("F");
    }

    public void endGame()
    {
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

        Destroy(enemy.gameObject);
        return true;
    }

    void HideLocked()
    {
        if (Locked) Locked.gameObject.SetActive(false);
    }

}
