using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    private Animator anim;
    public SwordHitbox hitbox; // Assign in Inspector

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.Play("sword_swing", 0, 0f);
            StartCoroutine(ActivateHitboxTemporarily(0.2f, 0.3f)); // Adjust timing
        }
    }

    IEnumerator ActivateHitboxTemporarily(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        hitbox.ActivateHitbox();
        yield return new WaitForSeconds(duration);
        hitbox.DeactivateHitbox();
    }
}
