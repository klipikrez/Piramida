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
    public void PlayAudioClip(string audioClipName, float volume = 1)
    {

        StartCoroutine(Play(audioDictionary[audioClipName], volume));

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

    public void PlayAudioDDDClipDynamic(string audioClipName, Transform follow, float volume = 1)
    {

        StartCoroutine(PlayDDDSynamic(audioDictionary[audioClipName], follow, volume));

    }

    public void PlayAudioDDDClipStatic(string audioClipName, Vector3 position, float volume = 1)
    {

        StartCoroutine(PlayDDDStatic(audioDictionary[audioClipName], position, volume));

    }

    IEnumerator PlayDDDStatic(AudioClip audio, Vector3 position, float volume)
    {
        GameObject gameobj = Instantiate(DDDSoundPrefab, position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(gameobj);
    }

    IEnumerator PlayDDDSynamic(AudioClip audio, Transform follow, float volume)
    {
        GameObject gameobj = Instantiate(DDDSoundPrefab, follow.position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
        while (follow.gameObject != null && follow.gameObject.activeSelf)
        {
            yield return new WaitForEndOfFrame();
            gameobj.transform.position = follow.position;
        }
        audioSource.Stop();
        Destroy(gameobj);
    }

    IEnumerator Play(AudioClip audio, float volume)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(audioSource);
    }
}
