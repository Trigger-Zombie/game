using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    public TextMeshProUGUI waveText;

    public void UpdateWave(int waveNumber)
    {
        waveText.text = $"Wave: {waveNumber}";
    }
}
