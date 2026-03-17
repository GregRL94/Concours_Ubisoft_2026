using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Audio/Audio Library")]
public class AudioLibrarySO : ScriptableObject
{
    public List<SoundData> sounds = new();

    private Dictionary<string, SoundData> lookup;

    public SoundData Get(string id)
    {
        if (lookup == null)
        {
            lookup = new Dictionary<string, SoundData>();
            foreach (var s in sounds)
            {
                if (!lookup.ContainsKey(s.id))
                    lookup.Add(s.id, s);
            }
        }
        
        return lookup.TryGetValue(id, out var sound) ? sound : null;
    }
}

