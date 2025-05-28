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

    void Start()
    {
        if (chestPromptUI != null)
        {
            promptText = chestPromptUI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

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
                        Invoke(nameof(ResetChest), 20f);  // 👈 Schedule reset
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
                        Debug.Log("Picked up gun from chest.");
                        ResetChest();  // 🧹 Reset immediately after pickup
                    }
                }

                return;
            }
        }

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

    void ResetChest()
    {
        if (chestReset) return;
        chestReset = true;
        Debug.Log("🔁 Resetting chest now.");

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
    }
}
