using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelectionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject cardPanel;
    public TextMeshProUGUI[] cardTitles;
    public TextMeshProUGUI[] cardDescriptions;
    public Button[] cardButtons;

    private bool isActive = false;

    private void Awake()
    {
        // Hide panel by default
        cardPanel.SetActive(false);

        // Setup button listeners
        cardButtons[1].onClick.AddListener(OnHealthCardSelected);
    }

    public void ShowCards()
    {
        if (isActive) return;

        isActive = true;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Setup health upgrade card (middle card)
        var healthUpgrade = UpgradeManager.Instance.GetCurrentHealthUpgrade();
        if (healthUpgrade != null)
        {
            cardTitles[1].text = healthUpgrade.upgradeName;
            cardDescriptions[1].text = healthUpgrade.description;
            cardButtons[1].interactable = true;
        }

        // Disable other cards for now
        cardTitles[0].text = "Coming Soon";
        cardDescriptions[0].text = "Upgrade in development";
        cardButtons[0].interactable = false;

        cardTitles[2].text = "Coming Soon";
        cardDescriptions[2].text = "Upgrade in development";
        cardButtons[2].interactable = false;

        // Show panel
        cardPanel.SetActive(true);
    }

    public void OnHealthCardSelected()
    {
        if (!isActive) return;

        // Apply upgrade
        UpgradeManager.Instance.ApplyHealthUpgrade();

        // Hide UI
        HideCards();

        // Notify RoundManager to resume
        RoundManager.Instance.ResumeAfterCardSelection();
    }

    private void HideCards()
    {
        isActive = false;
        cardPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}