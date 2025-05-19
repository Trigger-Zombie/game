using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public int coinCount = 0;
    public TextMeshProUGUI coinText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
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
}

