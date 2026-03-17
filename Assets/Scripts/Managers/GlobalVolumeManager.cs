using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeManager : MonoBehaviour
{
    public static GlobalVolumeManager Instance;

    [SerializeField] private Volume volume;

    public Bloom Bloom { get; private set; }
    public Vignette Vignette { get; private set; }
    public ChromaticAberration Chromatic { get; private set; }
    public ColorAdjustments ColorAdjustments { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    void Initialize()
    {
        volume = GetComponent<Volume>();

        var profile = volume.profile;

        Bloom bloom;
        Vignette vignette;
        ChromaticAberration chromatic;
        ColorAdjustments colorAdjustments;

        profile.TryGet(out bloom);
        profile.TryGet(out vignette);
        profile.TryGet(out chromatic);
        profile.TryGet(out colorAdjustments);

        Bloom = bloom;
        Vignette = vignette;
        Chromatic = chromatic;
        ColorAdjustments = colorAdjustments;
        //Debug.Log("Bloom found: " + (Bloom != null));
        //Debug.Log("Vignette found: " + (Vignette != null));
        //Debug.Log("Chromatic found: " + (Chromatic != null));
    }
}