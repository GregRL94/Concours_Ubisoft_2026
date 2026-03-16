using UnityEngine;

public enum AudioType
{
    Music,
    SFX,
    UI
}

[System.Serializable]
public class SoundData
{
    public string id; // Examples: "UI_PlayGame", "SFX_Jump", "Music_Gameplay"
    public AudioClip clip;
    public AudioType type;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3, 3f)] public float pitch = 1f;
    public bool loop;
    public bool randomizePitch;
    [Range(0f, 0.5f)] public float pitchRange = 0.1f;
}


