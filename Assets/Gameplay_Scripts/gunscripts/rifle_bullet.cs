using UnityEngine;

public class rifle_bullet : MonoBehaviour
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
        Debug.Log("YOU HIT SOMETHING");
        Destroy(gameObject);
    }
}

