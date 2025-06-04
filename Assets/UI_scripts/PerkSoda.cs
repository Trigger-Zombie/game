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

    [Header("Camera Shake")]
    public float shakeDuration = 6f;
    public float shakeIntensity = 0.1f;
    private bool isShaking = false;
    private float shakeTimer = 0f;
    private Vector3 originalCamPos;

    private bool perkGiven = false;

    void Start()
    {
        if (playerCam == null)
        {
            playerCam = Camera.main;
            Debug.Log("PerkSoda: Assigned PlayerCam via Camera.main");
        }

        if (playerCam != null)
        {
            originalCamPos = playerCam.transform.localPosition;
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
        if (!perkGiven && playerCam != null)
        {
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

                        if (audioSource != null && drinkSound != null)
                        {
                            audioSource.PlayOneShot(drinkSound);
                        }

                        // 🚀 Start one-time camera shake
                        if (playerCam != null)
                        {
                            isShaking = true;
                            shakeTimer = shakeDuration;
                            originalCamPos = playerCam.transform.localPosition;
                        }
                    }

                    return;
                }
            }

            perkPromptUI?.SetActive(false);
        }

        // 🔁 Apply shake if active
        if (isShaking && playerCam != null)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer > 0f)
            {
                float shakeOffsetX = Random.Range(-1f, 1f) * shakeIntensity;
                float shakeOffsetY = Random.Range(-1f, 1f) * shakeIntensity;
                Vector3 shakeOffset = new Vector3(shakeOffsetX, shakeOffsetY, 0f);
                playerCam.transform.localPosition = originalCamPos + shakeOffset;
            }
            else
            {
                isShaking = false;
                playerCam.transform.localPosition = originalCamPos;
            }
        }
    }
}
