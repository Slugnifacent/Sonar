using UnityEngine;
using System.Collections;

public partial class Audio {
    public static object CreateAudio(object GameObj, string Name)
    {
        return new Audio((GameObject)GameObj, Name);
    }   

    public static void Play(object audio)
    {
        Audio sound = (Audio)audio;
        sound.PLAY();
    }

    public static void Stop(object audio)
    {
        Audio sound = (Audio)audio;
        sound.STOP();
    }

    public static void Pause(object audio)
    {
        Audio sound = (Audio)audio;
        sound.PUASE();
    }

    public static void LoadAudio()
    {
        Audio.LoadSoundBank("Ambient");
    }

    public static void UnloadAudio()
    {
        Audio.UnloadSoundBank("");
    }
}
