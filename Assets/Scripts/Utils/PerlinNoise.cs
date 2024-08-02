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

    public int GenerateGrassHeight(float x, float z)
    {
        int height = (int) Map(0, maxHeight, 0, 1, MathMagic(x * smooth, z * smooth, octaves, persistence));
        return height;
    }

    public int GenerateSnowHeight(float x, float z)
    {
        int height = (int)Map(0, maxHeight + 5, 0, 1, MathMagic(x * smooth * 2, z * smooth * 2, octaves + 1, persistence));
        return height;
    }

    public float GenerateCaves(float x,float y, float z, float smooth, int octaves)
    {
        var xy = MathMagic(x * smooth, y * smooth, octaves, persistence);
        var yz = MathMagic(y * smooth, z * smooth, octaves, persistence);
        var xz = MathMagic(x * smooth, z * smooth, octaves, persistence);
        var yx = MathMagic(y * smooth, x * smooth, octaves, persistence);
        var zy = MathMagic(z * smooth, y * smooth, octaves, persistence);
        var zx = MathMagic(z * smooth, x * smooth, octaves, persistence);

        float value = (xy + yz + xz + yx + zy + zx) / 6;
        return value;
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
