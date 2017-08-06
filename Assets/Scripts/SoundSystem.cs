using UnityEngine;
using System.Collections.Generic;

class SoundSystem: MonoBehaviour
{
    public List<AudioSource> audioSources;

    public static SoundSystem s_soundSystem;

    void Start() {
        s_soundSystem = this;
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource freeSoundSource = null;
        foreach (AudioSource source in audioSources) {
            if (!source.isPlaying) {
                freeSoundSource = source;
            }
        }

        if (freeSoundSource == null) {
            return;
        }

        freeSoundSource.clip = clip;
        freeSoundSource.Play();
    }
}
