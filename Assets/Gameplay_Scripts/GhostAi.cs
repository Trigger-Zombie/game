using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GhostAi : MonoBehaviour
{
    public float launchForce;
    public float timeBetweenShots;
    private float timeSinceLastShot;
    public Transform player;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    public GameObject FireBallPrefab;
    public Transform FirePoint;
    private player_controller playerController;
    float ghostSpeed = 5;
    public ghostHitBox AmAlive;
    public WaveManager waveManager;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = ghostSpeed;
        timeSinceLastShot = 0f;
        // Get the NavMeshAgent component attached to this GameObject
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

        Transform ghostHealthBox = transform.Find("Ghost");

        if (AmAlive == null)
        {
            AmAlive = GetComponent<ghostHitBox>();
            if (AmAlive != null)
            {
                Debug.LogWarning("AmAlive (ghostHitBox) was not assigned in the Inspector for " + gameObject.name + ". Found it on the same GameObject.", this);
            }
            else
            {
                Debug.LogError("CRITICAL: AmAlive (ghostHitBox) is not assigned in the Inspector AND not found on " + gameObject.name + ". Ghost AI will likely fail.", this);
                // Disable the script if AmAlive is absolutely critical for Update
                if (this.enabled) this.enabled = false; // Check this.enabled to avoid issues if already being disabled
                return;
            }
        }
    }
    private void Update()
    {
        // In Update()
        timeSinceLastShot += Time.deltaTime;
        if (!AmAlive.alive)
        {
            // CoinManager coinManager = FindFirstObjectByType<CoinManager>();
            // if (coinManager != null)
            // {
            CoinManager.Instance.AddCoin(1);
            // Notify WaveManager
            if (AmAlive.waveManager != null)
            {
                AmAlive.waveManager.EnemyDied();
            }
            else
            {
                Debug.LogWarning("⚠️ waveManager is null on zombie death!");
            }

            Destroy(gameObject);
            return;
        }
        if (player != null)
        {
            Vector3 targetPosition = player.position;
            UnityEngine.AI.NavMeshHit hit;

            // Sample for a point on the NavMesh near the player's XZ, but at the ghost's height
            // or a defined flying height relative to the player.
            // This example tries to find a point on the NavMesh at the ghost's current Y level
            // above the player. Adjust the sourcePosition for sampling as needed.
            Vector3 sampleSourcePosition = new Vector3(player.position.x, transform.position.y, player.position.z);

            // Or, if you want the ghost to be at a fixed height above the player's NavMesh projection:
            // 1. First, sample player's position on THEIR NavMesh (if they have one) or ground.
            // 2. Then, calculate a target position above that for the ghost.
            // 3. Then, sample that target position on the GHOST'S NavMesh.

            // Simpler approach for now: try to find a point on the ghost's NavMesh
            // that corresponds to the player's XZ coordinates.
            if (UnityEngine.AI.NavMesh.SamplePosition(sampleSourcePosition, out hit, 5.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
            else
            {
                // Fallback or debug: Player is too far from a valid NavMesh point for the ghost
                // Maybe the ghost just hovers or uses a different behavior
                // For now, let's try the raw player XZ at ghost's Y
                Vector3 projectedPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
                navMeshAgent.SetDestination(projectedPosition); // This might still not be on NavMesh
                // Debug.LogWarning("Could not find a close NavMesh position for the ghost to target above player.");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other == null)
        {
            Debug.LogError("Collider 'other' is null in OnTriggerStay!");
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (player == null)
            {
                Debug.LogError("Player field is null in OnTriggerStay!");
                return;
            }
            if (navMeshAgent == null)
            {
                Debug.LogError("NavMeshAgent field is null in OnTriggerStay!");
                return;
            }

            if (timeSinceLastShot >= timeBetweenShots)
            {
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                Vector3 playerCenter = player.GetComponent<Collider>().bounds.center;
                Vector3 direction = playerCenter - FirePoint.position;
                Vector3 directionNormalized = direction.normalized;
                ShootFireBall(directionNormalized);
                navMeshAgent.speed = 0; // Should be safe now due to check above
                timeSinceLastShot = 0f;
                Debug.Log("Firing");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            navMeshAgent.speed = ghostSpeed;
        }
    }

    private void ShootFireBall(Vector3 directionNormalized)
    {
        // Instantiate the cannonball at the stationary ball's position
        GameObject newFireBall = Instantiate(FireBallPrefab, FirePoint.position, Quaternion.identity);

        // Get the Rigidbody component and apply force
        Rigidbody rb = newFireBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = directionNormalized * launchForce;
        }

        Destroy(newFireBall, 3f);
    }
}