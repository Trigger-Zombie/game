using UnityEngine;

public class PerkInteraction : MonoBehaviour
{
    public Camera playerCam;
    public GameObject perkPromptUI; // "Press [E] 50 Gold" UI
    public float lookDistance = 5f;
    public int perkCost = 50;
    public TimeManager timeManager;


    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            if (hit.collider.CompareTag("Perk"))
            {
                perkPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (CoinManager.Instance.coinCount >= perkCost)
                    {
                        CoinManager.Instance.AddCoin(-perkCost);
                        timeManager.perkUnlocked = true;
                    }
                    else
                    {
                        Debug.Log("Not enough gold for perk.");
                    }
                }

                return;
            }
        }
        perkPromptUI.SetActive(false);
    }
}
