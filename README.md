# Audio Services
Audio Services is an Addressables/Task focused audio system. This package requires a minimum of 2019.3.

## Concepts

### AudioService
The AudioService is the main component required in the scene to process all requests.

### Sound
The Sound is a light wrapper around an AudioSource. Each sound is a child of the AudioService.

### SoundBuilder
Since there is an infinite amount of ways to configure an AudioSource for playback, we utilize the builder pattern to return a Sound.

### ISoundTween
ISoundTween is a queuable class that executes during playback of a Sound. You can implement your own tween to modify different effects on the AudioSource. `FadeVolumeSoundTween` is currently the only included tween available.

## Setting Up Your Project

### Audio Mixer
Create an AudioMixer and mark it as an Addressable. Then assign it to the AssetReference field on the AudioService component.

#### Audio Mixer Groups
Create mixer groups as normal. These will be referenced when building the sound via the `WithAudioMixerGroup` method.

### Audio Clips
Mark each AudioClip as an Addressable if you want to utilize it for this system.

### Creating Sounds via the SoundBuilder
You'll need to load the indvidiual sounds into the system before they can be used. You chain the various options and then finish with `Build()` which returns a `Task<Sound>`. If you `await` within an `async` method you can get the reference to the newly created `Sound`

```csharp
var sound = await audioService.CreateSoundBuilder("mysound")
                                .WithAssetAddress("Assets/Audio/click.wav")
                                .WithAudioMixerGroup("SFX")
                                .Build();
```

### Finding Existing Sounds
You can find existing sounds by querying with the AudioService.

```csharp
var projectileSounds = audioService.FindSounds((sound) => sound.Category == "Projectile");
var mySound = audioService.FirstOrDefaultById("mysound");
```

### Playing Sounds

#### Standard Play
A standard play utilizes the `AudioSource.Play()` method. You can utilize the `Completed` callback and any `ISoundTween` queued.

```csharp
mySound.Play();

// Or if you want to wait until clip is done...
await mySound.PlayAsync();
```

#### OneShot
One shot utilizes `AudioSource.PlayOneShot`. The `Completed` callback will not fire and `ISoundTween` is not taken into account. This is perfect for UI hover or click effects because OneShot allows you to stack audio without requiring an instance of an AudioSource for each.

```csharp
myClick.PlayOneShot();
```

### Unloading Sounds
Since these assets are loaded via the Addressable system, we want to ensure we clean-up any sounds no longer needed. Utilize the `Sound.Unload()` method.

```csharp
// Cleans up the references in Addressable and destroys the AudioSource.
mySound.Unload();
```