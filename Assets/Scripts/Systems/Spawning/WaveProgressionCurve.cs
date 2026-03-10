using UnityEngine;

[CreateAssetMenu(menuName = "Game/Spawning/Wave Progression", fileName = "WaveProgression")]
public class WaveProgressionCurve : ScriptableObject
{
    [Header("Wave Credits")]
    [SerializeField] private int minimumWaveCredits = 1;
    [SerializeField] private AnimationCurve waveCreditsByWave = new AnimationCurve(
        new Keyframe(1f, 6f),
        new Keyframe(5f, 14f),
        new Keyframe(10f, 24f));

    [Header("Cost Bias")]
    [SerializeField] private AnimationCurve expensiveUnitPreferenceByWave = new AnimationCurve(
        new Keyframe(1f, 0.15f),
        new Keyframe(5f, 0.45f),
        new Keyframe(10f, 0.85f));

    public int GetWaveCredits(int waveNumber)
    {
        float wave = Mathf.Max(1, waveNumber);
        int evaluatedCredits = Mathf.RoundToInt(waveCreditsByWave.Evaluate(wave));
        return Mathf.Max(minimumWaveCredits, evaluatedCredits);
    }

    public float GetCostSelectionMultiplier(int enemyCost, int maxAffordableCost, int waveNumber)
    {
        float clampedPreference = Mathf.Clamp01(expensiveUnitPreferenceByWave.Evaluate(Mathf.Max(1, waveNumber)));
        int safeMaxCost = Mathf.Max(1, maxAffordableCost);
        float normalizedCost = Mathf.InverseLerp(1f, safeMaxCost, Mathf.Max(1, enemyCost));
        float cheapBias = 1f - normalizedCost;
        float expensiveBias = normalizedCost;
        float mixed = Mathf.Lerp(cheapBias, expensiveBias, clampedPreference);
        return Mathf.Max(0.05f, mixed + 0.1f);
    }
}
