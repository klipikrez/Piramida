using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using static Functions;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {

        settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.dataPath + "/StreamingAssets/klipik.rez"));
    }
    private void Start()
    {

        options.UpdateSettingsValues(settings);
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SetObjectsActive(0);
        }

    }

    #region menu
    public GameObject[] GameObjectUIPanels;


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
        if (i != -1)
        {
            GameObjectUIPanels[i].SetActive(true);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
    #endregion
    #region Oprions

    /**podsetnik*/
    /*
    {
            string json = JsonUtility.ToJson(settings);// citaj klasu kao json string

            settings = JsonUtility.FromJson<Settings>(json);// pisi klasu od json stringa

            File.ReadAllText(Application.dataPath + "/Wision5252/klipik.rez");// citaj json kao string

            File.WriteAllText(Application.dataPath + "/Wision5252/klipik.rez", json);//pisi u json kao json string

            JsonUtility.FromJsonOverwrite(json, settings);//pisi u klasu kao json string
    }*/

    Settings settings;
    public AudioMixer audioMixer;
    public Options options;
    bool changingLanguage = false;

    public void FullScreenValue(bool value)
    {
        if (settings != null)
        {
            settings.fullScreen = value;
            UpdateSettings(value ? 1 : 0, settings.fps);
        }

    }

    public void FpsValue(float value)
    {
        if (settings != null)
        {
            settings.fps = (int)value;
            VsyncValue(value <= 0.1f ? true : false);
            UpdateSettings(settings.fullScreen ? 1 : 0, (int)value);
        }
    }

    public void VsyncValue(bool value)
    {
        if (settings != null)
        {
            settings.vsync = value;
            QualitySettings.vSyncCount = value ? 1 : 0;
            UpdateSettings();
        }
    }

    public void VolumeValue(float value, int index)
    {
        if (settings != null)
        {
            value /= 100;
            settings.volumes[index] = value;
            string name = "";
            switch (index)
            {
                case 0:
                    {
                        name = "master";
                        audioMixer.SetFloat(name, (Mathf.Log10(value) * 20) != float.NegativeInfinity ? Mathf.Log10(value) * 20 : -52);
                        break;
                    }
                case 1:
                    {
                        name = "music";
                        audioMixer.SetFloat(name, (Mathf.Log10(value) * 20) != float.NegativeInfinity ? Mathf.Log10(value) * 20 : -52);
                        break;
                    }
                case 2:
                    {
                        name = "ddd";
                        audioMixer.SetFloat("ddd", (Mathf.Log10(value) * 20) != float.NegativeInfinity ? Mathf.Log10(value) * 20 : -52);
                        audioMixer.SetFloat("dd", (Mathf.Log10(value) * 20) != float.NegativeInfinity ? Mathf.Log10(value) * 20 : -52);
                        break;
                    }
                case 3:
                    {
                        name = "dd";
                        break;
                    }
            }

            UpdateSettings();
        }
    }

    public void SetLanguage(string index)
    {
        if (changingLanguage)
        {
            return;
        }
        StartCoroutine(c_SetLanguage(index));
    }

    IEnumerator c_SetLanguage(string index)
    {
        changingLanguage = true;
        yield return LocalizationSettings.InitializationOperation;
        UnityEngine.Localization.Locale[] locales = LocalizationSettings.AvailableLocales.Locales.ToArray();
        bool selectedSomething = false;
        foreach (UnityEngine.Localization.Locale locale in locales)
        {

            if (locale.Identifier.Code == index)
            {
                selectedSomething = true;
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log(LocalizationSettings.SelectedLocale.Identifier.Code);
                settings.language = LocalizationSettings.SelectedLocale.Identifier.Code;

            }
        }
        if (!selectedSomething)
        {
            Debug.LogError("Nije dobar jezik");
        }
        else
        {
            UpdateSettings();
        }

        changingLanguage = false;
    }

    public void SetShadows(int value)
    {
        if (settings != null)
        {
            // Locate the current URP Asset
            UniversalRenderPipelineAsset data = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            // Do nothing if Unity can't locate the URP Asset
            if (!data) return;
            settings.shadowDistance = value;
            data.shadowDistance = value;
            UpdateSettings();
        }
    }





    public void UpdateSettings(int fullScreen = -1, int hz = -1)
    {
        Application.targetFrameRate = hz != -1 ? (hz) : Application.targetFrameRate;

        Screen.SetResolution(
            Screen.width,
             Screen.height,
              fullScreen != -1 ? (fullScreen == 1 ? true : false) : Screen.fullScreen,
               hz != -1 ? (hz) : Application.targetFrameRate);

        File.WriteAllText(Application.dataPath + "/StreamingAssets/klipik.rez", JsonUtility.ToJson(settings));//update setings json
    }


    #endregion
}
