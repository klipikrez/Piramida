using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMusicSet", menuName = "Audio/MusicSet")]
public class MusicSet : ScriptableObject
{
    int lastSong = 0;
    public AudioClip[] songs;
    public AudioClip PickRandomSong()
    {
        int index = Random.Range(0, songs.Length);
        lastSong = index;
        Debug.Log(index);
        return songs[index];
    }

    public AudioClip PickRandomSongExclude()
    {
        int index = Random.Range(0, songs.Length - 1);
        int[] indexArrayExcluded = ExcludePreviousSong();
        lastSong = indexArrayExcluded[index];
        Debug.Log(index);

        return songs[indexArrayExcluded[index]];
    }

    int[] ExcludePreviousSong()
    {
        List<int> tmp = new List<int>();
        for (int i = 0; i < songs.Length; i++)
        {
            if (i != lastSong)
            {
                tmp.Add(i);
            }
        }
        return tmp.ToArray();
    }
}
