using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CardSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cardPanel;
    public Button leftCardButton;
    public Button middleCardButton;
    public Button rightCardButton;
    public TextMeshProUGUI leftCardTitle;
    public TextMeshProUGUI middleCardTitle;
    public TextMeshProUGUI rightCardTitle;
    public TextMeshProUGUI leftCardDescription;
    public TextMeshProUGUI middleCardDescription;
    public TextMeshProUGUI rightCardDescription;

    [Header("Settings")]
    public float clickDelay = 0.5f;

    private bool cardsClickable = false;
    private Coroutine enableClickCoroutine;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput not found in scene!");
        }
    }

    private void Start()
    {
        if (!VerifyComponents())
        {
            Debug.LogError("CardSelectionUI components not properly set up!");
            enabled = false;
            return;
        }

        leftCardButton.onClick.AddListener(() => OnCardSelected(0));
        middleCardButton.onClick.AddListener(() => OnCardSelected(1));
        rightCardButton.onClick.AddListener(() => OnCardSelected(2));

        HideAllCards();
    }

    public void ShowRandomUpgrade()
    {
        Debug.Log("[CardUI] Starting ShowRandomUpgrade");

        // Reset state
        cardsClickable = false;
        if (enableClickCoroutine != null)
        {
            StopCoroutine(enableClickCoroutine);
        }

        // Setup UI
        SetupCardContents();

        // Show UI
        cardPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player input
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        // Start click delay
        enableClickCoroutine = StartCoroutine(EnableCardClicking());
        Debug.Log("[CardUI] Started click delay coroutine");
    }

    private void SetupCardContents()
    {
        // Left Card - Weapon Upgrade
        var weaponUpgrade = UpgradeManager.Instance?.GetRandomWeaponUpgrade();
        if (weaponUpgrade != null)
        {
            leftCardTitle.text = weaponUpgrade.upgradeName;
            leftCardDescription.text = weaponUpgrade.description;
            leftCardButton.gameObject.SetActive(true);
        }

        // Middle Card - Health Upgrade
        var healthUpgrade = UpgradeManager.Instance?.GetRandomHealthUpgrade();
        if (healthUpgrade != null)
        {
            middleCardTitle.text = healthUpgrade.upgradeName;
            middleCardDescription.text = healthUpgrade.description;
            middleCardButton.gameObject.SetActive(true);
        }

        // Right Card - Movement Upgrade
        var movementUpgrade = UpgradeManager.Instance?.GetRandomMovementUpgrade();
        if (movementUpgrade != null)
        {
            rightCardTitle.text = movementUpgrade.upgradeName;
            rightCardDescription.text = movementUpgrade.description;
            rightCardButton.gameObject.SetActive(true);
        }
    }

    private IEnumerator EnableCardClicking()
    {
        Debug.Log("[CardUI] Starting click delay timer");
        yield return new WaitForSecondsRealtime(clickDelay); // Using realtime to avoid pause issues

        cardsClickable = true;
        Debug.Log("[CardUI] Cards are now clickable");

        // Visual feedback
        leftCardButton.interactable = true;
        middleCardButton.interactable = true;
        rightCardButton.interactable = true;
    }

    private void OnCardSelected(int cardIndex)
    {
        if (!cardsClickable)
        {
            Debug.Log($"[CardUI] Card {cardIndex} clicked too early!");
            return;
        }

        Debug.Log($"[CardUI] Card {cardIndex} selected");

        switch (cardIndex)
        {
            case 0: // Weapon Upgrade
                ApplyWeaponUpgrade();
                break;
            case 1: // Health Upgrade
                ApplyHealthUpgrade();
                break;
            case 2: // Movement Upgrade
                ApplyMovementUpgrade();
                break;
        }

        CloseCardMenu();
    }

    private void ApplyWeaponUpgrade()
    {
        var upgrade = UpgradeManager.Instance?.GetRandomWeaponUpgrade();
        if (upgrade != null)
        {
            UpgradeManager.Instance?.ApplyWeaponUpgrade(upgrade);
            Debug.Log($"[CardUI] Applied weapon upgrade: {upgrade.upgradeName}");
        }
    }

    private void ApplyHealthUpgrade()
    {
        var upgrade = UpgradeManager.Instance?.GetRandomHealthUpgrade();
        if (upgrade != null)
        {
            UpgradeManager.Instance?.ApplyHealthUpgrade(upgrade);
            Debug.Log($"[CardUI] Applied health upgrade: {upgrade.upgradeName}");
        }
    }

    private void ApplyMovementUpgrade()
    {
        var upgrade = UpgradeManager.Instance?.GetRandomMovementUpgrade();
        if (upgrade != null)
        {
            UpgradeManager.Instance?.ApplyMovementUpgrade(upgrade);
            Debug.Log($"[CardUI] Applied movement upgrade: {upgrade.upgradeName}");
        }
    }

    private void CloseCardMenu()
    {
        Debug.Log("[CardUI] Closing card menu");

        // Re-enable player input
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        HideAllCards();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Notify RoundManager
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.ResumeAfterCardSelection();
        }
        else
        {
            Debug.LogError("[CardUI] RoundManager instance not found!");
        }
    }

    private void HideAllCards()
    {
        cardPanel.SetActive(false);
        leftCardButton.gameObject.SetActive(false);
        middleCardButton.gameObject.SetActive(false);
        rightCardButton.gameObject.SetActive(false);
        cardsClickable = false;

        if (enableClickCoroutine != null)
        {
            StopCoroutine(enableClickCoroutine);
            enableClickCoroutine = null;
        }
    }

    private bool VerifyComponents()
    {
        bool isValid = true;

        if (cardPanel == null) { Debug.LogError("Card Panel reference missing!"); isValid = false; }
        if (leftCardButton == null) { Debug.LogError("Left Card Button missing!"); isValid = false; }
        if (middleCardButton == null) { Debug.LogError("Middle Card Button missing!"); isValid = false; }
        if (rightCardButton == null) { Debug.LogError("Right Card Button missing!"); isValid = false; }
        if (leftCardTitle == null) { Debug.LogError("Left Card Title missing!"); isValid = false; }
        if (middleCardTitle == null) { Debug.LogError("Middle Card Title missing!"); isValid = false; }
        if (rightCardTitle == null) { Debug.LogError("Right Card Title missing!"); isValid = false; }
        if (leftCardDescription == null) { Debug.LogError("Left Card Description missing!"); isValid = false; }
        if (middleCardDescription == null) { Debug.LogError("Middle Card Description missing!"); isValid = false; }
        if (rightCardDescription == null) { Debug.LogError("Right Card Description missing!"); isValid = false; }

        return isValid;
    }
}