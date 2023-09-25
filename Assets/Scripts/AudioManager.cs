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
        public string name;
        public AudioSource source;
        public GameObject obj;
        public Coroutine coroutine;
        public AudioAudi(AudioSource source, GameObject obj, Coroutine coroutine, string name)
        {
            this.name = name;
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
    [System.NonSerialized]
    public AudioSource musicSource;
    public AudioMixerGroup musicGroup;
    Coroutine switchMusicCorutine;
    //public Dictionary<string, AudioAudi> PlayingAudio = new Dictionary<string, AudioAudi>();

    /*[UDictionary.Split(50, 50)]
    public UDictionary2 PlayingAudio;
    [System.Serializable]
    public class UDictionary2 : UDictionary<System.Guid, AudioAudi> { }*/
    public Dictionary<System.Guid, AudioAudi> PlayingAudio = new Dictionary<System.Guid, AudioAudi>();
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
        SetMainMusic("AshAndBone");
    }

    public void SetMainMusic(string audioClipName)
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.clip = audioDictionary[audioClipName];
            musicSource.volume = 1;
            musicSource.priority = 200;
            musicSource.outputAudioMixerGroup = musicGroup;
            musicSource.Play();
        }
        else
        {
            if (musicSource.clip.name != audioClipName)
            {
                if (switchMusicCorutine != null)
                {
                    StopCoroutine(switchMusicCorutine);
                }
                switchMusicCorutine = StartCoroutine(c_SwitchMusic(audioDictionary[audioClipName], 1));
            }
        }
    }

    public void StopMusic()
    {
        Destroy(musicSource);
    }

    public System.Guid PlayAudioClip(string audioClipName, float volume = 1, int priority = 128)
    {
        System.Guid id = System.Guid.NewGuid();
        PlayingAudio.Add(id, new AudioAudi());
        PlayingAudio[id].name = audioClipName;
        PlayingAudio[id].coroutine = StartCoroutine(Play(audioDictionary[audioClipName], volume, priority, id));
        return id;
    }

    public void StopAudio(System.Guid name)
    {
        foreach (KeyValuePair<System.Guid, AudioAudi> emmiter in PlayingAudio)
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
    public System.Guid PlayVoiceLine(string audioClipName, float volume = 1, int priority = 128)
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
        System.Guid id = System.Guid.NewGuid();
        PlayingAudio.Add(id, new AudioAudi());
        voiceCorutine = StartCoroutine(Play(audioDictionary[audioClipName], volume, priority, VoiceLineSource, id));
        PlayingAudio[id].coroutine = voiceCorutine;
        PlayingAudio[id].name = audioClipName;
        return id;

    }

    public System.Guid PlayAudioClipLooping(string audioClipName, float volume = 1)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audioDictionary[audioClipName];
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        System.Guid id = System.Guid.NewGuid();
        PlayingAudio.Add(id, new AudioAudi(audioSource, null, null, audioClipName));
        return id;
    }

    public System.Guid PlayAudioDDDClipDynamic(string audioClipName, Transform follow, float dddPercent = 1, float volume = 1, int priority = 128)
    {
        System.Guid id = System.Guid.NewGuid();
        PlayingAudio.Add(id, new AudioAudi());
        PlayingAudio[id].name = audioClipName;
        PlayingAudio[id].coroutine = StartCoroutine(PlayDDDSynamic(audioDictionary[audioClipName], follow, dddPercent, volume, priority, id));

        return id;
    }

    public System.Guid PlayAudioDDDClipStatic(string audioClipName, Vector3 position, float dddPercent = 1, float volume = 1, int priority = 128)
    {
        System.Guid id = System.Guid.NewGuid();
        PlayingAudio.Add(id, new AudioAudi());
        PlayingAudio[id].name = audioClipName;
        PlayingAudio[id].coroutine = StartCoroutine(PlayDDDStatic(audioDictionary[audioClipName], position, dddPercent, volume, priority, id));
        return id;
    }

    IEnumerator PlayDDDStatic(AudioClip audio, Vector3 position, float dddPercent, float volume, int priority, System.Guid id)
    {
        GameObject gameobj = Instantiate(DDDSoundPrefab, position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.spatialBlend = dddPercent;
        audioSource.priority = priority;
        audioSource.Play();
        PlayingAudio[id].obj = gameobj;
        PlayingAudio[id].source = audioSource;
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(gameobj);
        PlayingAudio.Remove(id);
    }

    public System.Guid PlayAudioDDDClipStaticLooping(string audioClipName, Vector3 position, float dddPercent = 1, float volume = 1, int priority = 128)
    {

        //StartCoroutine(PlayDDDStaticLooping(audioDictionary[audioClipName], position, dddPercent, volume, priority));

        GameObject gameobj = Instantiate(DDDSoundPrefab, position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audioDictionary[audioClipName];
        audioSource.volume = volume;
        audioSource.spatialBlend = dddPercent;
        audioSource.priority = priority;
        audioSource.loop = true;
        System.Guid id = System.Guid.NewGuid();
        PlayingAudio.Add(id, new AudioAudi(audioSource, gameobj, null, audioClipName));
        audioSource.Play();
        return id;
    }



    IEnumerator PlayDDDSynamic(AudioClip audio, Transform follow, float dddPercent, float volume, int priority, System.Guid id)
    {
        GameObject gameobj = Instantiate(DDDSoundPrefab, follow.position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.spatialBlend = dddPercent;
        audioSource.priority = priority;
        PlayingAudio[id].obj = gameobj;
        PlayingAudio[id].source = audioSource;
        audioSource.Play();
        while (follow != null && follow.gameObject.activeSelf)
        {
            gameobj.transform.position = follow.position;
            yield return new WaitForEndOfFrame();
        }
        audioSource.Stop();
        Destroy(gameobj);
        PlayingAudio.Remove(id);
    }

    IEnumerator Play(AudioClip audio, float volume, int priority, System.Guid id)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.priority = priority;
        PlayingAudio[id].source = audioSource;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(audioSource);
        PlayingAudio.Remove(id);
    }

    IEnumerator Play(AudioClip audio, float volume, int priority, AudioSource audioSource, System.Guid id)
    {

        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.priority = priority;
        PlayingAudio[id].source = audioSource;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);
        audioSource.Stop();
        Destroy(audioSource);
        PlayingAudio.Remove(id);
    }

    IEnumerator c_SwitchMusic(AudioClip switchTo, float time = 0.5f)
    {
        float timer = 0;
        float currentVolume = musicSource.volume;
        bool switched = false;
        while (timer < time)
        {
            float amount = timer / time;
            if (amount < 0.5f)
            {
                musicSource.volume = currentVolume - currentVolume * amount * 2;
            }
            else
            {
                if (!switched)
                {
                    switched = true;
                    musicSource.clip = switchTo;
                    musicSource.Play();
                }
                musicSource.volume = (amount - 0.5f) * 2;
            }
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }
}
