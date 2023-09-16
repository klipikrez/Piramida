using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject[] GameObjectUIPanels;

    private void Awake()
    {


    }
    private void Start()
    {


        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SetObjectsActive(0);
        }

    }
    public void MainManu()
    {
        SetObjectsActive(0);
    }
    public void Play()
    {
        SetObjectsActive(1);
    }
    public void Options()
    {
        SetObjectsActive(2);
    }

    public void SetObjectsActive(int i)
    {
        foreach (GameObject obj in GameObjectUIPanels)
        {
            if (obj != null)
                obj.SetActive(false);
        }
        GameObjectUIPanels[i].SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
