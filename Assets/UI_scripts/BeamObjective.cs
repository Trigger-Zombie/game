using UnityEngine;

public class BeamObjective : MonoBehaviour
{
    public AudioSource spawnAudioSource;
    public AudioSource destroyAudioSource;
    public GameObject objectiveUI;
    void Start()
    {
        // Play the sound when the beam spawns
        if (spawnAudioSource != null && spawnAudioSource.clip != null)
        {
            spawnAudioSource.Play();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Beam reached!");

            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin(99);
            }
            else
            {
                Debug.LogWarning("CoinManager reference is missing!");
            }
            if (objectiveUI != null)
            {
                objectiveUI.SetActive(false);
            }
            if (destroyAudioSource != null && destroyAudioSource.clip != null)
            {
                destroyAudioSource.Play();
                Destroy(gameObject, destroyAudioSource.clip.length); // wait for sound
            }
            else
            {

            Destroy(gameObject); // Remove the beam when collected
            }
        }
    }
}
