using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public sound[] sounds;

    public static AudioManager instance;

    // Use this for initialization
    void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        foreach(sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start(){
        Play("combatTheme1");
    }

    public void Play (string name){
        sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null){
            
            Debug.LogWarning("Sound: " + name + "not found!");
            return;
        }   

        s.source.Play();
    }
}
