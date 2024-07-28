using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    [SerializeField] int maxHeight;
    [SerializeField] float smooth;
    [SerializeField] int octaves;
    [SerializeField] float persistence;

    public static PerlinNoise Instance;

    void Awake()
    {
        Instance = this;
    }

    public int GenerateRockHeight(float x, float z)
    {
        int height = (int)Map(0, maxHeight - 5, 0, 1, MathMagic(x * smooth * 2, z * smooth * 2, octaves + 1, persistence));
        return height;
    }

    public int GenerateHeight(float x, float z)
    {
        int height = (int) Map(0, maxHeight, 0, 1, MathMagic(x * smooth, z * smooth, octaves, persistence));
        return height;
    }

    private float Map(float newmin, float newmax, float originmin, float originmax, float value)
    {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(originmin, originmax, value));
    }

    /// <summary>
    /// Factorial Brownian Motion
    /// Calculate each perlin noise to generate height value and add it to next one.
    /// </summary>
    /// <returns>
    /// Average of all noise values.
    /// </returns>
    private float MathMagic(float x, float z, int oct, float pers)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < oct; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= pers;
            frequency *= 2;
        }

        return total / maxValue;
    }
}
