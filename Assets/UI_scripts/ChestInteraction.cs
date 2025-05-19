using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public Animator chestAnimator;
    public Animator cubeAnimator;
    public Camera playerCam;
    public GameObject chestPromptUI;
    public float lookDistance = 5f;
    public GunCycler gunCycler;

    private bool chestOpened = false;
    private bool gunAvailable = false;

    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            if (hit.collider.CompareTag("Chest"))
            {
                chestPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Check if the player has at least 100 coins
                    if (CoinManager.Instance.coinCount >= 100)
                    {
                        // Deduct 100 coins and play animations
                        CoinManager.Instance.AddCoin(-100);

                        chestAnimator.SetTrigger("OpenChest");
                        cubeAnimator.SetTrigger("OpenChest");
                        AudioSource chestAudio = chestAnimator.GetComponent<AudioSource>();
                        if (chestAudio != null)
                        {
                            chestAudio.Play();
                        }
                        //GunCycler gunCycler = hit.collider.GetComponentInChildren<GunCycler>();
                        if (gunCycler == null)
                            {
                                gunCycler = hit.collider.GetComponentInChildren<GunCycler>();
                            }
                        gunCycler?.StartCycling(); // safe call if not null
                        chestOpened = true;
                        Invoke(nameof(MakeGunAvailable), gunCycler.totalCycleTime + 0.5f);
                    }
                    else
                    {
                        Debug.Log("Not enough gold to open the chest.");
                        // Optional: Show a "not enough gold" message on UI
                    }
                }

                if (chestOpened && Input.GetKeyDown(KeyCode.F))
                {
                    GameObject player = GameObject.FindWithTag("Player");
                    var playerController = player.GetComponent<player_controller>();

                    if (gunCycler.finalGunPrefab != null)
                    {
                        playerController.PickupGun(gunCycler.finalGunPrefab);
                        Destroy(gunCycler.currentGunInstance); // remove the preview gun
                        gunCycler.finalGunPrefab = null; // clear it so you don't pick it up again
                        Debug.Log("Picked up gun from chest.");
                    }
                }
                return;
            }
        }
        chestPromptUI.SetActive(false);
    }
    void MakeGunAvailable()
    {
        gunAvailable = true;
    }
}