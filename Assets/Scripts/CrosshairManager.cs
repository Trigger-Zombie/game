using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public GameObject pistolCrosshair;
    public GameObject rifleCrosshair;
    public GameObject shotgunCrosshair;

    public void ShowCrosshair(string weaponType)
    {
        pistolCrosshair.SetActive(false);
        rifleCrosshair.SetActive(false);
        shotgunCrosshair.SetActive(false);

        switch (weaponType)
        {
            case "Pistol":
                pistolCrosshair.SetActive(true);
                break;
            case "Rifle":
                rifleCrosshair.SetActive(true);
                break;
            case "Shotgun":
                shotgunCrosshair.SetActive(true);
                break;
        }
    }
}
