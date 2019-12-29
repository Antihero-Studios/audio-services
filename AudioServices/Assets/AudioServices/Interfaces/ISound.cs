using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AudioServices.Interfaces
{
    public interface ISound
    {
        string Id { get; }
        string Category { get; }

        event EventHandler Completed;
        event EventHandler Unloaded;

        AudioSource GetAudioSource();

        void QueueTween(ISoundTween soundTween);

        void Play();
        Task PlayAsync();
        void PlayOneShot(float volumeScale = 1f);

        void Unload();
    }
}
