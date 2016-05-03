using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundChannel : MonoBehaviour {

    public enum PlayPriority : int {
        NORMAL = 0,
        HIGH = 1
    }

    PlayPriority _playPriority = PlayPriority.NORMAL;
    public PlayPriority playPriority {
        get {
            return _playPriority;
        }
        set {
            _playPriority = value;
        }
    }

    AudioSource AudioComponent {
        get { return this.GetComponent<AudioSource>(); }
    }
    
    public bool isReady {
        get {

            AudioSource audioSrc = AudioComponent;

            if (!audioSrc)
                return false;

            if (_playPriority == PlayPriority.NORMAL || !audioSrc.isPlaying)
                return true;
            else
                return false;

        }
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void PlaySound(AudioClip clip, PlayPriority priority = PlayPriority.NORMAL, float pitch = 1.0f, float volume = 1.0f) {

        AudioSource audioComp = AudioComponent;

        if (audioComp == null)
            return;

    }

}
