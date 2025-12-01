using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class CardScreenController : MonoBehaviour
{
    [SerializeField] private GameObject cardScreenPanel;
    [SerializeField] private CardPowerUp cardRender1;
    [SerializeField] private CardPowerUp cardRender2;
    [SerializeField] private TextMeshProUGUI titleText;

    public void ShowCardSelection()
    {
        Time.timeScale = 0f;
        cardScreenPanel.SetActive(true);

        GameManager.Instance.ActivateActionMap(GameInputMap.UI);

        List<CardType> allCards = System.Enum.GetValues(typeof(CardType))
            .Cast<CardType>()
            .ToList();

        System.Random rng = new();
        List<CardType> shuffledCards = allCards.OrderBy(c => rng.Next()).ToList();

        cardRender1.gameObject.SetActive(true);
        cardRender1.SetupCard(shuffledCards[0]);

        cardRender2.gameObject.SetActive(true);
        cardRender2.SetupCard(shuffledCards[1]);
    }

    public void HideCardSelection()
    {
        cardScreenPanel.SetActive(false);
    }
}
