using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tymski;
using UnityEngine.SceneManagement;
using static Functions;
public class RuntimeSceneManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject gozmo;
    public GameObject gizmoCamera;
    public static RuntimeSceneManager Instance { get; private set; }
    public SceneReference mainMenu;
    public GameObject gameMenu;

    private void Awake()
    {
        SetLoadingGizmos(false);

        Instance = this;

        DontDestroyOnLoad(gameObject);
        if (SceneManager.GetActiveScene().buildIndex != 0 && GameObject.FindGameObjectWithTag("GameMenu") == null)
        {
            Instantiate(gameMenu);
        }

    }
    private void Start()
    {

        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        SetLoadingGizmos(false);

        if (next.buildIndex != 0 && GameObject.FindGameObjectWithTag("GameMenu") == null)
        {
            Instantiate(gameMenu);
        }
    }

    public void ReturnToMainMenu()
    {

        Load(mainMenu);
    }

    public void Load(SceneReference sceneRef)
    {
        SetLoadingGizmos(true);
        GameObject[] movements = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject movement in movements)
        {
            movement.GetComponent<PlayerMovement>().UnsubscribeButtonPressFunctions();
            movement.GetComponent<PlayerArms>().UnsubscribeButtonPressFunctions();
        }

        StartCoroutine(LoadAsyncScene(sceneRef));

    }

    public void Load(int cseneIndex)
    {
        SetLoadingGizmos(true);
        GameObject[] movements = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject movement in movements)
        {
            movement.GetComponent<PlayerMovement>().UnsubscribeButtonPressFunctions();
            movement.GetComponent<PlayerArms>().UnsubscribeButtonPressFunctions();
        }

        StartCoroutine(LoadAsyncScene(cseneIndex));

    }

    IEnumerator LoadAsyncScene(SceneReference sceneRef)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneRef);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

            yield return null;
        }
    }

    IEnumerator LoadAsyncScene(int cseneIndex)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(cseneIndex);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

            yield return null;
        }
    }

    void SetLoadingGizmos(bool val)
    {
        loadingScreen.SetActive(val);
        gizmoCamera.SetActive(val);
        if (val)
        {
            StartInvokeRepeating();
        }
        else
        {
            CancleInvokeRepeating();
        }
    }

    void Gizmo()
    {
        gozmo.transform.Rotate(UniformNoise(52, 30, 1, Time.time) + Vector3.one * 0.3f);
    }

    public void StartInvokeRepeating()
    {
        InvokeRepeating("Gizmo", 0f, 1f / 60f);
    }
    public void CancleInvokeRepeating()
    {
        CancelInvoke();
    }
    public static Vector3 UniformNoise(float seed, float strenth, float speed, float time)
    {
        return new Vector3(
            (0.4665f - Mathf.PerlinNoise(seed, time * speed)) * strenth,
            (0.4665f - Mathf.PerlinNoise(seed + 52, time * speed)) * strenth,
            (0.4665f - Mathf.PerlinNoise(seed + 152, time * speed)) * strenth);

    }

}
