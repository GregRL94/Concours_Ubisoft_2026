using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//public class AudioManager : MonoBehaviour
//{
//    public static AudioManager Instance;

//    [Header("Library")]
//    [SerializeField] private AudioLibrarySO library;

//    [Header("Mixers")]
//    [SerializeField] private AudioMixerGroup musicMixer;
//    [SerializeField] private AudioMixerGroup sfxMixer;
//    [SerializeField] private AudioMixerGroup uiMixer;

//    private AudioSource currentMusic;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void PlaySound(string soundId, Vector3? position = null)
//    {
//        SoundData sound = library.Get(soundId);

//        if (sound == null || sound.clip == null)
//        {
//            Debug.LogWarning($"[AudioManager] Sound not found: {soundId}");
//            return;
//        }

//        GameObject go = new($"Audio_{soundId}");

//        if (position.HasValue) go.transform.position = position.Value;
//        AudioSource source = go.AddComponent<AudioSource>();

//        source.clip = sound.clip;
//        source.volume = sound.volume;
//        source.pitch = GetPitch(sound);
//        source.loop = sound.loop;
//        source.outputAudioMixerGroup = GetMixer(sound.type);

//        //// Spatial logic for 3d or 2d games
//        source.spatialBlend = position.HasValue ? 1f : 0f;

//        source.Play();

//        // Behaviors of different types of Audio played
//        switch (sound.type)
//        {
//            case AudioType.Music:
//                if (currentMusic != null)
//                    currentMusic.Stop();
//                currentMusic = source;
//                break;

//            case AudioType.SFX:
//            case AudioType.UI:
//                if (!sound.loop)
//                    Destroy(go, source.clip.length);
//                break;
//        }
//    }



//    private float GetPitch(SoundData sound)
//    {
//        if (!sound.randomizePitch) return sound.pitch;
//        float randomPitchRange = Random.Range(-sound.pitchRange, sound.pitchRange);
//        return sound.pitch + randomPitchRange;
//    }


//    private AudioMixerGroup GetMixer(AudioType type)
//    {
//        //return type switch
//        //{
//        //    AudioType.Music => musicMixer,
//        //    AudioType.SFX => sfxMixer,
//        //    AudioType.UI => uiMixer,
//        //};

//        AudioMixerGroup mixerGroup = null;
//        switch (type) 
//        {
//            case AudioType.Music:
//                mixerGroup = musicMixer;
//                break;
//            case AudioType.SFX:
//                mixerGroup = sfxMixer;
//                break;

//            case AudioType.UI:
//                mixerGroup = uiMixer;
//                break;
//            default:
//                break;
//        }
//        return mixerGroup;
//    }




//}


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Library")]
    [SerializeField] private AudioLibrarySO library;

    [Header("Mixers")]
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private AudioMixerGroup uiMixer;

    [Header("CrossFade Variables")]
    [SerializeField] private float crossFadeDuration = 0.5f;
    
    private AudioSource currentMusic;
    private string currentMusicId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // MUSIC - PERSISTENT
    public void PlayMusic(string musicId)
    {
        if (currentMusicId == musicId)
            return; 

        SoundData sound = library.Get(musicId);

        if (sound == null || sound.clip == null)
        {
            Debug.LogWarning($"[AudioManager] Music not found: {musicId}");
            return;
        }

        if (currentMusic != null)
        {
            Destroy(currentMusic.gameObject);
            currentMusic = null;
        }

        GameObject go = new($"{musicId}");
        DontDestroyOnLoad(go);

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.loop = true;
        source.outputAudioMixerGroup = musicMixer;
        source.spatialBlend = 0f;

        source.Play();

        currentMusic = source;
        currentMusicId = musicId;
    }
    public void CrossFadeMusic(string newMusicId)
    {
        if (currentMusicId == newMusicId)
            return;

        SoundData sound = library.Get(newMusicId);
        if (sound == null || sound.clip == null)
        {
            Debug.LogWarning($"[AudioManager] Music not existant ! : {newMusicId}");
            return;
        }

        StartCoroutine(CrossFadeRoutine(sound, newMusicId));
    }
    private IEnumerator CrossFadeRoutine(SoundData newSound, string newMusicId)
    {
        AudioSource oldMusic = currentMusic;

        GameObject go = new($"{newMusicId}");
        DontDestroyOnLoad(go);

        AudioSource newMusic = go.AddComponent<AudioSource>();
        newMusic.clip = newSound.clip;
        newMusic.volume = newSound.volume;
        newMusic.loop = newSound.loop;
        newMusic.outputAudioMixerGroup = musicMixer;
        newMusic.spatialBlend = 0f;
        newMusic.outputAudioMixerGroup = GetMixer(newSound.type);
        
        newMusic.Play();

        float time = 0f;
        while (time < crossFadeDuration)
        {
            if (newMusic == null ) yield break;

            time += Time.deltaTime;
            float t = time / crossFadeDuration;

            if (oldMusic != null)
                oldMusic.volume = Mathf.Lerp(oldMusic.volume, 0f, t);

            newMusic.volume = Mathf.Lerp(0f, newSound.volume, t);

            yield return null;
        }

        if (oldMusic != null)
            Destroy(oldMusic.gameObject);

        currentMusic = newMusic;
        currentMusicId = newMusicId;
    }

    public void StopMusic()
    {
        StopAllCoroutines();

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);


        foreach (AudioSource source in allAudioSources)
        {
            Destroy(source.gameObject);
        }

        currentMusic = null;
        currentMusicId = null;
    }

    //  SFX / UI - ONE SHOT
    public void PlaySound(string soundId, Vector3? position = null)
    {
        SoundData sound = library.Get(soundId);

        if (sound == null || sound.clip == null)
        {
            Debug.LogWarning($"[AudioManager] Sound not found: {soundId}");
            return;
        }

        GameObject go = new($"{soundId}");
        DontDestroyOnLoad(go);

        if (position.HasValue)
            go.transform.position = position.Value;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = GetPitch(sound);
        source.loop = sound.loop;
        source.outputAudioMixerGroup = GetMixer(sound.type);
        source.spatialBlend = position.HasValue ? 1f : 0f;

        source.Play();

        if (!sound.loop)
            Destroy(go, source.clip.length);
    }

    // GETTERS
    private float GetPitch(SoundData sound)
    {
        if (!sound.randomizePitch) return sound.pitch;
        return sound.pitch + Random.Range(-sound.pitchRange, sound.pitchRange);
    }

    private AudioMixerGroup GetMixer(AudioType type)
    {
        switch (type)
        {
            case AudioType.Music: return musicMixer;
            case AudioType.SFX: return sfxMixer;
            case AudioType.UI: return uiMixer;
            default: return null;
        }
    }


    //    // Other potential methods
    //    RandomSound();
    //    ???

}

