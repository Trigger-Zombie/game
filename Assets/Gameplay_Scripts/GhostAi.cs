using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GhostAi : MonoBehaviour
{
    public float launchForce;
    public float timeBetweenShots;
    private float timeSinceLastShot;
    public bool attacking = true;
    public Transform player;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    public GameObject FireBallPrefab;
    public Transform FirePoint;
    public Rigidbody ghostRigid;
    public int ghostSpeed = 2;


    void Awake()
    {
        // Get the NavMeshAgent component attached to this GameObject
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        ghostRigid = GetComponent<Rigidbody>();
        if (player != null && navMeshAgent != null && navMeshAgent.isOnNavMesh) // Check navMeshAgent is not null and is on a NavMesh
        {
            Debug.LogError("GhostAi: NavMeshAgent component not found on this GameObject!");
        }
    }
    private void Update()
    {
        // Increment how much time has passed since the last shot was fired
        timeSinceLastShot += Time.deltaTime;
    }

    void FixedUpdate()
    {
         if (player != null && ghostRigid != null)
        {
            ghostRigid.MovePosition(transform.position + player.position * Time.fixedDeltaTime * ghostSpeed);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (timeSinceLastShot >= timeBetweenShots)
                ShootFireBall();
            timeSinceLastShot = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            attacking = false;
        }
    }

    private void ShootFireBall()
    {
        // Instantiate the cannonball at the stationary ball's position
        GameObject newFireBall = Instantiate(FireBallPrefab, FirePoint.position, Quaternion.identity);

        // Get the Rigidbody component and apply force
        Rigidbody rb = newFireBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(FireBallPrefab.transform.forward * launchForce); // Note the negative sign, since we rotated the cannon 180 degrees
        }

        Destroy(newFireBall, 3f);
    }

}
