using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AudioServices.Interfaces
{
    public interface ISoundBuilder
    {
        Task<ISound> Build();
        void Unload();
        ISoundBuilder With3DPosition(Vector3 position);
        ISoundBuilder WithAssetAddress(string address);
        ISoundBuilder WithAssetReference(AssetReference assetReference);
        ISoundBuilder WithAudioClip(AudioClip audioClip);
        ISoundBuilder WithAudioMixerGroup(string audioMixerGroup);
        ISoundBuilder WithAudioRolloffMode(AudioRolloffMode audioRolloffMode);
        ISoundBuilder WithCategory(string category);
        ISoundBuilder WithDoppler(float dopplerLevel);
        ISoundBuilder WithLooping(bool doesLoop = true);
        ISoundBuilder WithMaxDistance(float maxDistance);
        ISoundBuilder WithMinDistance(float minDistance);
        ISoundBuilder WithPitch(float pitch);
        ISoundBuilder WithQueuedSoundTween(ISoundTween soundTween);
        ISoundBuilder WithSpatialBlend(float spatialBlend);
        ISoundBuilder WithVolume(float volume);
    }
}