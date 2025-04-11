using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

[System.Serializable]
public class TutorialImageAssignment
{
    public Image targetUIElement;
    public Sprite spriteToShow;
}

[System.Serializable]
public class TutorialStep
{
    [TextArea]
    public string message;
    public List<TutorialImageAssignment> imageAssignments;
    public bool triggerCutscene = false;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public CanvasGroup textCanvasGroup;

    [Header("Step System")]
    public List<TutorialStep> steps;

    [Header("Zombie")]
    public GameObject zombiePrefab;
    public Transform zombieSpawnPoint;
    public AudioClip tutorialZombieKilledClip;

    [Header("Cutscene")]
    public GameObject cutscenePrefab;
    public Transform cutsceneSpawnPoint;
    public GameObject player;
    public MonoBehaviour[] movementScriptsToDisable;
    public Camera mainCamera;

    [Header("UI Elements")]
    public GameObject crosshair;
    public GameObject ammoCounter;
    public GameObject roundDisplayText;


    public bool IsTutorialActive = true;
    


    private GameObject tutorialZombie;
    private List<Image> previouslyShownImages = new List<Image>();
    private bool zombieKilled = false;
    private bool finalCheckpointReached = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (roundDisplayText != null)
        {
            roundDisplayText.SetActive(false);
        }

    }

    void Start()
    {
        if (steps.Count > 0)
        {
            ShowTutorialStep(0);
        }
    }

    public void OnTutorialWaypointReached(int index)
    {
        if (index >= steps.Count) return;

        var step = steps[index];
        ShowTutorialStep(index);

        if (step.triggerCutscene)
        {
            PlayCutscene();
            return;
        }

        if (index == 3)
        {
            SpawnTutorialZombie();
        }

        if (index == steps.Count - 1)
        {
            EndTutorial();
        }

    }

    void ShowTutorialStep(int index)
    {
        if (index >= steps.Count) return;

        var step = steps[index];

        foreach (var img in previouslyShownImages)
        {
            if (img != null)
            {
                img.gameObject.SetActive(false);
                var cg = img.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0f;
            }
        }
        previouslyShownImages.Clear();

        tutorialPanel?.SetActive(true);

        if (tutorialText != null && textCanvasGroup != null)
        {
            tutorialText.text = step.message;
            StartCoroutine(FadeCanvasGroup(textCanvasGroup, 0f, 1f, 0.3f));
        }

        foreach (var assignment in step.imageAssignments)
        {
            if (assignment.targetUIElement == null) continue;

            if (assignment.spriteToShow != null)
            {
                assignment.targetUIElement.sprite = assignment.spriteToShow;
                assignment.targetUIElement.gameObject.SetActive(true);

                CanvasGroup cg = assignment.targetUIElement.GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = assignment.targetUIElement.gameObject.AddComponent<CanvasGroup>();
                }

                cg.alpha = 0f;
                StartCoroutine(FadeCanvasGroup(cg, 0f, 1f, 0.3f));

                previouslyShownImages.Add(assignment.targetUIElement);
            }
        }

        StartCoroutine(AutoFadeStepUI());
    }

    IEnumerator AutoFadeStepUI()
    {
        yield return new WaitForSeconds(2.5f);

        if (textCanvasGroup != null)
            StartCoroutine(FadeCanvasGroup(textCanvasGroup, 1f, 0f, 0.5f));

        foreach (var img in previouslyShownImages)
        {
            if (img != null)
            {
                CanvasGroup cg = img.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    StartCoroutine(FadeCanvasGroup(cg, 1f, 0f, 0.5f));
                }
            }
        }
    }

    void SpawnTutorialZombie()
    {
        if (tutorialZombie == null && zombieSpawnPoint != null && zombiePrefab != null)
        {
            tutorialZombie = Instantiate(zombiePrefab, zombieSpawnPoint.position, zombieSpawnPoint.rotation);
            if (tutorialZombie.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.OnDeath += OnTutorialZombieKilled;
            }
        }
    }

    void OnTutorialZombieKilled(GameObject zombie)
    {
        if (zombie.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            enemyHealth.OnDeath -= OnTutorialZombieKilled;
        }

        SoundManager.Instance.PlaySound(tutorialZombieKilledClip, zombie.transform.position);
        zombieKilled = true;
   
    }



    IEnumerator FinalStepDelayAndHide()
    {
        yield return new WaitForSeconds(1f);

        tutorialPanel?.SetActive(false);

        foreach (var img in previouslyShownImages)
        {
            if (img != null)
            {
                img.gameObject.SetActive(false);
            }
        }
        previouslyShownImages.Clear();

        EndTutorial();
    }

    void PlayCutscene()
    {
        if (crosshair != null)
            crosshair.SetActive(false);

        if (ammoCounter != null)
            ammoCounter.SetActive(false);

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);

        if (cutscenePrefab != null && cutsceneSpawnPoint != null)
        {
            Instantiate(cutscenePrefab, cutsceneSpawnPoint.position, cutsceneSpawnPoint.rotation);
        }
    }



    public void ResumeAfterCutscene()
    {
        foreach (var script in movementScriptsToDisable)
        {
            script.enabled = true;
        }

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);

            var cam = mainCamera.GetComponent<Camera>();
            if (cam != null) cam.enabled = true;

            var listener = mainCamera.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = true;
        }

        if (crosshair != null)
            crosshair.SetActive(true);

        if (ammoCounter != null)
            ammoCounter.SetActive(true);
    }



    void EndTutorial()
    {
        IsTutorialActive = false;

        if (roundDisplayText != null)
        {
            roundDisplayText.SetActive(true);
        }

        RoundManager.Instance.tutorialMode = false;
        RoundManager.Instance.StartNewRound();
    }



    IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        cg.alpha = endAlpha;
    }
}
