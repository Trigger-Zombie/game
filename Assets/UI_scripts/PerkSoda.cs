using UnityEngine;

public class PerkSoda : MonoBehaviour
{
    public static bool sodaSpawned = false;
    public Camera playerCam;
    public GameObject perkPromptUI;
    public float lookDistance = 3f;
    public TimeManager timeManager;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip drinkSound;

    private bool perkGiven = false;

    void Start()
    {
        if (playerCam == null)
        {
            playerCam = Camera.main;
            Debug.Log("PerkSoda: Assigned PlayerCam via Camera.main");
        }

        if (perkPromptUI == null)
        {
            GameObject uiObj = GameObject.Find("Soda UI");
            if (uiObj != null)
            {
                perkPromptUI = uiObj;
                Debug.Log("PerkSoda: Found Soda UI");
            }
        }

        if (timeManager == null)
        {
            timeManager = FindAnyObjectByType<TimeManager>();
            Debug.Log("PerkSoda: Found TimeManager in scene");
        }

        // ✅ Auto-assign the audio source from the Player if not already set
        if (audioSource == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                audioSource = playerObj.GetComponent<AudioSource>();
                if (audioSource != null)
                    Debug.Log("PerkSoda: Found AudioSource on Player");
                else
                    Debug.LogWarning("PerkSoda: Player found but no AudioSource attached.");
            }
            else
            {
                Debug.LogWarning("PerkSoda: No GameObject with tag 'Player' found.");
            }
        }
    }


    void Update()
    {
        if (perkGiven || playerCam == null) return;

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Soda"))
            {
                perkPromptUI?.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Perk granted from soda!");
                    perkGiven = true;
                    timeManager.perkUnlocked = true;
                    perkPromptUI?.SetActive(false);

                    Destroy(hit.collider.gameObject); // 💥 Destroy soda

                    // 🔊 Play drink sound from player's AudioSource
                    if (audioSource != null && drinkSound != null)
                    {
                        audioSource.PlayOneShot(drinkSound);
                    }
                    else
                    {
                        Debug.LogWarning("Drink sound or AudioSource not set or found.");
                    }
                }

                return;
            }
        }

        perkPromptUI?.SetActive(false);
    }
}
