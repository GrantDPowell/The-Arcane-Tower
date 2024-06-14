using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpCardItem : MonoBehaviour
{
    public TMP_Text cardNameText;
    public TMP_Text cardDescription; // Optional: If you want to show the card cost
    public Button selectButton;

    public Image cardImage;

    private SpellCard spellCard;
    private PlayerCard playerCard;
    private LevelUpUI levelUpUI;

    public void Initialize(SpellCard card, LevelUpUI levelUpUI)
    {
        spellCard = card;
        cardNameText.text = card.cardName;
        cardDescription.text = card.description.ToString(); // Optional
        cardImage.sprite = card.cardSprite;
        selectButton.onClick.AddListener(() => SelectCard());
        this.levelUpUI = levelUpUI;
    }

    public void Initialize(PlayerCard card, LevelUpUI levelUpUI)
    {
        playerCard = card;
        cardNameText.text = card.cardName;
        cardDescription.text = card.description.ToString(); // Optional
        cardImage.sprite = card.cardSprite;
        selectButton.onClick.AddListener(() => SelectCard());
        this.levelUpUI = levelUpUI;
    }

    private void SelectCard()
    {
        if (spellCard != null)
        {
            levelUpUI.ApplyCard(spellCard);
        }
        else if (playerCard != null)
        {
            levelUpUI.ApplyCard(playerCard);
        }
    }
}
