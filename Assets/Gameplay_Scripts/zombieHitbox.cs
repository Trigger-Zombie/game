using UnityEngine;
using System.Collections;
public class zombieHitbox : MonoBehaviour, IDamageCapable
{
    public float zombieHealth = 100;
    public WaveManager waveManager;
    public bool alive = true;
    private Renderer zombieRenderer;
    private Color originalColor;
    public Color damageColor = Color.red;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the renderer from this GameObject or its children
        zombieRenderer = GetComponentInChildren<Renderer>();
        if (zombieRenderer != null)
        {
            // Clone material so each zombie has their own instance
            zombieRenderer.material = new Material(zombieRenderer.material);
            originalColor = zombieRenderer.material.color;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDMG(float Damage)
    {

        zombieHealth -= Damage;
        Debug.Log("Zombie Health: " + zombieHealth);

        if (zombieRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
        if (zombieHealth <= 0)
        {
            Debug.Log("Zombie would have died");
            alive = false;
        }
    }
    private IEnumerator FlashRed()
    {
        zombieRenderer.material.color = damageColor;
        yield return new WaitForSeconds(0.25f);
        zombieRenderer.material.color = originalColor;
    }
}