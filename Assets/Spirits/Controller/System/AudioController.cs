using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    public AudioSource MianAudioSource;
    public AudioSource SubAudioSource;
    public List<AudioClip> audioList = new List<AudioClip>();

    // Start is called before the first frame update
    void Awake()
    {
        MianAudioSource = GetComponent<AudioSource>();
        SubAudioSource = transform.Find("SubAudioSource").GetComponent<AudioSource>();
        AddAudio();
        MianAudioSource.loop = true;
        SubAudioSource.loop = false;
        SubAudioSource.volume = 0.4f;
    }


    public void ChangceAudio(int i,int clipid)
    {
        switch (i)
        {
            case 0:
                MianAudioSource.clip = audioList[clipid];
                MianAudioSource.Play();
                break;
            case 1:
                SubAudioSource.clip = audioList[clipid];
                SubAudioSource.Play(); break;
           
        }
    }

    void AddAudio()
    {
        audioList.Add(Resources.Load<AudioClip>("Audio/Day"));
        audioList.Add(Resources.Load<AudioClip>("Audio/Night"));
        audioList.Add(Resources.Load<AudioClip>("Audio/Painting"));
        audioList.Add(Resources.Load<AudioClip>("Audio/Pay"));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
