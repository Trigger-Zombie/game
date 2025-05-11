using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoText;

    public void UpdateAmmo(int current, int total)
    {
        ammoText.text = $"{current} / {total}";
    }
}