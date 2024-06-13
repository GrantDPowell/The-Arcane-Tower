using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardItemForLoadout : MonoBehaviour
{
    public TMP_Text cardNameText;
    public TMP_Text cardCostText;
    public Button refundButton;

    private SpellCard spellCard;
    private PlayerCard playerCard;
    private LoadoutUIManager loadoutUIManager;

    public void Initialize(SpellCard card, string cardType)
    {
        spellCard = card;
        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();
        refundButton.onClick.AddListener(() => RefundCard(cardType));
        loadoutUIManager = FindObjectOfType<LoadoutUIManager>();
    }

    public void Initialize(PlayerCard card, string cardType)
    {
        playerCard = card;
        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();
        refundButton.onClick.AddListener(() => RefundCard(cardType));
        loadoutUIManager = FindObjectOfType<LoadoutUIManager>();
    }

    private void RefundCard(string cardType)
    {
        if (cardType == "Spell")
        {
            loadoutUIManager.RefundSpellCard(spellCard);
        }
        else if (cardType == "Player")
        {
            loadoutUIManager.RefundPlayerCard(playerCard);
        }
    }
}
