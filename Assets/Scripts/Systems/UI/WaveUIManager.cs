using TMPro;
using UnityEngine;

public class WaveUIManager : MonoBehaviour
{
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private TMP_Text waveLabel;
    [SerializeField] private string waveLabelFormat = "Wave {0}";

    private void Start()
    {
        ResolveReferences();
        RefreshLabel();
    }

    private void ResolveReferences()
    {
        if (waveSpawner == null)
        {
            waveSpawner = FindFirstObjectByType<WaveSpawner>();
        }

        if (waveLabel == null)
        {
            TMP_Text[] labels = GetComponentsInChildren<TMP_Text>(true);
            for (int i = 0; i < labels.Length; i++)
            {
                if (labels[i] != null && labels[i].name == "WaveLabel")
                {
                    waveLabel = labels[i];
                    break;
                }
            }

            if (waveLabel == null && labels.Length > 0)
            {
                waveLabel = labels[0];
            }
        }
    }

    private void OnEnable()
    {
        ResolveReferences();
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveStarted += HandleWaveStarted;
        }
    }

    private void OnDisable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveStarted -= HandleWaveStarted;
        }
    }

    private void HandleWaveStarted(int waveNumber)
    {
        SetWaveLabel(waveNumber);
    }

    private void RefreshLabel()
    {
        int wave = waveSpawner != null ? Mathf.Max(1, waveSpawner.CurrentWave) : 1;
        SetWaveLabel(wave);
    }

    private void SetWaveLabel(int waveNumber)
    {
        if (waveLabel == null)
        {
            return;
        }

        string formatToUse = string.IsNullOrWhiteSpace(waveLabelFormat) ? "Wave {0}" : waveLabelFormat;
        waveLabel.text = string.Format(formatToUse, Mathf.Max(1, waveNumber));
    }
}
