using AudioServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AudioServices
{
    public class Sound : MonoBehaviour, ISound
    {
        private readonly Queue<ISoundTween> _tweenQueue = new Queue<ISoundTween>();
        private AudioSource _audioSource;
        private ISoundTween _currentTween;
        private bool _isPlaying;
        private float _playTime;
        private List<TaskCompletionSource<bool>> _onCompleteTaskCompletions = new List<TaskCompletionSource<bool>>();

        public event EventHandler Completed;
        public event EventHandler Unloaded;

        public string Id { get; set; }
        public string Category { get; set; }
        public SoundBuilder SoundBuilder { get; set; }

        public AudioSource GetAudioSource()
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            return _audioSource;
        }

        public void QueueTween(ISoundTween soundTween)
        {
            _tweenQueue.Enqueue(soundTween);
        }

        private void Update()
        {
            if (_isPlaying)
            {
                OnCheckPlayCompleted();
                OnExecuteTweenUpdate();
            }
        }

        private void OnExecuteTweenUpdate()
        {
            if (_currentTween == null
                && _tweenQueue.Count > 0)
            {
                _currentTween = _tweenQueue.Dequeue();
                _currentTween.OnStart(this);
                _currentTween.Completed += OnTweenComplete;
            }

            _currentTween?.OnUpdate(this);
        }

        private void OnCheckPlayCompleted()
        {
            var playDelta = Time.realtimeSinceStartup - _playTime;

            if (playDelta >= _audioSource.clip.length && !_audioSource.loop)
            {
                _isPlaying = false;
                Completed?.Invoke(this, EventArgs.Empty);
                InvokeCompletedTaskCompletions();
            }
        }

        private void InvokeCompletedTaskCompletions()
        {
            for(var i = _onCompleteTaskCompletions.Count - 1; i >= 0; i--)
            {
                _onCompleteTaskCompletions[i].SetResult(true);
                _onCompleteTaskCompletions.RemoveAt(i);
            }

            _onCompleteTaskCompletions.Clear();
        }

        private void OnTweenComplete(object sender, System.EventArgs e)
        {
            _currentTween.Completed -= OnTweenComplete;
            _currentTween = null;
        }

        public void Play()
        {
            _playTime = Time.realtimeSinceStartup;
            _isPlaying = true;
            _audioSource.Play();
        }

        public void PlayOneShot(float volumeScale = 1f)
        {
            _audioSource.PlayOneShot(_audioSource.clip, volumeScale);
        }

        public Task PlayAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _onCompleteTaskCompletions.Add(tcs);

            Play();

            return tcs.Task;
        }

        public void Stop()
        {
            _isPlaying = false;
            _audioSource.Stop();
        }

        public void Unload()
        {
            SoundBuilder.Unload();
            SoundBuilder = null;

            Unloaded?.Invoke(this, EventArgs.Empty);

            GameObject.Destroy(gameObject);
        }
    }
}
