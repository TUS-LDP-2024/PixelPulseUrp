using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CardUI
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image iconImage;

    public void Setup(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
    }
}

public class CardSelectionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject cardPanel;
    public CardUI[] cards;
    public Button[] cardButtons;

    private void Awake()
    {
        cardPanel.SetActive(false);

        for (int i = 0; i < cardButtons.Length; i++)
        {
            int index = i;
            cardButtons[i].onClick.AddListener(() => SelectCard(index));
        }
    }

    public void ShowCards()
    {
        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Setup cards
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].Setup($"Upgrade {i + 1}", $"Bonus effect {i + 1}");
        }

        cardPanel.SetActive(true);
    }

    private void SelectCard(int cardIndex)
    {
        Debug.Log($"Selected card {cardIndex + 1}");

        // Hide panel
        cardPanel.SetActive(false);

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Unpause the game
        Time.timeScale = 1f;

        // Notify RoundManager
        RoundManager.Instance.UpgradeSelected();
    }
}