using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject UI;
    [System.NonSerialized]
    public bool paused = false;
    public static float timeSinceStart = 0f;
    [System.NonSerialized]
    public static GameMenu Instance;
    public MainMenu mainMenu;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Resume();

    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (UI.activeSelf)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UI.SetActive(true);
        Time.timeScale = 0.0f;
        mainMenu.SetObjectsActive(0);
    }
    public void Resume()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UI.SetActive(false);
        mainMenu.SetObjectsActive(0);
        Time.timeScale = 1.0f;
    }

    public void ReturnToMainMenu()
    {
        paused = false;
        UI.SetActive(false);
        Time.timeScale = 1.0f;
        RuntimeSceneManager.Instance.ReturnToMainMenu();
    }

    public void Retry()
    {
        paused = false;
        mainMenu.SetObjectsActive(-1);
        RuntimeSceneManager.Instance.Load(SceneManager.GetActiveScene().buildIndex);
    }

    public void Lost()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UI.SetActive(true);
        Time.timeScale = 0.0f;
        mainMenu.SetObjectsActive(2);
    }

    public void Win()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UI.SetActive(true);
        Time.timeScale = 0.0f;
        mainMenu.SetObjectsActive(3);
    }

}
