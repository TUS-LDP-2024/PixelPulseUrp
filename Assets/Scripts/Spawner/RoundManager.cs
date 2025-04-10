using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    public List<ZombieSpawner> AllSpawners { get; private set; } = new List<ZombieSpawner>();

    [Header("Round Settings")]
    public float timeBetweenRounds = 5f;
    public int baseZombies = 8;
    public float spawnIncreaseFactor = 1.2f;

    [Header("UI")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI countdownText;

    [Header("Card System")]
    public CardSelectionUI cardSelectionUI;

    public int currentRound { get; private set; } = 0;
    public int ZombiesToSpawnThisRound { get; private set; } = 0;
    public int ZombiesSpawnedThisRound { get; private set; } = 0;
    public int ZombiesAlive { get; private set; } = 0;
    public bool IsRoundActive { get; private set; } = false;
    public bool IsSelectingUpgrade { get; private set; } = false;

    public bool tutorialMode = true; // flag

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ZombiesToSpawnThisRound = baseZombies;

        if (!tutorialMode)
        {
            StartNewRound();
        }
        else
        {
            Debug.Log("Tutorial active: Waiting to start rounds until tutorial completes.");
        }
    }

    public void StartNewRound()
    {
        countdownText.gameObject.SetActive(false);

        currentRound++;
        ZombiesAlive = 0;
        ZombiesSpawnedThisRound = 0;

        if (currentRound > 1)
        {
            ZombiesToSpawnThisRound = Mathf.RoundToInt(baseZombies * Mathf.Pow(spawnIncreaseFactor, currentRound - 1));
        }

        IsRoundActive = true;
        IsSelectingUpgrade = false;
        roundText.text = $"ROUND {currentRound}";
        Debug.Log($"Started Round {currentRound}. Zombies to spawn: {ZombiesToSpawnThisRound}");

        foreach (var spawner in AllSpawners)
        {
            spawner.enabled = true;
        }
    }

    public void IncrementSpawnedCount()
    {
        ZombiesSpawnedThisRound++;
        ZombiesAlive++;
        Debug.Log($"Spawned zombie {ZombiesSpawnedThisRound}/{ZombiesToSpawnThisRound}. Alive: {ZombiesAlive}");
    }

    public void DecrementAliveCount()
    {
        ZombiesAlive--;
        Debug.Log($"Zombie died. Alive: {ZombiesAlive}. Total killed: {ZombiesSpawnedThisRound - ZombiesAlive}");

        if (ZombiesSpawnedThisRound >= ZombiesToSpawnThisRound && ZombiesAlive <= 0)
        {
            StartCoroutine(CompleteRound());
        }
    }

    public bool CanSpawnMoreZombies()
    {
        return IsRoundActive && ZombiesSpawnedThisRound < ZombiesToSpawnThisRound;
    }

    private IEnumerator SmoothTimeScaleTransition(float targetTimeScale, float duration)
    {
        float initialTimeScale = Time.timeScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(initialTimeScale, targetTimeScale, elapsed / duration);
            yield return null;
        }
        Time.timeScale = targetTimeScale;
    }

    private IEnumerator CompleteRound()
    {
        IsRoundActive = false;
        IsSelectingUpgrade = true;

        yield return StartCoroutine(SmoothTimeScaleTransition(0.2f, 0.5f));
        yield return new WaitForSecondsRealtime(1.5f);
        yield return StartCoroutine(SmoothTimeScaleTransition(0f, 0.2f));

        if (cardSelectionUI != null)
        {
            cardSelectionUI.ShowRandomUpgrade();
            Debug.Log("Waiting for card selection...");
            yield return new WaitWhile(() => IsSelectingUpgrade);
        }

        countdownText.gameObject.SetActive(true);

        float timer = timeBetweenRounds;
        while (timer > 0)
        {
            countdownText.text = $"Next round in: {timer:F1}";
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        yield return StartCoroutine(SmoothTimeScaleTransition(1f, 0.5f));

        StartNewRound();
    }

    public void ResumeAfterCardSelection()
    {
        IsSelectingUpgrade = false;
        Time.timeScale = 1f;
    }

    public void RegisterSpawner(ZombieSpawner spawner)
    {
        if (!AllSpawners.Contains(spawner))
        {
            AllSpawners.Add(spawner);
            spawner.enabled = IsRoundActive;
        }
    }
}