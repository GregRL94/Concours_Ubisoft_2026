using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeManager : MonoBehaviour
{
    public static GlobalVolumeManager Instance;

    [SerializeField] private Volume volume;

    public Bloom Bloom { get; private set; }
    public Vignette Vignette { get; private set; }
    public ColorAdjustments ColorAdjustments { get; private set; }
    public ChromaticAberration Chromatic { get; private set; }
    public FilmGrain FilmGrain { get; private set; }
    public LensDistortion LensDistortion { get; private set; }
    public PaniniProjection PaniniProjection { get; private set; }

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
        volume = GetComponentInChildren<Volume>();

        var profile = volume.profile;

        Bloom bloom;
        Vignette vignette;
        ChromaticAberration chromatic;
        ColorAdjustments colorAdjustments;
        FilmGrain filmGrain;
        LensDistortion lensDistortion;
        PaniniProjection paniniProjection;

        profile.TryGet(out bloom);
        profile.TryGet(out vignette);
        profile.TryGet(out chromatic);
        profile.TryGet(out colorAdjustments);
        profile.TryGet(out filmGrain);
        profile.TryGet(out lensDistortion);
        profile.TryGet(out paniniProjection);

        Bloom = bloom;
        Vignette = vignette;
        Chromatic = chromatic;
        ColorAdjustments = colorAdjustments;
        FilmGrain = filmGrain;
        LensDistortion = lensDistortion;
        PaniniProjection = paniniProjection;

    }
}