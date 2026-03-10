using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private HealthComponent playerHealth;

    [Header("Win Condition")]
    [SerializeField] private int winWave = 10;

    [Header("End Flow")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private float endScreenDuration = 3f;

    private bool gameEnded;
    private Coroutine endFlowRoutine;

    private void Awake()
    {
        ResolveReferences();
        SetScreenState(victoryScreen, false);
        SetScreenState(defeatScreen, false);
    }

    private void ResolveReferences()
    {
        if (waveSpawner == null)
        {
            waveSpawner = FindFirstObjectByType<WaveSpawner>();
        }

        if (playerHealth == null)
        {
            HealthComponent[] healthComponents = FindObjectsByType<HealthComponent>(FindObjectsSortMode.None);
            for (int i = 0; i < healthComponents.Length; i++)
            {
                if (healthComponents[i] != null && healthComponents[i].Type == DamagableType.Player)
                {
                    playerHealth = healthComponents[i];
                    break;
                }
            }
        }

        if (victoryScreen == null)
        {
            GameObject found = FindSceneObjectByName("VictoryScreen");
            if (found != null)
            {
                victoryScreen = found;
            }
        }

        if (defeatScreen == null)
        {
            GameObject found = FindSceneObjectByName("DefeatScreen");
            if (found != null)
            {
                defeatScreen = found;
            }
        }
    }

    private static GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            GameObject candidate = allObjects[i];
            if (candidate != null && candidate.scene.IsValid() && candidate.name == objectName)
            {
                return candidate;
            }
        }

        return null;
    }

    private void OnEnable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveStarted += HandleWaveStarted;
        }

        if (playerHealth != null)
        {
            playerHealth.OnDied += HandlePlayerDied;
        }
    }

    private void OnDisable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveStarted -= HandleWaveStarted;
        }

        if (playerHealth != null)
        {
            playerHealth.OnDied -= HandlePlayerDied;
        }
    }

    private void HandleWaveStarted(int waveNumber)
    {
        if (gameEnded)
        {
            return;
        }

        if (waveNumber >= Mathf.Max(1, winWave))
        {
            TriggerWin();
        }
    }

    private void HandlePlayerDied(HealthComponent health)
    {
        if (gameEnded || health == null)
        {
            return;
        }

        if (health.Type == DamagableType.Player)
        {
            TriggerDefeat();
        }
    }

    private void TriggerWin()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        if (waveSpawner != null)
        {
            waveSpawner.StopSpawning();
        }

        SetScreenState(victoryScreen, true);
        SetScreenState(defeatScreen, false);
        StartRestartCountdown();
    }

    private void TriggerDefeat()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        if (waveSpawner != null)
        {
            waveSpawner.StopSpawning();
        }

        SetScreenState(victoryScreen, false);
        SetScreenState(defeatScreen, true);
        StartRestartCountdown();
    }

    private void StartRestartCountdown()
    {
        if (endFlowRoutine != null)
        {
            StopCoroutine(endFlowRoutine);
        }

        endFlowRoutine = StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        float delay = Mathf.Max(0f, endScreenDuration);
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private static void SetScreenState(GameObject screen, bool isVisible)
    {
        if (screen != null)
        {
            screen.SetActive(isVisible);
        }
    }
}
