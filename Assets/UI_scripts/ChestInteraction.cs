using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public Animator chestAnimator;
    public Animator cubeAnimator;
    public Camera playerCam;
    public GameObject chestPromptUI;
    public float lookDistance = 5f;

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
                    chestAnimator.SetTrigger("OpenChest");
                    cubeAnimator.SetTrigger("OpenChest");
                }

                return;
            }
        }

        chestPromptUI.SetActive(false);
    }
}
