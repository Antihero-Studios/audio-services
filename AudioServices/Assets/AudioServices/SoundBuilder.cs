using AudioServices.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AudioServices
{
    public class SoundBuilder : ISoundBuilder
    {
        private readonly AudioService _audioService;
        private readonly string _id;
        private bool _loops;
        private Vector3 _position;
        private float _spatialBlend;
        private float _dopplerLevel;
        private AudioRolloffMode _audioRolloffMode = AudioRolloffMode.Logarithmic;
        private float _minDistance = 1f;
        private float _maxDistance = 500f;
        private float _volume = 1f;
        private float _pitch = 1f;
        private string _audioMixerGroup;
        private List<ISoundTween> _soundTweens = new List<ISoundTween>();
        private string _category;

        private AudioClip _audioClip;
        private AssetReference _audioClipReference;
        private string _audioClipAddress;

        private AsyncOperationHandle<AudioClip> _asyncOperationHandle;

        public SoundBuilder(AudioService audioService, string id)
        {
            _audioService = audioService;
            _id = id;
        }

        public ISoundBuilder WithAssetReference(AssetReference assetReference)
        {
            _audioClipReference = assetReference;

            return this;
        }

        public ISoundBuilder WithAssetAddress(string address)
        {
            _audioClipAddress = address;

            return this;
        }

        public ISoundBuilder WithAudioClip(AudioClip audioClip)
        {
            _audioClip = audioClip;

            return this;
        }

        public ISoundBuilder With3DPosition(Vector3 position)
        {
            _spatialBlend = 1f;
            _position = position;

            return this;
        }

        public ISoundBuilder WithLooping(bool doesLoop = true)
        {
            _loops = doesLoop;

            return this;
        }

        public ISoundBuilder WithDoppler(float dopplerLevel)
        {
            _dopplerLevel = dopplerLevel;

            return this;
        }

        public ISoundBuilder WithSpatialBlend(float spatialBlend)
        {
            _spatialBlend = spatialBlend;

            return this;
        }

        public ISoundBuilder WithAudioRolloffMode(AudioRolloffMode audioRolloffMode)
        {
            _audioRolloffMode = audioRolloffMode;

            return this;
        }

        public ISoundBuilder WithMinDistance(float minDistance)
        {
            _minDistance = minDistance;

            return this;
        }

        public ISoundBuilder WithMaxDistance(float maxDistance)
        {
            _maxDistance = maxDistance;

            return this;
        }

        public ISoundBuilder WithVolume(float volume)
        {
            _volume = volume;

            return this;
        }

        public ISoundBuilder WithAudioMixerGroup(string audioMixerGroup)
        {
            _audioMixerGroup = audioMixerGroup;

            return this;
        }

        public ISoundBuilder WithQueuedSoundTween(ISoundTween soundTween)
        {
            _soundTweens.Add(soundTween);

            return this;
        }

        public ISoundBuilder WithPitch(float pitch)
        {
            _pitch = pitch;

            return this;
        }

        public ISoundBuilder WithCategory(string category)
        {
            _category = category;

            return this;
        }

        public async Task<ISound> Build()
        {
            // Create...
            var instance = new GameObject($"Sound[{_id}]", new System.Type[] { typeof(AudioSource), typeof(Sound) });

            // Configure Sound...
            var sound = instance.GetComponent<Sound>();
            sound.Id = _id;
            sound.SoundBuilder = this;
            sound.Category = _category;

            foreach(var tween in _soundTweens)
            {
                sound.QueueTween(tween);
            }

            // Register...
            _audioService.Register(sound);

            // Configure AudioSource...
            var source = sound.GetAudioSource();
            var clip = await GetAudioClipAsync();
            source.outputAudioMixerGroup = await GetAudioMixerGroupAsync();
            source.clip = clip ?? throw new System.Exception($"SoundBuilder: Could not find AudioClip with specified settings (address = {_audioClipAddress}, reference = {_audioClipReference})");
            source.dopplerLevel = _dopplerLevel;
            source.minDistance = _minDistance;
            source.maxDistance = _maxDistance;
            source.spatialBlend = _spatialBlend;
            source.rolloffMode = _audioRolloffMode; 
            source.loop = _loops;
            source.volume = _volume;
            source.pitch = _pitch;
            source.playOnAwake = false;

            instance.transform.SetParent(_audioService.transform);
            instance.transform.position = _position;

            return sound;
        }

        private async Task<AudioMixerGroup> GetAudioMixerGroupAsync()
        {
            if (string.IsNullOrEmpty(_audioMixerGroup))
                return null;

            return (await _audioService.FindMatchingGroupsAsync(_audioMixerGroup))?.FirstOrDefault();
        }

        private async Task<AudioClip> GetAudioClipAsync()
        {
            if (_audioClip != null)
                return _audioClip;

            if (!string.IsNullOrEmpty(_audioClipAddress))
            {
                _asyncOperationHandle = Addressables.LoadAssetAsync<AudioClip>(_audioClipAddress);
                return await _asyncOperationHandle.Task;
            }

            if (_audioClipReference != null)
            {
                _asyncOperationHandle = _audioClipReference.LoadAssetAsync<AudioClip>();
                return await _asyncOperationHandle.Task;
            }

            return null;
        }

        public void Unload()
        {
            if(_audioClipReference != null)
            {
                _audioClipReference.ReleaseAsset();
                _audioClipReference = null;
            }

            Addressables.Release(_asyncOperationHandle);
        }
    }
}
