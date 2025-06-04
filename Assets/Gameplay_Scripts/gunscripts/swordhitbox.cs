using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public float damage = 50f; // Sword damage
    public bool active = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;

        // Prevent self-hit or unnecessary tags if needed
        if (other.CompareTag("Player")) return;

        IDamageCapable target = other.GetComponent<IDamageCapable>();
        if (target != null)
        {
            target.TakeDMG(damage);
        }
    }

    public void ActivateHitbox() => active = true;
    public void DeactivateHitbox() => active = false;
}
