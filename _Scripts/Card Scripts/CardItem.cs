using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public TMP_Text cardNameText;
    public TMP_Text cardCostText;
    public Button buyButton;

    private SpellCard spellCard;
    private PlayerCard playerCard;
    private ShopUIManager shopUIManager;

    public void Initialize(SpellCard card, string cardType)
    {
        spellCard = card;
        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();
        buyButton.onClick.AddListener(() => BuyCard(cardType));
        shopUIManager = FindObjectOfType<ShopUIManager>();
    }

    public void Initialize(PlayerCard card, string cardType)
    {
        playerCard = card;
        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();
        buyButton.onClick.AddListener(() => BuyCard(cardType));
        shopUIManager = FindObjectOfType<ShopUIManager>();
    }

    private void BuyCard(string cardType)
    {
        if (cardType == "Spell")
        {
            shopUIManager.BuySpellCard(spellCard);
        }
        else if (cardType == "Player")
        {
            shopUIManager.BuyPlayerCard(playerCard);
        }
    }
}
