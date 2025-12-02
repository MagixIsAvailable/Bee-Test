using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ProceduralMeadowGenerator : MonoBehaviour
{
    [Header("World")]
    [Tooltip("Meters along X and Z. 200 = 200m.")]
    public float sizeX = 200f;
    public float sizeZ = 200f;
    public int seed = 12345;

    [Header("Density / Noise")]
    [Range(0f, 2f)] public float globalDensity = 1.0f;         // overall multiplier
    [Tooltip("Bigger = wider patches.")]
    public float densityNoiseScale = 0.01f;                     // Perlin scale
    public Vector2 densityNoiseOffset = new Vector2(100f, 250f);

    [Header("Biome Mask (0..1)")]
    [Tooltip("Perlin determining biome blend (0 = meadow; 1 = marsh/edge).")]
    public float biomeNoiseScale = 0.005f;
    public Vector2 biomeNoiseOffset = new Vector2(420f, 1337f);
    [Range(0f,1f)] public float biomeThreshold = 0.55f;         // split between two biome lists

    [Header("Poisson Sampling")]
    [Tooltip("Minimum spacing in meters between flowers at neutral density.")]
    public float baseMinRadius = 1.2f;
    [Tooltip("How much density noise changes spacing. 0 = constant spacing.")]
    [Range(0f, 1f)] public float radiusVariance = 0.5f;         // 0..1

    [Header("Prefabs")]
    [Tooltip("Species that appear in low biome value (meadow).")]
    public GameObject[] meadowSpecies;                          // e.g., clovers, daisies
    [Tooltip("Species that appear in high biome value (edge/marsh).")]
    public GameObject[] edgeSpecies;                            // e.g., knapweed, vetchling
    [Tooltip("Optional rock/tree/log landmarks (placed sparsely).")]
    public GameObject[] landmarks;
    [Tooltip("Optional water sources (pond/puddle prefabs).")]
    public GameObject[] waterSources;

    [Header("Counts")]
    [Tooltip("Target number of flower candidates before Poisson rejects.")]
    public int candidateCount = 2500;
    [Tooltip("Approx number of landmarks.")]
    public int landmarkCount = 10;
    [Tooltip("Approx number of water sources.")]
    public int waterCount = 3;

    [Header("Y Placement")]
    public float groundY = 0f;     // if you use Terrain, swap this for a sampler

    [Header("Gizmos")]
    public bool showBounds = true;
    public bool showSamples = false;

    // runtime
    private Transform _root;
    private System.Random _rng;
    private List<Vector2> _placed2D = new List<Vector2>();

    void OnValidate()
    {
        sizeX = Mathf.Max(10f, sizeX);
        sizeZ = Mathf.Max(10f, sizeZ);
        baseMinRadius = Mathf.Max(0.2f, baseMinRadius);
        candidateCount = Mathf.Clamp(candidateCount, 100, 20000);
    }

    [ContextMenu("Build World")]
    public void Build()
    {
        Clear();
        _root = new GameObject("~World_Runtime").transform;
        _root.SetParent(transform, false);
        _rng = new System.Random(seed);
        _placed2D.Clear();

        // FLOWERS
        var flowersParent = new GameObject("Flowers").transform;
        flowersParent.SetParent(_root, false);

        // Generate candidate positions (blue-noise-ish via Poisson)
        var rect = new Rect(-sizeX * 0.5f, -sizeZ * 0.5f, sizeX, sizeZ);
        var poisson = new PoissonDiskSampler(rect, baseMinRadius, _rng);

        // We’ll vary radius per point using density noise so pass a callback:
        IEnumerable<Vector2> points = poisson.SamplesWithVariableRadius((Vector2 p) =>
        {
            // density 0..1 from Perlin
            float dn = Density01(p);
            // more dense => smaller radius; less dense => larger radius
            float r = Mathf.Lerp(baseMinRadius * (1f - radiusVariance),
                                 baseMinRadius * (1f + radiusVariance),
                                 1f - dn) / Mathf.Max(0.2f, globalDensity);
            return Mathf.Clamp(r, 0.2f, baseMinRadius * 2.5f);
        }, candidateCount);

        int placed = 0;
        foreach (var p in points)
        {
            // Choose biome & species
            float biome = Biome01(p);
            var list = biome < biomeThreshold ? meadowSpecies : edgeSpecies;
            if (list == null || list.Length == 0) continue;

            var prefab = list[_rng.Next(0, list.Length)];
            if (prefab == null) continue;

            // Optional: thin out based on density (skip some points in low-density zones)
            float keep = Density01(p); // 0..1
            if (Sample01(_rng) > keep) continue;

            // Drop the instance
            Vector3 pos = new Vector3(p.x, groundY, p.y) + transform.position;
            Quaternion rot = Quaternion.Euler(0f, (float)Sample01(_rng) * 360f, 0f);
            var go = Instantiate(prefab, pos, rot, flowersParent);

            // Random uniform scale jitter (±15%)
            float s = 1f + ((float)Sample01(_rng) * 0.3f - 0.15f);
            go.transform.localScale *= Mathf.Clamp(s, 0.7f, 1.3f);

            _placed2D.Add(p);
            placed++;
        }

        // LANDMARKS
        if (landmarks != null && landmarks.Length > 0 && landmarkCount > 0)
        {
            var lmParent = new GameObject("Landmarks").transform;
            lmParent.SetParent(_root, false);
            for (int i = 0; i < landmarkCount; i++)
            {
                var p = RandomPointInRect(rect);
                var pf = landmarks[_rng.Next(0, landmarks.Length)];
                Vector3 pos = new Vector3(p.x, groundY, p.y) + transform.position;
                var go = Instantiate(pf, pos, Quaternion.Euler(0, (float)Sample01(_rng) * 360f, 0), lmParent);
                float scale = 0.9f + (float)Sample01(_rng) * 0.6f;
                go.transform.localScale *= scale;
            }
        }

        // WATER
        if (waterSources != null && waterSources.Length > 0 && waterCount > 0)
        {
            var wParent = new GameObject("Water").transform;
            wParent.SetParent(_root, false);
            for (int i = 0; i < waterCount; i++)
            {
                var p = RandomPointInRect(rect);
                var pf = waterSources[_rng.Next(0, waterSources.Length)];
                Vector3 pos = new Vector3(p.x, groundY, p.y) + transform.position;
                var go = Instantiate(pf, pos, Quaternion.identity, wParent);
                float scale = 0.8f + (float)Sample01(_rng) * 0.8f;
                go.transform.localScale *= scale;
            }
        }

        Debug.Log($"[ProceduralMeadowGenerator] Placed {placed} flowers, {landmarkCount} landmarks, {waterCount} water.");
    }

    [ContextMenu("Clear World")]
    public void Clear()
    {
        var child = transform.Find("~World_Runtime");
        if (child != null) DestroyImmediate(child.gameObject);
    }

    // ---------- Helpers ----------
    float Density01(Vector2 p)
    {
        float nx = p.x * densityNoiseScale + densityNoiseOffset.x;
        float nz = p.y * densityNoiseScale + densityNoiseOffset.y;
        return Mathf.Clamp01(Mathf.PerlinNoise(nx, nz));
    }

    float Biome01(Vector2 p)
    {
        float nx = p.x * biomeNoiseScale + biomeNoiseOffset.x;
        float nz = p.y * biomeNoiseScale + biomeNoiseOffset.y;
        return Mathf.Clamp01(Mathf.PerlinNoise(nx, nz));
    }

    Vector2 RandomPointInRect(Rect r)
    {
        return new Vector2(
            (float)(_rng.NextDouble() * r.width + r.xMin),
            (float)(_rng.NextDouble() * r.height + r.yMin)
        );
    }

    double Sample01(System.Random rnd) => rnd.NextDouble();

    void OnDrawGizmosSelected()
    {
        if (!showBounds) return;
        Gizmos.color = Color.yellow;
        Vector3 c = transform.position;
        Gizmos.DrawWireCube(c, new Vector3(sizeX, 0.1f, sizeZ));

            if (showSamples && _placed2D != null)
            {
                Gizmos.color = new Color(0.2f, 0.8f, 0.3f, 0.8f);
                foreach (var p in _placed2D)
                    Gizmos.DrawSphere(new Vector3(p.x, groundY + 0.05f, p.y) + transform.position, 0.1f);
            }
        }
    }
