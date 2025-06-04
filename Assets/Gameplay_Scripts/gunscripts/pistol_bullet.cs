using UnityEngine;

public class pistol_bullet : MonoBehaviour
{
    public float speed = 50f;
    public float lifetime = 5f;
    public float damage = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
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
    
        Debug.Log("Collided with: " + collision.gameObject.name);
        // Directly check if the collided object implements IDamageCapable
        IDamageCapable target = collision.gameObject.GetComponent<IDamageCapable>();
        
        if (target != null)
        {
            // If the object has the IDamageCapable interface, apply damage
            //Debug.Log("Hit a damageable object, calling TakeDMG");
            target.TakeDMG(damage);
        }
        else
        {
            // If it doesn't implement IDamageCapable, you can log it (optional)
            //Debug.Log("Object does not implement IDamageCapable.");
        }

        Destroy(gameObject);
    }
}

