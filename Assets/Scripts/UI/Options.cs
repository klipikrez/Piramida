using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using static Functions;
using System;

public class Options : MonoBehaviour
{
    /**podsetnik*/
    /*
    {
            string json = JsonUtility.ToJson(settings);// citaj klasu kao json string

            settings = JsonUtility.FromJson<Settings>(json);// pisi klasu od json stringa

            File.ReadAllText(Application.dataPath + "/Wision5252/klipik.rez");// citaj json kao string

            File.WriteAllText(Application.dataPath + "/Wision5252/klipik.rez", json);//pisi u json kao json string

            JsonUtility.FromJsonOverwrite(json, settings);//pisi u klasu kao json string
    }*/
    public MainMenu menu;
    public Toggle fullScreenToggle;
    public Slider Fps;
    public Slider[] VolumeSliders = new Slider[4];
    public TMP_Dropdown lanhuage;




    void Start()
    {
        if (menu == null)
        {
            menu = transform.parent.parent.GetComponent<MainMenu>();
        }
        /*
        fullScreenToggle.isOn = settings.fullScreen;
        Fps.value = settings.fps;
        UpdateSettings(settings.fullScreen ? 1 : 0, settings.fps);

        for (int i = 0; i < VolumeSliders.Length; i++)
        {
            VolumeSliders[i].value = settings.volumes[i] * 100;
        }
        VsyncValue(settings.vsync);//ovo nije potrebno
*/
    }

    public void UpdateSettingsValues(Settings settings)
    {
        fullScreenToggle.isOn = settings.fullScreen;
        Fps.value = settings.fps;
        menu.FpsValue(settings.fps);
        int languageIndex = 0;
        for (int i = 0; i < lanhuage.options.Count; i++)
        {
            Debug.Log(lanhuage.options[i].image.name);
            if (settings.language == lanhuage.options[i].image.name)
            {
                languageIndex = i;
            }
        }
        lanhuage.value = languageIndex;
        SetLanguage(languageIndex);

        for (int i = 0; i < settings.volumes.Length; i++)
        {
            VolumeSliders[i].value = settings.volumes[i] * 100;
        }

    }

    public void FullScreenValue(bool value)
    {
        menu.FullScreenValue(value);

    }

    public void FpsValue(float value)
    {
        menu.FpsValue(value);
    }

    public void VsyncValue(bool value)
    {
        menu.VsyncValue(value);
    }

    public void Volume0Value(float value)
    {
        VolumeValue(value, 0);
    }
    public void Volume1Value(float value)
    {
        VolumeValue(value, 1);
    }
    public void Volume2Value(float value)
    {
        VolumeValue(value, 2);
    }
    public void Volume3Value(float value)
    {
        VolumeValue(value, 3);
    }
    void VolumeValue(float value, int index)
    {
        menu.VolumeValue(value, index);
    }
    public void SetLanguage(int value)
    {

        menu.SetLanguage(lanhuage.options[value].image.name);
    }


    /*
        public void FullScreen(bool value)
        {
            OptionsManager.Instance.SetFullScreen(value);
            //UpdateUi();
        }

        public void SetVolume1(float value)
        {
            OptionsManager.Instance.SetVolume(value / 100, 0);
            //UpdateVolumeUi(value, 0); nediraj pokvareno
        }
        public void SetVolume2(float value)
        {
            OptionsManager.Instance.SetVolume(value / 100, 1);
            //UpdateVolumeUi(value, 1);
        }
        public void SetVolume3(float value)
        {
            OptionsManager.Instance.SetVolume(value / 100, 2);
            //UpdateVolumeUi(value, 2);
        }
        public void SetVolume4(float value)
        {
            OptionsManager.Instance.SetVolume(value / 100, 3);
            //UpdateVolumeUi(value, 3);
        }
        public void SetVolume(float value, int index)
        {
            OptionsManager.Instance.SetVolume(value / 100, index);
            //UpdateVolumeUi(value, index);
        }


        public void SetFramerate(float value)
        {
            OptionsManager.Instance.SetFramerate(value);
            //UpdateUi();
        }

        public void SetVSync(bool value)
        {

            OptionsManager.Instance.SetVSync(value);
            //UpdateUi();
        }*/
    /*
        public void UpdateUi()
        {
            Fps.value = PlayerPrefs.GetInt("fps");
            fullScreenToggle.isOn = PlayerPrefs.GetInt("fullScreen") == 1 ? true : false;
            for (int i = 0; i <= 3; i++)
            {
                if (PlayerPrefs.HasKey("Volume" + i))
                {
                    //SetVolume(PlayerPrefs.GetFloat("Volume" + i), i);
                    VolumeSliders[i].value = (PlayerPrefs.GetFloat("Volume" + i) * 100);
                }
            }
        }*/

}
