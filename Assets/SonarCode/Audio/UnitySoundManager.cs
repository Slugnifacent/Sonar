using UnityEngine;
using System.Collections;
using Sonar;

public class UnitySoundManager : MonoBehaviour {
    SoundManager soundManager;

    // Use this for initialization
    void Awake()
    {
        soundManager = new SoundManager(this.gameObject);
    }

    void Start () {
        PlaySound();
    }

    public void PlaySound()
    {
        SoundManager.createSound(this.gameObject,SoundType.AMBIENT.AMBIANT_SPECTRE_INVESTIGATE.ToString());
    }

	// Update is called once per frame
	void Update () {
        soundManager.Update();

    }
}
