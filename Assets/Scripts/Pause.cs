using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private Button homeButton;



    private void Awake()
    {
        homeButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.HomeScreen);
        });

        Time.timeScale = 1f;
    }
}
