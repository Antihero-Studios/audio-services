using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace AudioServices.Interfaces
{
    public interface IAudioService
    {
        ISoundBuilder CreateSoundBuilder(string id);
        Task<AudioMixerGroup[]> FindMatchingGroupsAsync(string subPath);

        void SetAudioMixer(string audioMixerAddress);
        void SetAudioMixer(AssetReference assetReference);
        void SetAudioMixer(AudioMixer audioMixer);

        Task<AudioMixer> GetAudioMixerAsync();

        IEnumerable<ISound> FindSounds(Func<ISound, bool> predicate);
        ISound GetFirstOrDefault(Func<ISound, bool> predicate);
        ISound GetFirstOrDefaultById(string id);

        void Register(ISound sound);
        void Unregister(ISound sound);
    }
}