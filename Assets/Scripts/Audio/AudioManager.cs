using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


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

    [Header("Random Music Playlist")]
    private string[] currentPlaylist;
    private Coroutine playlistRoutine;

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


    // MUSIC 
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

    // CROSSFADE MUSIC
    public void CrossFadeMusic(string newMusicId, bool forceNoLoop = false)
    {
        if (currentMusicId == newMusicId)
            return;

        SoundData sound = library.Get(newMusicId);
        if (sound == null || sound.clip == null)
        {
            Debug.LogWarning($"[AudioManager] Music not existant ! : {newMusicId}");
            return;
        }

        StartCoroutine(CrossFadeRoutine(sound, newMusicId, forceNoLoop));
    }
    private IEnumerator CrossFadeRoutine(SoundData newSound, string newMusicId, bool forceNoLoop)
    {
        AudioSource oldMusic = currentMusic;

        GameObject go = new($"{newMusicId}");
        DontDestroyOnLoad(go);

        AudioSource newMusic = go.AddComponent<AudioSource>();
        newMusic.clip = newSound.clip;
        newMusic.volume = newSound.volume;
        newMusic.loop = forceNoLoop ? false : newSound.loop; // si sa vient dune playlist loop false sinon true
        newMusic.outputAudioMixerGroup = musicMixer;
        newMusic.spatialBlend = 0f;
        
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

    // MUSIC PLAYLIST
    public void StopPlaylist()
    {
        if (playlistRoutine != null)
            StopCoroutine(playlistRoutine);
    }
    public void PlayRandomPlaylist(string[] musicIds)
    {
        if (musicIds == null || musicIds.Length == 0)
        {
            Debug.LogWarning("[AudioManager] Playlist vide !");
            return;
        }

        currentPlaylist = musicIds;

        if (playlistRoutine != null)
            StopCoroutine(playlistRoutine);

        playlistRoutine = StartCoroutine(RandomPlaylistRoutine());
    }

    private IEnumerator RandomPlaylistRoutine()
    {
        string lastPlayed = null;

        while (true)
        {
            // Choisir une musique random != précédente
            string next = GetRandomMusic(lastPlayed);

            if (string.IsNullOrEmpty(next))
            {
                Debug.LogWarning("[AudioManager] Aucune musique valide dans la playlist.");
                yield break;
            }

            SoundData sound = library.Get(next);

            if (sound == null || sound.clip == null)
            {
                Debug.LogWarning($"[AudioManager] Music invalide: {next}");
                yield return null;
                continue;
            }

            sound.loop = false;

            // Crossfade vers nouvelle musique
            CrossFadeMusic(next, true);

            currentMusicId = next;
            lastPlayed = next;

            // attendre la fin du clip
            yield return new WaitForSeconds(sound.clip.length);
        }
    }

    private string GetRandomMusic(string last)
    {
        if (currentPlaylist.Length <= 1)
            return currentPlaylist[0];

        string chosen = currentPlaylist[Random.Range(0, currentPlaylist.Length)];

        while (chosen == last)
        {
            chosen = currentPlaylist[Random.Range(0, currentPlaylist.Length)];
        }

        return chosen;
    }

    // STOP MUSIC
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

    // STOP SOUND
    public void StopSound(string soundId, bool hasFadeOut = false, float fadeOutDuration = 0.25f, Vector3? position = null)
    {
        GameObject go = GameObject.Find($"{soundId}");
        if (go == null)
        {
            Debug.LogWarning($"[GameObject] Sound to stop not found: {soundId}");
            return;
        }
        
        AudioSource source = go.GetComponent<AudioSource>();
        if (source == null)
            return;

        if (hasFadeOut)
        {
            StartCoroutine(FadeOutRoutine(source, go, fadeOutDuration));
        }
        else
        {
            source.Stop();
            Destroy(go);
        }
    }

    private IEnumerator FadeOutRoutine(AudioSource source, GameObject soundGO, float fadeOutDuration)
    {
        float time = 0f;
        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeOutDuration;
            if (source != null)
                source.volume = Mathf.Lerp(source.volume, 0f, t);

            yield return null;
        }

        source.Stop();
        Destroy(soundGO);
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

}

