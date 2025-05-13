using UnityEngine;

public class SpinMeds : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public int healAmount = 50;
    public float respawnTime = 10f;
    public GameObject visual;

    private Collider pickupCollider;

    void Start()
    {
        pickupCollider = GetComponent<Collider>();
        if (visual != null) visual.SetActive(true);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_controller player = other.GetComponent<player_controller>();
            if (player != null)
            {
                player.TakeDamage(-healAmount); // ✅ Negative damage = healing
            }

            StartCoroutine(Respawn());
        }
    }

    System.Collections.IEnumerator Respawn()
    {
        if (visual != null) visual.SetActive(false);
        if (pickupCollider != null) pickupCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        if (visual != null) visual.SetActive(true);
        if (pickupCollider != null) pickupCollider.enabled = true;
    }
}
