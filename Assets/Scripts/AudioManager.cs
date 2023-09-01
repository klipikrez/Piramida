using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioAudi
    {
        public AudioSource source;
        public GameObject obj;
        public Coroutine coroutine;
        public AudioAudi(AudioSource source, GameObject obj, Coroutine coroutine)
        {
            this.source = source;
            this.obj = obj;
            this.coroutine = coroutine;
        }
        public AudioAudi()
        {

        }
    }

    [System.NonSerialized]
    public Dictionary<string, AudioClip> audioDictionary = new Dictionary<string, AudioClip>();
    public GameObject DDDSoundPrefab;
    public AudioMixerGroup DD;
    Coroutine voiceCorutine;
    AudioSource VoiceLineSource;
    //public Dictionary<string, AudioAudi> PlayingAudio = new Dictionary<string, AudioAudi>();

    [UDictionary.Split(50, 50)]
    public UDictionary2 PlayingAudio;
    [System.Serializable]
    public class UDictionary2 : UDictionary<string, AudioAudi> { }

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
        transform.position = Vector3.zero;

    }
    public void StartCorutineBre()
    {
        StartCoroutine("delay");
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(2f);
        PlayAudioClipLooping("AshAndBone", 0.25f);
    }
    public void PlayAudioClip(string audioClipName, float volume = 1, int priority = 128)
    {
        PlayingAudio.Add(audioClipName, new AudioAudi());
        PlayingAudio[audioClipName].coroutine = StartCoroutine(Play(audioDictionary[audioClipName], volume, priority));
    }

    public void StopAudio(string name)
    {
        foreach (KeyValuePair<string, AudioAudi> emmiter in PlayingAudio)
        {
            if (name == emmiter.Key)
            {
                if (emmiter.Value.coroutine != null)
                {
                    StopCoroutine(emmiter.Value.coroutine);
                }
                if (emmiter.Value.source != null)
                {
                    Destroy(emmiter.Value.source);
                }
                if (emmiter.Value.obj != null)
                {
                    Destroy(emmiter.Value.obj);
                }
                PlayingAudio.Remove(emmiter.Key);
                break;
            }
        }
    }
    public void PlayVoiceLine(string audioClipName, float volume = 1, int priority = 128)
    {
        if (voiceCorutine != null)
        {
            StopCoroutine(voiceCorutine);
            if (VoiceLineSource != null)
            {
                VoiceLineSource.Stop();
                Destroy(VoiceLineSource);
            }
        }
        VoiceLineSource = gameObject.AddComponent<AudioSource>();
        PlayingAudio.Add(audioClipName, new AudioAudi());
        voiceCorutine = StartCoroutine(Play(audioDictionary[audioClipName], volume, priority, VoiceLineSource));
        PlayingAudio[audioClipName].coroutine = voiceCorutine;

    }

    public void PlayAudioClipLooping(string audioClipName, float volume = 1)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audioDictionary[audioClipName];
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        PlayingAudio.Add(audioClipName, new AudioAudi(audioSource, null, null));

    }

    public void PlayAudioDDDClipDynamic(string audioClipName, Transform follow, float dddPercent = 1, float volume = 1, int priority = 128)
    {

        PlayingAudio.Add(audioClipName, new AudioAudi());
        PlayingAudio[audioClipName].coroutine = StartCoroutine(PlayDDDSynamic(audioDictionary[audioClipName], follow, dddPercent, volume, priority));
    }

    public void PlayAudioDDDClipStatic(string audioClipName, Vector3 position, float dddPercent = 1, float volume = 1, int priority = 128)
    {
        PlayingAudio.Add(audioClipName, new AudioAudi());
        PlayingAudio[audioClipName].coroutine = StartCoroutine(PlayDDDStatic(audioDictionary[audioClipName], position, dddPercent, volume, priority));
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
        PlayingAudio[audio.name].obj = gameobj;
        PlayingAudio[audio.name].source = audioSource;
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(gameobj);
        PlayingAudio.Remove(audio.name);
    }

    public void PlayAudioDDDClipStaticLooping(string audioClipName, Vector3 position, float dddPercent = 1, float volume = 1, int priority = 128)
    {

        //StartCoroutine(PlayDDDStaticLooping(audioDictionary[audioClipName], position, dddPercent, volume, priority));

        GameObject gameobj = Instantiate(DDDSoundPrefab, position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audioDictionary[audioClipName];
        audioSource.volume = volume;
        audioSource.spatialBlend = dddPercent;
        audioSource.priority = priority;
        audioSource.loop = true;
        PlayingAudio.Add(audioClipName, new AudioAudi(audioSource, gameobj, null));
        audioSource.Play();

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
        PlayingAudio[audio.name].obj = gameobj;
        PlayingAudio[audio.name].source = audioSource;
        audioSource.Play();
        while (follow != null && follow.gameObject.activeSelf)
        {
            gameobj.transform.position = follow.position;
            yield return new WaitForEndOfFrame();
        }
        audioSource.Stop();
        Destroy(gameobj);
        PlayingAudio.Remove(audio.name);
    }

    IEnumerator Play(AudioClip audio, float volume, int priority)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.priority = priority;
        PlayingAudio[audio.name].source = audioSource;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(audioSource);
        PlayingAudio.Remove(audio.name);
    }

    IEnumerator Play(AudioClip audio, float volume, int priority, AudioSource audioSource)
    {

        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.priority = priority;
        PlayingAudio[audio.name].source = audioSource;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(audioSource);
        PlayingAudio.Remove(audio.name);
    }
}
