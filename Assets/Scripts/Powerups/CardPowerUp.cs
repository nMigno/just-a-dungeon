using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CardType
{
    Mage,
    Chariot,
    Wheel,
    Star,
}

[RequireComponent(typeof(Button))]
public class CardPowerUp : MonoBehaviour
{
    private CardType powerUpType;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;
    [SerializeField] private Image cardIcon;

    [Header("Cards Sprites")]
    [SerializeField] private Sprite mageSprite;
    [SerializeField] private Sprite chariotSprite;
    [SerializeField] private Sprite wheelSprite;
    [SerializeField] private Sprite starSprite;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectCard);
    }

    void SelectCard()
    {
        LevelManager.Instance.CardSelect(powerUpType);
    }

    public void SetupCard(CardType newType)
    {
        powerUpType = newType;

        switch (powerUpType)
        {
            case CardType.Mage:
                cardNameText.text = "The Magician";
                cardDescriptionText.text = "+10% Attack speed";
                cardIcon.sprite = mageSprite;
                break;
            case CardType.Chariot:
                cardNameText.text = "The Chariot";
                cardDescriptionText.text = "+10% Move speed";
                cardIcon.sprite = chariotSprite;
                break;
            case CardType.Wheel:
                cardNameText.text = "Wheel of Fortune";
                cardDescriptionText.text = "-10% Dash cooldown";
                cardIcon.sprite = wheelSprite;
                break;
            case CardType.Star:
                cardNameText.text = "The Star";
                cardDescriptionText.text = "+10% Spell size";
                cardIcon.sprite = starSprite;
                break;
        }
    }
}
