using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int coinCount = 0;
    public TextMeshProUGUI coinText;

    void Start()
    {
        Debug.Log("CoinManager starting with coinCount = " + coinCount);
        UpdateCoinText();
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        Debug.Log("AddCoin called. New coin count = " + coinCount);
        UpdateCoinText();
    }

    void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Gold: " + coinCount.ToString();
            coinText.ForceMeshUpdate();  // <-- forces visual refresh
            Debug.Log("UI Updated: Gold: " + coinCount);
        }
    }
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.G))  // press G to simulate a coin pickup
    {
        AddCoin(1);
    }
}
}

