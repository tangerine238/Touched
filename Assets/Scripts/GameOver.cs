using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;



    private void Awake()
    {
        restartButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.Game);
        });
        homeButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.HomeScreen);
        });

        Time.timeScale = 1f;
    }
}
