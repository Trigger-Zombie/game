using UnityEngine;

public class shotgun_bullet : MonoBehaviour
{
    public float speed = 80f; // maybe slightly slower than rifle
    public float lifetime = 5f; // shotgun pellets can despawn faster
    public float damage = 10f;   // less damage per pellet

    private Rigidbody rb;
    void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);

        //rb = GetComponent<Rigidbody>();
        
        // Apply initial force in the forward direction
        //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

        //Destroy(gameObject, lifetime);    
    }

    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Bullet"))
    {
        // Ignore collision if the other object is a bullet
        return;
    }
        Debug.Log("Shotgun pellet collided with: " + collision.gameObject.name);
        IDamageCapable target = collision.gameObject.GetComponent<IDamageCapable>();
        
        if (target != null)
        {
            Debug.Log("Pellet hit a damageable object, applying damage");
            target.TakeDMG(damage);
        }
        else
        {
            Debug.Log("Pellet hit something else.");
        }

        Destroy(gameObject);
    }
}
