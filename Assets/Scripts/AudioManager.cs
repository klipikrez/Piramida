using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [System.NonSerialized]
    public Dictionary<string, AudioClip> audioDictionary = new Dictionary<string, AudioClip>();
    public GameObject DDDSoundPrefab;
    public AudioMixerGroup DD;
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Object[] allAudios = Resources.LoadAll("Audio", typeof(AudioClip));

        foreach (AudioClip clip in allAudios)
        {
            audioDictionary.Add(clip.name, clip);
        }
    }
    /********************************************************/
    /*mozes da dodas da mozes da mu prosleds vise stringova,*/
    /*i on da odaberte jedan random string iz datih ponuda  */
    /********************************************************/
    private void Start()
    {
        StartCoroutine("delay");
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(2f);
        PlayAudioClipLooping("AshAndBone", 0.3f);
    }
    public void PlayAudioClip(string audioClipName, float volume = 1, int priority = 128)
    {

        StartCoroutine(Play(audioDictionary[audioClipName], volume, priority));

    }

    public void PlayAudioClipLooping(string audioClipName, float volume = 1)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audioDictionary[audioClipName];
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();

    }

    public void PlayAudioDDDClipDynamic(string audioClipName, Transform follow, float dddPercent = 1, float volume = 1, int priority = 128)
    {

        StartCoroutine(PlayDDDSynamic(audioDictionary[audioClipName], follow, dddPercent, volume, priority));

    }

    public void PlayAudioDDDClipStatic(string audioClipName, Vector3 position, float dddPercent = 1, float volume = 1, int priority = 128)
    {

        StartCoroutine(PlayDDDStatic(audioDictionary[audioClipName], position, dddPercent, volume, priority));

    }

    IEnumerator PlayDDDStatic(AudioClip audio, Vector3 position, float dddPercent, float volume, int priority)
    {
        GameObject gameobj = Instantiate(DDDSoundPrefab, position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.spatialBlend = dddPercent;
        audioSource.priority = priority;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(gameobj);
    }

    IEnumerator PlayDDDSynamic(AudioClip audio, Transform follow, float dddPercent, float volume, int priority)
    {
        GameObject gameobj = Instantiate(DDDSoundPrefab, follow.position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.spatialBlend = dddPercent;
        audioSource.priority = priority;
        audioSource.Play();
        while (follow != null && follow.gameObject.activeSelf)
        {
            gameobj.transform.position = follow.position;
            yield return new WaitForEndOfFrame();
        }
        audioSource.Stop();
        Destroy(gameobj);
    }

    IEnumerator Play(AudioClip audio, float volume, int priority)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.priority = priority;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(audioSource);
    }
}
