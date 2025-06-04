using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    private Animator anim;
    public SwordHitbox hitbox; // Assign in Inspector
    public AudioClip shootClip;

    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.Play("sword_swing", 0, 0f);
            StartCoroutine(ActivateHitboxTemporarily(0.2f, 0.3f)); // Adjust timing
            audioSource.PlayOneShot(shootClip);
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
