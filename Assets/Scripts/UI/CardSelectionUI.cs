using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cardPanel;

    // Left Card
    public GameObject leftCard;
    public TextMeshProUGUI leftCardTitle;
    public TextMeshProUGUI leftCardDescription;
    public Button leftCardButton;
    public Image leftCardIcon;

    // Middle Card (Health Upgrades)
    public GameObject middleCard;
    public TextMeshProUGUI middleCardTitle;
    public TextMeshProUGUI middleCardDescription;
    public Button middleCardButton;
    public Image middleCardIcon;

    // Right Card
    public GameObject rightCard;
    public TextMeshProUGUI rightCardTitle;
    public TextMeshProUGUI rightCardDescription;
    public Button rightCardButton;
    public Image rightCardIcon;

    private void Start()
    {
        // Initialize buttons
        leftCardButton.onClick.AddListener(() => OnCardClicked());
        middleCardButton.onClick.AddListener(() => OnCardClicked());
        rightCardButton.onClick.AddListener(() => OnCardClicked());

        HideAllCards();
    }

    public void ShowRandomUpgrade()
    {
        if (!VerifyComponents()) return;

        // Set up left card (empty or different type for now)
        leftCardTitle.text = "Coming Soon";
        leftCardDescription.text = "Additional upgrades in future updates!";
        leftCardButton.onClick.RemoveAllListeners();
        leftCardButton.onClick.AddListener(() => OnCardClicked());

        // Set up middle card (health upgrades only)
        var healthUpgrade = UpgradeManager.Instance?.GetRandomHealthUpgrade();
        if (healthUpgrade != null)
        {
            middleCardTitle.text = healthUpgrade.upgradeName;
            middleCardDescription.text = healthUpgrade.description;
            middleCardButton.onClick.RemoveAllListeners();
            middleCardButton.onClick.AddListener(() => OnCardClicked(healthUpgrade));
        }

        // Set up right card (empty or different type for now)
        rightCardTitle.text = "Coming Soon";
        rightCardDescription.text = "Additional upgrades in future updates!";
        rightCardButton.onClick.RemoveAllListeners();
        rightCardButton.onClick.AddListener(() => OnCardClicked());

        // Show UI
        cardPanel.SetActive(true);
        leftCard.SetActive(true);
        middleCard.SetActive(true);
        rightCard.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnCardClicked(UpgradeManager.UpgradeEffect upgrade = null)
    {
        if (upgrade != null)
        {
            UpgradeManager.Instance?.ApplyUpgrade(upgrade);
        }

        HideAllCards();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        RoundManager.Instance?.ResumeAfterCardSelection();
    }

    private void HideAllCards()
    {
        cardPanel.SetActive(false);
        leftCard.SetActive(false);
        middleCard.SetActive(false);
        rightCard.SetActive(false);
    }

    private bool VerifyComponents()
    {
        bool allValid = true;

        if (leftCardTitle == null) { Debug.LogError("Left Card Title missing!"); allValid = false; }
        if (middleCardTitle == null) { Debug.LogError("Middle Card Title missing!"); allValid = false; }
        if (rightCardTitle == null) { Debug.LogError("Right Card Title missing!"); allValid = false; }

        return allValid && cardPanel != null && leftCard != null && middleCard != null && rightCard != null;
    }
}