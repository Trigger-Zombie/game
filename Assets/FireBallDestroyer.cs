// In FireBallDestroyer.cs
using UnityEngine;
public class FireBallDestroyer : MonoBehaviour
{
    private player_controller playerController; 
    public Transform player;
    public int damageAmount;
    private void Start()
    {
        damageAmount = 10;
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform; // Assuming the player has the tag "Player"
        }
        if (player != null)
        {
            playerController = player.GetComponent<player_controller>();
            if (playerController == null)
            {
                Debug.LogError("player_controller script not found on player object.");
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Log detailed information about the collision
        Debug.Log($"FIREBALL COLLIDED WITH: Name: {collision.gameObject.name}, " +
                  $"Tag: {collision.gameObject.tag}, " +
                  $"Layer: {LayerMask.LayerToName(collision.gameObject.layer)}");

        // You might want to comment this out temporarily to see if the fireball
        // visually bounces or just passes through if the layer logic was almost working.
        if (collision.gameObject.tag == "Player")
        {
            playerController.TakeDamage(damageAmount);
        }
        Destroy(gameObject);
    }
}
