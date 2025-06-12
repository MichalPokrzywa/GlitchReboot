using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio/Audio Storage")]
public class AudioStorage : ScriptableObject
{
    [Header("Sounds")]
    [SerializeField] private List<SoundClip> sounds;

    [SerializeField] List<SpiderVoiceOver> spiderVoiceOvers;

    [Header("Menu music")]
    [SerializeField] private List<AudioClip> menuMusicClips;

    [Header("Gameplay music")]
    [SerializeField] private List<AudioClip> gameplayMusicClips;

    private AudioClip lastMenuMusicClip = null;
    private AudioClip lastGameplayMusicClip = null;

    [Serializable]
    class SpiderVoiceOver
    {
        public Scene scene;
        public List<AudioClip> clips;
    }

    public AudioClip GetSoundByType(Sound sound)
    {
        SoundClip soundClip = sounds.Find(soundClip => soundClip.type == sound);

        if (soundClip is null)
        {
            return null;
        }

        if (soundClip.clips.Count == 0)
        {
            Debug.LogError("No AudioClips defined for sound: " + sound.ToString());
            return null;
        }

        float probability = 0.90f;

        if (soundClip.clips.Count == 2)
        {
            if (Random.Range(0.0f, 1.0f) < probability)
                return soundClip.clips[1];
            return soundClip.clips[0];
        }

        return soundClip.clips[Random.Range(0, soundClip.clips.Count)];
    }

    public AudioClip GetRandomMenuMusic()
    {
        if (menuMusicClips.Count == 0)
        {
            Debug.LogError("No AudioClips defined for MenuMusic");
            return null;
        }

        AudioClip newClip;

        newClip = menuMusicClips[Random.Range(0, menuMusicClips.Count)];

        lastMenuMusicClip = newClip;
        return newClip;
    }

    public AudioClip GetRandomGameplayMusic()
    {
        if (gameplayMusicClips.Count == 0)
        {
            Debug.LogError("No AudioClips defined for GameplayMusic");
            return null;
        }

        AudioClip newClip;

        do
        {
            newClip = gameplayMusicClips[Random.Range(0, gameplayMusicClips.Count)];
        }
        while (newClip == lastGameplayMusicClip);

        lastGameplayMusicClip = newClip;
        return newClip;
    }

    public AudioClip GetSpiderVoiceOver(Scene scene, int id)
    {
        if (spiderVoiceOvers == null || spiderVoiceOvers.Count == 0)
            return null;

        SpiderVoiceOver voiceOver = spiderVoiceOvers.Find(vo => vo.scene == scene);
        if (voiceOver == null || voiceOver.clips.Count == 0)
            return null;

        if (id < 0 || id >= voiceOver.clips.Count)
            return null;

        return voiceOver.clips[id];
    }
}

[System.Serializable]
public class SoundClip
{
    public Sound type;
    public List<AudioClip> clips;
}

public enum Sound
{
    None = 0,
    Click = 1,
    BookPageTurning = 2,
    ObjectivePanelSwitch = 3,
    GuideShowing = 4,
    ObjectiveCompleted = 5,
}