using UnityEngine;
using System.Collections;

namespace GLIB.Audio {

    [RequireComponent(typeof(AudioSource))]
    public class SoundChannel : MonoBehaviour
    {

        public enum PlayPriority : int
        {
            NORMAL = 0,
            HIGH = 1
        }

        PlayPriority _playPriority = PlayPriority.NORMAL;
        /// <summary>
        /// Sets the priority flag of the SoundChannel. See the SoundModule.PlaySound Method for more Information.
        /// </summary>
        public PlayPriority playPriority
        {
            get
            {
                return _playPriority;
            }
            set
            {
                _playPriority = value;
            }
        }

        AudioSource AudioComponent
        {
            get { return this.GetComponent<AudioSource>(); }
        }

        public float Pitch {
            get {
                AudioSource audioSrc = AudioComponent;
                if (audioSrc != null)
                    return audioSrc.pitch;
                else
                    return -1;
            }
            set {
                AudioSource audioSrc = AudioComponent;
                if (audioSrc != null)
                    audioSrc.pitch = value;
            }
        }

        public float Volume {
            get
            {
                AudioSource audioSrc = AudioComponent;
                if (audioSrc != null)
                    return audioSrc.volume;
                else
                    return -1;
            }
            set
            {
                AudioSource audioSrc = AudioComponent;
                if (audioSrc != null)
                    audioSrc.volume = value;
            }
        }

        public bool isPlaying
        {
            get
            {

                AudioSource audioSrc = AudioComponent;

                if (!audioSrc)
                    return false;

                if (audioSrc.isPlaying)
                    return true;
                else
                    return false;

            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlaySound(AudioClip clip, PlayPriority priority = PlayPriority.NORMAL, float volume = 1.0f, float pitch = 1.0f,  bool loop = false)
        {

            AudioSource audioComp = AudioComponent;

            if (audioComp == null || clip == null)
            {
                Debug.LogError("Audio component is not ready, or the clip provided is null");
                return;
            }

            audioComp.clip = clip;
            audioComp.pitch = pitch;
            audioComp.volume = volume;
            audioComp.loop = loop;
            _playPriority = priority;

            audioComp.Play();

        }

        public void StopSound()
        {

            AudioSource audioComp = AudioComponent;

            if (audioComp == null)
                return;

            audioComp.Stop();

        }

        public void PauseSound() {

            AudioSource audioComp = AudioComponent;

            if (audioComp == null)
                return;

            audioComp.Pause();

        }

        public void UnPauseSound() {

            AudioSource audioComp = AudioComponent;

            if (audioComp == null)
                return;

            audioComp.UnPause();

        }

    }

}
