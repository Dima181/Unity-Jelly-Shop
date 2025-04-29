using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Mine.CodeBase.Framework.Manager.Sound
{
    [Serializable]
    public class SoundClip
    {
        #region Properties
        
        public AudioClip AudioClip => _audioClip;
        public float DefaultVolume => _defaultVolume;

        #endregion


        #region Fields
        
        [SerializeField] private AudioClip _audioClip;
        [SerializeField, Range(0, 1)] private float _defaultVolume = 0.5f;

        #endregion
    }

    public class SoundManager : IStartable
    {
        #region Properties

        public bool IsMuteBGM
        {
            get => _isMuteBgm;

            set
            {
                _isMuteBgm = value;

                _audioSourceForBgm.mute = _isMuteBgm;
            }
        }

        public bool IsMuteEffect
        {
            get => _isMuteEffect;

            set
            {
                _isMuteEffect = value;

                foreach (var audioSourceForEffect in _audioSourcesForSfx)
                {
                    audioSourceForEffect.mute = _isMuteEffect;
                }
            }
        }

        public AudioClip CurrentBGM => _audioSourceForBgm.clip;

        #endregion


        #region Fields

        private AudioSource _audioSourceForBgm;
        private List<AudioSource> _audioSourcesForSfx = new();
        private bool _isMuteBgm;
        private bool _isMuteEffect;
        private GameObject _audioPlayer;

        #endregion


        #region Entry Point

        void IStartable.Start()
        {
            _audioPlayer = new GameObject("AudioPlayer");
            AddAudioSourceForBgm();
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Play BGM.
        /// </summary>
        /// <param name="sound">Information about the sound to be played</param>
        /// <param name="volume">Volume (default value when 0)</param>
        public void PlayBgm(SoundClip sound, float volume = 0f)
        {
            if (CurrentBGM == sound.AudioClip)
                return;

            _audioSourceForBgm.mute = _isMuteBgm;
            PlaySound(_audioSourceForBgm, sound, volume);
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="sound">Information about the sound to be played</param>
        /// <param name="volume">Volume (default value when 0)</param>
        public void PlaySfx(SoundClip sound, float volume = 0f)
        {
            if (IsMuteEffect)
                return;

            for (int i = 0; i < _audioSourcesForSfx.Count; i++)
            {
                if (!_audioSourcesForSfx[i].isPlaying)
                {
                    PlaySound(_audioSourcesForSfx[i], sound, volume);
                    return;
                }

            }
            
            var newAudioSource = AddAudioSourceForSfx();
            PlaySound(newAudioSource, sound, volume);
        }


        #endregion


        #region Private Methods

        private void AddAudioSourceForBgm()
        {
            _audioSourceForBgm = _audioPlayer.AddComponent<AudioSource>();
            _audioSourceForBgm.loop = true;
            _audioSourceForBgm.playOnAwake = false;
        }

        private AudioSource AddAudioSourceForSfx()
        {
            var audioSource = _audioPlayer.GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            _audioSourcesForSfx.Add(audioSource);
            return audioSource;
        }

        private void PlaySound(AudioSource audioSource, SoundClip sound, float volume)
        {
            audioSource.clip = sound.AudioClip;
            audioSource.volume = volume == 0 ? sound.DefaultVolume : volume;
            audioSource.Play();
        }

        #endregion
    }
}
