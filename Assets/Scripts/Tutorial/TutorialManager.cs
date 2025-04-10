using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public Text tutorialText;
    public Image tutorialImage;

    [Header("References")]
    public WaypointManager waypointManager;
    public ZombieSpawner tutorialZombieSpawner;
    public AudioClip tutorialZombieKilledClip;

    public bool IsTutorialActive = true;
    private GameObject tutorialZombie;

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
        ShowTutorialMessage("Welcome to the game! Use WASD and the mouse to move.", null);
    }

    public void ShowTutorialMessage(string message, Sprite imageSprite)
    {
        tutorialPanel?.SetActive(true);
        if (tutorialText != null)
            tutorialText.text = message;
        if (tutorialImage != null)
        {
            tutorialImage.sprite = imageSprite;
            tutorialImage.gameObject.SetActive(imageSprite != null);
        }
    }

    public void OnTutorialWaypointReached(int index)
    {
        switch (index)
        {
            case 1:
                ShowTutorialMessage("Nice! Press 'Space' to jump.", null);
                break;
            case 2:
                ShowTutorialMessage("Try crouching by pressing 'Ctrl'.", null);
                break;
            case 3:
                ShowTutorialMessage("Get ready! A zombie is climbing in!", null);
                SpawnTutorialZombie();
                break;
            default:
                EndTutorial();
                break;
        }
    }

    void SpawnTutorialZombie()
    {
        if (tutorialZombie == null && tutorialZombieSpawner != null)
        {
            tutorialZombie = Instantiate(tutorialZombieSpawner.zombiePrefab, tutorialZombieSpawner.spawnPoint.position, Quaternion.identity);
            var enemyHealth = tutorialZombie.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.OnDeath += OnTutorialZombieKilled;
        }
    }

    void OnTutorialZombieKilled(GameObject zombie)
    {
        var health = zombie.GetComponent<EnemyHealth>();
        if (health != null)
            health.OnDeath -= OnTutorialZombieKilled;

        SoundManager.Instance.PlaySound(tutorialZombieKilledClip, zombie.transform.position);

        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
            ui.FadeInHUD();

        EndTutorial();

        RoundManager.Instance.tutorialMode = false;
        RoundManager.Instance.StartNewRound();
    }

    void EndTutorial()
    {
        IsTutorialActive = false;
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }
}
