using UnityEngine;

public class SpinMeds : MonoBehaviour
{
    public AudioSource healSound;
    public float rotationSpeed = 50f;
    public int healAmount = 100;
    public float respawnTime = 10f;
    public GameObject visual;
    public GameObject pharmacyLogo;

    private Collider pickupCollider;

    void Start()
    {
        pickupCollider = GetComponent<Collider>();
        if (visual != null) visual.SetActive(true);
    }

    void Update()
    {
        if (pharmacyLogo != null)
        {
            pharmacyLogo.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Make sure we're getting the root object that has the controller script
            player_controller player = other.GetComponentInParent<player_controller>();
            if (player != null)
            {
                player.TakeDamage(-healAmount); // Heal
            }
            if (healSound != null)
            {
                healSound.Play();
            }
            StartCoroutine(Respawn());
        }
    }
    System.Collections.IEnumerator Respawn()
    {
        if (visual != null) visual.SetActive(false);
        if (pharmacyLogo != null) pharmacyLogo.SetActive(false);
        if (pickupCollider != null) pickupCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        if (visual != null) visual.SetActive(true);
        if (pharmacyLogo != null) pharmacyLogo.SetActive(true);
        if (pickupCollider != null) pickupCollider.enabled = true;
    }

}
