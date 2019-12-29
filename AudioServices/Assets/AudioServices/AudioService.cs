using AudioServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace AudioServices
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        private List<ISound> _registeredSounds = new List<ISound>();
        private AssetReference _audioMixerAssetRef;
        private string _audioMixerAddress;

        [SerializeField]
        private AudioMixer _audioMixer; 

        public void SetAudioMixer(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
        }

        public void SetAudioMixer(AssetReference assetReference)
        {
            _audioMixerAssetRef = assetReference;
        }

        public void SetAudioMixer(string audioMixerAddress)
        {
            _audioMixerAddress = audioMixerAddress;
        }

        public async Task<AudioMixer> GetAudioMixerAsync()
        {
            if (_audioMixer != null)
                return _audioMixer;

            if (!string.IsNullOrEmpty(_audioMixerAddress))
                _audioMixer = await Addressables.LoadAssetAsync<AudioMixer>(_audioMixerAddress).Task;

            if(_audioMixerAddress != null)
                _audioMixer = await _audioMixerAssetRef?.LoadAssetAsync<AudioMixer>().Task;

            return _audioMixer;
        }
        
        public async Task<AudioMixerGroup[]> FindMatchingGroupsAsync(string subPath)
        {
            var mixer = await GetAudioMixerAsync();

            return mixer?.FindMatchingGroups(subPath);
        }

        public virtual ISoundBuilder CreateSoundBuilder(string id)
        {
            return new SoundBuilder(this, id);
        }

        public ISound GetFirstOrDefault(Func<ISound, bool> predicate)
        {
            return _registeredSounds.Where(predicate).FirstOrDefault();
        }

        public ISound GetFirstOrDefaultById(string id)
        {
            return GetFirstOrDefault((sound) => sound.Id == id);
        }

        public IEnumerable<ISound> FindSounds(Func<ISound, bool> predicate)
        {
            return _registeredSounds.Where(predicate);
        }

        public void Register(ISound sound)
        {
            if (sound != null && !_registeredSounds.Contains(sound))
            {
                _registeredSounds.Add(sound);
                sound.Unloaded += OnSoundUnloaded;
            }
        }

        private void OnSoundUnloaded(object sender, EventArgs e)
        {
            Unregister(sender as ISound);
        }

        public void Unregister(ISound sound)
        {
            _registeredSounds.Remove(sound);
        }
    }
}