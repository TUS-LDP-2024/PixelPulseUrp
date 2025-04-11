using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialImageAssignment
{
    public Image targetUIElement; // UI Image on canvas
    public Sprite spriteToShow;   // Sprite to assign
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
    public Text tutorialText;
    public CanvasGroup textCanvasGroup;

    [Header("Step System")]
    public List<TutorialStep> steps;

    [Header("Zombie")]
    public GameObject zombiePrefab;
    public Transform zombieSpawnPoint;
    private GameObject tutorialZombie;

    [Header("Cutscene")]
    public GameObject cutscenePrefab;
    public Transform cutsceneSpawnPoint;
    public GameObject player;
    public MonoBehaviour[] movementScriptsToDisable;
    public Camera mainCamera;

    public AudioClip tutorialZombieKilledClip;

    public bool IsTutorialActive = true;

    private List<Image> previouslyShownImages = new List<Image>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

        if (index == 4)
        {
            StartCoroutine(FinalStepDelayAndHide());
        }
    }

    void ShowTutorialStep(int index)
    {
        if (index >= steps.Count) return;

        var step = steps[index];

        // Fully hide all previously shown image objects
        foreach (var img in previouslyShownImages)
        {
            if (img != null)
            {
                img.gameObject.SetActive(false); // Disable GameObject
                var cg = img.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0f;
            }
        }
        previouslyShownImages.Clear();

        tutorialPanel?.SetActive(true);

        // Show and fade in text
        if (tutorialText != null && textCanvasGroup != null)
        {
            tutorialText.text = step.message;
            StartCoroutine(FadeCanvasGroup(textCanvasGroup, 0f, 1f, 0.5f));
        }

        // Show and fade in assigned images
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
                StartCoroutine(FadeCanvasGroup(cg, 0f, 1f, 0.5f));

                previouslyShownImages.Add(assignment.targetUIElement);
            }
        }
    }

    IEnumerator FinalStepDelayAndHide()
    {
        yield return new WaitForSeconds(5f);

        if (textCanvasGroup != null)
            StartCoroutine(FadeCanvasGroup(textCanvasGroup, 1f, 0f, 1f));

        foreach (var img in previouslyShownImages)
        {
            CanvasGroup cg = img.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                StartCoroutine(FadeCanvasGroup(cg, 1f, 0f, 1f));
            }
        }

        yield return new WaitForSeconds(1.2f);

        // Fully deactivate them
        foreach (var img in previouslyShownImages)
        {
            if (img != null)
            {
                img.gameObject.SetActive(false);
            }
        }
        previouslyShownImages.Clear();

        tutorialPanel?.SetActive(false);

        EndTutorial();
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

        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.FadeInTutorialHUD();
        }

        EndTutorial();
    }

    void PlayCutscene()
    {
        foreach (var script in movementScriptsToDisable)
        {
            script.enabled = false;
        }

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);

        if (cutscenePrefab != null && cutsceneSpawnPoint != null)
            Instantiate(cutscenePrefab, cutsceneSpawnPoint.position, cutsceneSpawnPoint.rotation);
    }

    public void ResumeAfterCutscene()
    {
        foreach (var script in movementScriptsToDisable)
        {
            script.enabled = true;
        }

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);
    }

    void EndTutorial()
    {
        IsTutorialActive = false;

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
