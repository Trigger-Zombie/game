using UnityEngine;

public class PerkInteraction : MonoBehaviour
{
    public Camera playerCam;
    public GameObject perkPromptUI; // "Press [E] 50 Gold" UI
    public float lookDistance = 5f;
    public int perkCost = 100;
    public SodaCycler sodaCycler; // Auto-assigned

    [Header("Audio")]
    public AudioSource vendingAudioSource;
    public AudioClip cycleStartSound;

    void Start()
    {
        if (sodaCycler == null)
        {
            sodaCycler = GetComponent<SodaCycler>();
            if (sodaCycler != null)
                Debug.Log("✅ SodaCycler auto-assigned in Start()");
            else
                Debug.LogError("❌ SodaCycler still NULL in Start()! Check object setup.");
        }
    }

    void Update()
    {
        if (playerCam == null)
        {
            Debug.LogError("❌ PlayerCam not assigned!");
            return;
        }

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            if (hit.collider.CompareTag("Perk"))
            {
                perkPromptUI?.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (CoinManager.Instance.coinCount >= perkCost)
                    {
                        CoinManager.Instance.AddCoin(-perkCost);

                        // 🔊 Play sound
                        if (vendingAudioSource != null && cycleStartSound != null)
                        {
                            vendingAudioSource.Stop();
                            vendingAudioSource.PlayOneShot(cycleStartSound);
                            Debug.Log("▶️ Played vending machine sound.");
                        }

                        if (sodaCycler != null)
                        {
                            sodaCycler.StartSodaCycle();
                            Debug.Log("🧃 Soda cycle started!");
                        }
                        else
                        {
                            Debug.LogError("❌ sodaCycler was NULL when trying to start cycle!");
                        }
                    }
                    else
                    {
                        Debug.Log("Not enough gold for perk.");
                    }
                }

                return;
            }
        }

        perkPromptUI?.SetActive(false);
    }
}
