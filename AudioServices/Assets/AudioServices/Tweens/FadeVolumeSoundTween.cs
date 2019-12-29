using AudioServices.Interfaces;
using System;
using UnityEngine;

namespace AudioServices.Tweens
{
    public class FadeVolumeSoundTween : ISoundTween
    {
        private readonly float _durationSeconds;
        private readonly float _startVolume;
        private readonly float _endVolume;

        private float _time;

        public FadeVolumeSoundTween(float durationSeconds, float startVolume = 0f, float endVolume = 1f)
        {
            _durationSeconds = durationSeconds;
            _startVolume = startVolume;
            _endVolume = endVolume;
        }

        public event EventHandler Completed;

        public void OnStart(ISound sound)
        {
            sound.GetAudioSource().volume = _startVolume;
        }

        public void OnUpdate(ISound sound)
        {
            _time += Time.deltaTime;
            var delta = _time / _durationSeconds;
            sound.GetAudioSource().volume = Mathf.Lerp(_startVolume, _endVolume, delta);

            if(delta >= 1f)
            {
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
