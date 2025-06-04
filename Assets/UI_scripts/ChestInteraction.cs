using UnityEngine;
using TMPro;

public class ChestInteraction : MonoBehaviour
{
    public Animator chestAnimator;
    public Animator cubeAnimator;
    public AnimationClip idleClip;

    public Camera playerCam;
    public GameObject chestPromptUI;
    public float lookDistance = 5f;
    public GunCycler gunCycler;

    private bool chestReset = false;
    private bool chestOpened = false;
    private bool gunAvailable = false;
    private TextMeshProUGUI promptText;
    private bool gunPickedUp = false;

    void Start()
    {
        if (chestPromptUI != null)
        {
            promptText = chestPromptUI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    /*
        void Update()
        {
            Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, lookDistance))
            {
                if (hit.collider.CompareTag("Chest"))
                {
                    chestPromptUI.SetActive(true);

                    if (!chestOpened)
                    {
                        if (promptText != null)
                            promptText.text = "Weapon\nPress [E]\n100 Gold";
                    }

                    if (Input.GetKeyDown(KeyCode.E) && !chestOpened)
                    {
                        if (CoinManager.Instance.coinCount >= 100)
                        {
                            CoinManager.Instance.AddCoin(-100);

                            chestAnimator.SetTrigger("OpenChest");
                            cubeAnimator.SetTrigger("OpenChest");

                            AudioSource chestAudio = chestAnimator.GetComponent<AudioSource>();
                            if (chestAudio != null)
                                chestAudio.Play();

                            if (gunCycler == null)
                                gunCycler = hit.collider.GetComponentInChildren<GunCycler>();

                            gunCycler?.StartCycling();
                            chestOpened = true;
                            chestReset = false;
                            Invoke(nameof(MakeGunAvailable), gunCycler.totalCycleTime + 0.5f);
                            Invoke(nameof(ResetChest), 20f);  // Schedule reset
                        }
                        else
                        {
                            Debug.Log("Not enough gold to open the chest.");
                        }
                    }

                    if (chestOpened && gunAvailable && Input.GetKeyDown(KeyCode.F))
                    {
                        GameObject player = GameObject.FindWithTag("Player");
                        var playerController = player.GetComponent<player_controller>();

                        if (gunCycler.finalGunPrefab != null)
                        {
                            playerController.PickupGun(gunCycler.finalGunPrefab);
                            Destroy(gunCycler.currentGunInstance);
                            gunCycler.finalGunPrefab = null;
                            gunPickedUp = true;
                            Debug.Log("Picked up gun from chest.");
                            ResetChest();
                        }
                    }
                    return;
                }
            }

            chestPromptUI?.SetActive(false);
            if (promptText != null)
                promptText.text = "";
        }
        */
    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;
        
        /*if (Physics.Raycast(ray, out hit, lookDistance))
        {
            Debug.Log("RAY HIT: " + hit.collider.gameObject.name + " (root: " + hit.collider.transform.root.name + ")");
        }
        else
        {
            Debug.Log("RAY HIT NOTHING");
        }*/

        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            // === Looking at Chest ===
            if (hit.collider.CompareTag("Chest"))
            {
                chestPromptUI.SetActive(true);

                if (!chestOpened)
                {
                    if (promptText != null)
                        promptText.text = "Weapon\nPress [E]\n100 Gold";
                }

                if (Input.GetKeyDown(KeyCode.E) && !chestOpened)
                {
                    if (CoinManager.Instance.coinCount >= 100)
                    {
                        CoinManager.Instance.AddCoin(-100);

                        chestAnimator.SetTrigger("OpenChest");
                        cubeAnimator.SetTrigger("OpenChest");

                        AudioSource chestAudio = chestAnimator.GetComponent<AudioSource>();
                        if (chestAudio != null)
                            chestAudio.Play();

                        if (gunCycler == null)
                            gunCycler = hit.collider.GetComponentInChildren<GunCycler>();

                        gunCycler?.StartCycling();
                        chestOpened = true;
                        chestReset = false;

                        // Delay when the gun becomes pickable
                        Invoke(nameof(MakeGunAvailable), gunCycler.totalCycleTime + 0.5f);
                        Invoke(nameof(ResetChest), 20f); // Reset chest after timeout
                    }
                    else
                    {
                        Debug.Log("Not enough gold to open the chest.");
                    }
                }

                return; // Exit early so it doesn't fall through
            }

            // === Looking at Dropped Weapon ===
            if (chestOpened && gunAvailable && gunCycler != null && gunCycler.currentGunInstance != null)
            {
                if (hit.collider.gameObject == gunCycler.currentGunInstance)
                {
                    if (promptText != null)
                    {
                        promptText.text = "Pick up weapon\nPress [F]";
                        chestPromptUI.SetActive(true);
                    }

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        GameObject player = GameObject.FindWithTag("Player");
                        var playerController = player.GetComponent<player_controller>();

                        if (gunCycler.finalGunPrefab != null)
                        {
                            playerController.PickupGun(gunCycler.finalGunPrefab);
                            Destroy(gunCycler.currentGunInstance);
                            gunCycler.finalGunPrefab = null;
                            Debug.Log("Picked up weapon from chest.");
                            ResetChest();
                        }
                    }

                    return;
                }
            }
        }

        // If not looking at anything relevant
        chestPromptUI?.SetActive(false);
        if (promptText != null)
            promptText.text = "";
    }


    void MakeGunAvailable()
    {
        gunAvailable = true;

        if (promptText != null)
            promptText.text = "Equip Weapon\nPress [F]";
    }

    /*    void ResetChest()
        {
            if (chestReset) return;
            chestReset = true;
            Debug.Log("Resetting chest now.");

            // Rewind to the idle/closed state
            if (idleClip != null)
                chestAnimator.Play(idleClip.name, 0, 0f);
            else
                Debug.LogWarning("Idle animation clip not assigned!");
            cubeAnimator.Play("Default", 0, 0f);

            chestOpened = false;
            gunAvailable = false;

            if (promptText != null)
                promptText.text = "";

            if (gunCycler != null)
            {
                Destroy(gunCycler.currentGunInstance);
                gunCycler.finalGunPrefab = null;
            }
        }*/
        void ResetChest()
    {
        if (chestReset) return;
        chestReset = true;

        if (!gunPickedUp)
            Debug.Log("Resetting chest after timeout without pickup.");
        else
            Debug.Log("Resetting chest after gun pickup.");

        if (idleClip != null)
            chestAnimator.Play(idleClip.name, 0, 0f);
        else
            Debug.LogWarning("Idle animation clip not assigned!");

        cubeAnimator.Play("Default", 0, 0f);

        chestOpened = false;
        gunAvailable = false;
        gunPickedUp = false;

        if (promptText != null)
            promptText.text = "";

        if (gunCycler != null)
        {
            Destroy(gunCycler.currentGunInstance);
            gunCycler.finalGunPrefab = null;
        }
    }
}
