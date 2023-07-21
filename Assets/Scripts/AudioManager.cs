using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // public AudioClip[] audioClips;
    [System.NonSerialized]
    public Dictionary<string, AudioClip> audioDictionary = new Dictionary<string, AudioClip>();
    public GameObject DDDSoundPrefab;
    public AudioMixerGroup DD;
    // Start is called before the first frame update
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
    /* private void Start()
     {
         if (SceneManager.GetActiveScene().buildIndex == 0)
         {
             PlayAudioClip(6);
         }
     }*/
    public void PlayAudioClip(string audioClipName, float volume)
    {

        StartCoroutine(Play(audioDictionary[audioClipName], volume));

    }

    public void PlayAudioDDDClipDynamic(string audioClipName, Transform follow, float volume)
    {

        StartCoroutine(PlayDDDSynamic(audioDictionary[audioClipName], follow, volume));

    }
    public void PlayAudioDDDClipStatic(string audioClipName, Vector3 position, float volume)
    {

        StartCoroutine(PlayDDDStatic(audioDictionary[audioClipName], position, volume));

    }
    /*   public void PlaySendSound()
       {
           StartCoroutine(Play(send[Random.Range(0, send.Length - 1)]));
       }*/
    /*  public void PlayBattleSound(Vector3 position)
      {
          vfxManager.Instance.Play(position, 0);
          StartCoroutine(PlayDDD(battle[Random.Range(0, battle.Length - 1)], position));

      }
  */
    /* public void PlayTowerSound(Vector3 position)
     {
         StartCoroutine(PlayDDD(tower[Random.Range(0, tower.Length - 1)], position));
     }*/

    IEnumerator PlayDDDStatic(AudioClip audio, Vector3 position, float volume)
    {
        GameObject gameobj = Instantiate(DDDSoundPrefab, position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length); //cekaj
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
            yield return new WaitForEndOfFrame(); //cekaj
                                                  //            Debug.Log(audio.name);
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
        yield return new WaitForSeconds(audio.length); //cekaj
        audioSource.Stop();
        Destroy(audioSource);
    }
}
