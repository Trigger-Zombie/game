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
                CoinManager.Instance.AddCoin(149);
            }
            else
            {
                Debug.LogWarning("CoinManager reference is missing!");
            }

            if (objectiveUI != null)
            {
                objectiveUI.SetActive(false);
            }

            // Immediately disable the collider and visuals
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                rend.enabled = false;
            }

            if (destroyAudioSource != null && destroyAudioSource.clip != null)
            {
                destroyAudioSource.Play();
                Destroy(gameObject, destroyAudioSource.clip.length); // Delay actual destruction
            }
            else
            {
                Destroy(gameObject); // No sound, destroy instantly
            }
        }
    }
}
