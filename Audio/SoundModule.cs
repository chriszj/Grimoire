using UnityEngine;
using System.Collections;

using GLIB.Core;

using System;
using System.Collections.Generic;

namespace GLIB.Audio {

    public class SoundModule : BackModule<SoundModule> {

        int _maxSFXChannels = 6;
        /// <summary>
        /// Defines the maximum Sound Effects channels to use. You must restart the module if you change this value while running.
        /// </summary>
        public int MaxSFXChannels {
            get {
                return _maxSFXChannels;
            }
            set {

                _maxSFXChannels = value;

            }
        }

        int _maxBGMChannels = 2;
        /// <summary>
        /// Retrieves the maximum number of Background Music Channels which as of this version only two are supported.
        /// </summary>
        public int MaxBGMChannels {
            get {
                return _maxBGMChannels;
            }
        }

        List<SoundChannel> _sfxChannels;
        List<SoundChannel> _bgmChannels;

        List<SoundChannel> _currentlyPlayingSFXChannels;
        List<SoundChannel> _currentlyPlayingBGMChannels;

        Dictionary<SoundChannel, float> _sfxChannelsOriginalVolumes;
        Dictionary<SoundChannel, float> _bgmChannelsOriginalVolumes;

        protected override void ProcessInitialization()
        {

            _sfxChannels = new List<SoundChannel>();
            _bgmChannels = new List<SoundChannel>();

            for (int sc = 0; sc < _maxSFXChannels; sc++) {

                GameObject sfxObj = new GameObject("sfx_" + sc + 1);
                sfxObj.transform.SetParent(this.gameObject.transform);

                SoundChannel sfxCh = sfxObj.AddComponent<SoundChannel>();

                _sfxChannels.Add(sfxCh);

            }

            for (int bc = 0; bc < _maxBGMChannels; bc++)
            {

                GameObject bgmObj = new GameObject("bgm_" + bc + 1);
                bgmObj.transform.SetParent(this.gameObject.transform);

                SoundChannel bgmCh = bgmObj.AddComponent<SoundChannel>();

                _bgmChannels.Add(bgmCh);

            }

        }

        protected override void ProcessUpdate()
        {

        }

        protected override void ProcessTermination()
        {

            StopAllSounds();

            _sfxChannels.Clear();

            _bgmChannels.Clear();

            _sfxChannels = null;
            _bgmChannels = null;

        }

        /// <summary>
        /// Play a sound using one of the available SFX SoundChannel components. If there is not any SoundChannel ready
        /// it will look for any of the Normal Play Priority playing SoundChannel to make it play a new sound requested by stoping the previous one (if playing).
        /// If all SoundChannels are of high priority then the sound to be played wont be processed at all.
        /// </summary>
        /// <param name="clip">An audioclip to be played.</param>
        /// <param name="channelIndex">The index of the channel to use, default value is automatic using priority flags.</param>
        /// <param name="priority">Sets the priority of this proccess.</param>
        /// <param name="volume">The value for the volume.</param>
        /// <param name="pitch">The value for the pitch.</param>
        /// <param name="loop">Set the audioclip to loop or not.</param>
        public void PlaySFX(AudioClip clip, int channelIndex = -1, SoundChannel.PlayPriority priority = SoundChannel.PlayPriority.NORMAL, float volume = 1f, float pitch = 1f, bool loop = false) {

            try
            {

                if (!isRunning)
                    throw new Exception("Error: Sound Module has not been initialized.");

                if (clip == null)
                    throw new Exception("Error: Clip argument is null");

                int chIndex = channelIndex;

                // Automatic channel index selection
                if (chIndex < 0)
                {
                    // Search for a ready channel
                    for (int fchk = 0; fchk < _sfxChannels.Count; fchk++) {

                        if (!_sfxChannels[fchk].isPlaying) {
                            chIndex = fchk;
                            break;
                        }

                    }

                    // Failed fist attempt? Look by priority process
                    if (chIndex < 0)
                    {
                        for (int schk = 0; schk < _sfxChannels.Count; schk++) {

                            if (_sfxChannels[schk].playPriority == SoundChannel.PlayPriority.NORMAL) {
                                chIndex = schk;
                                break;
                            }

                        }

                    }

                }

                if (chIndex >= 0 && chIndex < _sfxChannels.Count)
                    _sfxChannels[chIndex].PlaySound(clip, priority, volume, pitch, loop);
                else
                    Debug.LogWarning("No available SFX channel found. Either are busy or all have a High play priority process");

            }
            catch (Exception e) {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }


        }

        /// <summary>
        /// Play a clip using one of the available BGM SoundChannel components. If there is not any SoundChannel ready
        /// it will look for any of the Normal Play Priority playing SoundChannel to make it play a new sound requested by stoping the previous one (if playing).
        /// If all SoundChannels are of high priority then the sound to be played wont be processed at all.
        /// </summary>
        /// <param name="clip">An audioclip to be played.</param>
        /// <param name="channelIndex">The index of the channel to use, default value is automatic using priority flags.</param>
        /// <param name="priority">Sets the priority of this proccess.</param>
        /// <param name="volume">The value for the volume.</param>
        /// <param name="pitch">The value for the pitch.</param>
        /// <param name="loop">Set the audioclip to loop or not.</param>
        public void PlayBGM(AudioClip clip, int channelIndex = -1, SoundChannel.PlayPriority priority = SoundChannel.PlayPriority.NORMAL, float volume = 1, float pitch = 1, bool loop = false) {

            try {

                if (!isRunning)
                    throw new Exception("Error: Sound Module has not been initialized.");

                if (clip == null)
                    throw new Exception("Error: clip argument is null");

                int chIndex = channelIndex;

                // Automatic channel index selection
                if (chIndex < 0)
                {
                    // Search for a ready channel
                    for (int fchk = 0; fchk < _bgmChannels.Count; fchk++)
                    {

                        if (!_bgmChannels[fchk].isPlaying)
                        {
                            chIndex = fchk;
                            break;
                        }

                    }

                    // Failed fist attempt? Look by priority process
                    if (chIndex < 0)
                    {
                        for (int schk = 0; schk < _bgmChannels.Count; schk++)
                        {

                            if (_bgmChannels[schk].playPriority == SoundChannel.PlayPriority.NORMAL)
                            {
                                chIndex = schk;
                                break;
                            }

                        }

                    }

                }

                if (chIndex >= 0 && chIndex < _bgmChannels.Count)
                    _bgmChannels[chIndex].PlaySound(clip, priority, volume, pitch, loop);
                else
                    Debug.LogWarning("No available BGM channel found. Either are busy or all have a High play priority process");

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }

        }

        /// <summary>
        /// Stops all sounds comming from both SFXChannels and BGMChannels.
        /// </summary>
        public void StopAllSounds() {

            foreach (SoundChannel sfxChannel in _sfxChannels)
                sfxChannel.StopSound();

            foreach (SoundChannel bgmChannel in _bgmChannels)
                bgmChannel.StopSound();

        }

        // Pause a SFX Channel given its index. If a sound is paused it will be treated as is not playing, so be sure to set the appropiate priority flag.
        public void PauseSFXChannel(int index) {

            if (index < _sfxChannels.Count && index >= 0)
                _sfxChannels[index].PauseSound();

        }

        // UnPause a SFX Channel given its index.
        public void UnPauseSFXChannel(int index) {

            if (index < _sfxChannels.Count && index >= 0)
                _sfxChannels[index].UnPauseSound();

        }

        // Pause a BGM Channel given its index. If a sound is paused it will be treated as is not playing, so be sure to set the appropiate priority flag.
        public void PauseBGMChannel(int index) {

            if (index < _bgmChannels.Count && index >= 0)
                _bgmChannels[index].PauseSound();

        }

        // UnPause a BGM Channel given its index.
        public void UnPauseBGMChannel(int index) {

            if (index < _bgmChannels.Count && index >= 0)
                _bgmChannels[index].UnPauseSound();

        }

        /// <summary>
        /// Retrieves a SFX channel by its id.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SoundChannel GetSFXChannel(int index) {

            SoundChannel sfxChannel = null;

            if (index < _sfxChannels.Count && index >= 0)
                sfxChannel = _sfxChannels[index];

            return sfxChannel;
        }

        /// <summary>
        /// Retrieves a BGM Channel by its Id.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SoundChannel GetBGMChannel(int index) {

            SoundChannel bgmChannel = null;

            if (index < _bgmChannels.Count && index >= 0)
                bgmChannel = _bgmChannels[index];

            return bgmChannel;

        }

        /// <summary>
        /// Pauses all currently playing SFX channels;
        /// </summary>
        public void PauseAllPlayingSFXChannels() {

            _currentlyPlayingSFXChannels = new List<SoundChannel>();

            foreach (SoundChannel sound in _sfxChannels) {

                if (sound.isPlaying)
                {
                    _currentlyPlayingSFXChannels.Add(sound);
                    sound.PauseSound();
                }
            }

        }

        /// <summary>
        /// Resume all previously paused SFX channels
        /// </summary>
        public void UnPauseAllPreviouslyPlayingSFXChannels() {

            if (_currentlyPlayingSFXChannels != null)
            {
                foreach (SoundChannel sound in _currentlyPlayingSFXChannels)
                    sound.UnPauseSound();

                _currentlyPlayingSFXChannels.Clear();
            }
                       
        }

        /// <summary>
        /// Pauses all currently playing BGM channels;
        /// </summary>
        public void PauseAllPlayingBGMChannels()
        {

            _currentlyPlayingBGMChannels = new List<SoundChannel>();

            foreach (SoundChannel sound in _bgmChannels)
            {

                if (sound.isPlaying)
                {
                    _currentlyPlayingBGMChannels.Add(sound);
                    sound.PauseSound();
                }
            }

        }

        /// <summary>
        /// Resume all previously paused BGM channels
        /// </summary>
        public void UnPauseAllPreviouslyPlayingBGMChannels()
        {

            if (_currentlyPlayingBGMChannels != null)
            {
                foreach (SoundChannel sound in _currentlyPlayingBGMChannels)
                    sound.UnPauseSound();

                _currentlyPlayingBGMChannels.Clear();
            }

        }

        /// <summary>
        /// Mute all current sfx channels
        /// </summary>
        public void MuteSFXChannels() {

            _sfxChannelsOriginalVolumes = new Dictionary<SoundChannel, float>();

            foreach (SoundChannel sound in _sfxChannels)
            {
                _sfxChannelsOriginalVolumes.Add(sound, sound.Volume);
                sound.Volume = 0;
            }

        }

        /// <summary>
        /// Restores all previous sfx channels volumes.
        /// </summary>
        public void UnMuteSFXChannels() {

            if (_sfxChannelsOriginalVolumes != null)
            {
                foreach (SoundChannel sound in _sfxChannels)
                {
                    if (_sfxChannelsOriginalVolumes.ContainsKey(sound))
                    {
                        sound.Volume = _sfxChannelsOriginalVolumes[sound];
                    }
                   
                }
                _sfxChannelsOriginalVolumes.Clear();
            }
        }

        /// <summary>
        /// Mutes all bgm channels.
        /// </summary>
        public void MuteBGMChannels()
        {

            _bgmChannelsOriginalVolumes = new Dictionary<SoundChannel, float>();

            foreach (SoundChannel sound in _bgmChannels)
            {
                _bgmChannelsOriginalVolumes.Add(sound, sound.Volume);
                sound.Volume = 0;
            }

        }

        /// <summary>
        /// Restores all bgm channels volumes.
        /// </summary>
        public void UnMuteBGMChannels()
        {

            if (_bgmChannelsOriginalVolumes != null)
            {
                foreach (SoundChannel sound in _bgmChannels)
                {
                    if (_bgmChannelsOriginalVolumes.ContainsKey(sound))
                    {
                        sound.Volume = _bgmChannelsOriginalVolumes[sound];
                    }

                }
                _bgmChannelsOriginalVolumes.Clear();
            }
        }

    }

}
