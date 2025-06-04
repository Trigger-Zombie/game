using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankMovement : MonoBehaviour
{
    //private GameObject destination;
    private NavMeshAgent agent;
    private Animator tankAnimator;
    public Transform player; // Assign your Player transform in the Inspector

    private player_controller playerController;
    private bool isSlamming = false;
    private bool playerCurrentlyInTrigger = false;

    public float chaseSpeed = 5f; // Configurable chase speed
    public float attackRotationSpeed = 10f; // Rotation speed during attack
    public float chaseRotationSpeed = 5f; // Rotation speed when chasing/facing player while idle in trigger

    public TankHitDetect hitDetect;

    public int damageAmount = 25;
    private TankHitBox AmAlive;
    


    void Start()
    {
        //destination = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        tankAnimator = GetComponent<Animator>();
        //playerController = player.GetComponent<player_controller>();
        AmAlive = GetComponent<TankHitBox>(); // Get the TankHitBox component from this GameObject ("HUGO")
        hitDetect = GetComponent<TankHitDetect>();
        if (hitDetect == null)
        {
            Debug.LogError("hitDetect is null");
        }
        if (AmAlive == null)
        {
            Debug.LogError("TankMovement (" + this.gameObject.name + "): Failed to find 'TankHitBox' component on this same GameObject. Please ensure it's attached.", this.gameObject);
            enabled = false; // Critical component missing, disable script
            return;
        }
        /*
        if (player == null && destination != null)
        {
            player = destination.transform; // Fallback if player public Transform not set
        }
        else if (player == null && destination == null)
        {
            Debug.LogError("Player Transform not set and GameObject with tag 'Player' not found!");
            enabled = false; // Disable script if no target
            return;
        } */
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player not found!");
                enabled = false;
                return;
            }
        }
        playerController = player.GetComponent<player_controller>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on this GameObject!");
            enabled = false;
            return;
        }

        if (tankAnimator == null)
        {
            Debug.LogError("Animator component not found on this GameObject!");
            enabled = false;
            return;
        }

        isSlamming = false;
        playerCurrentlyInTrigger = false; // Assume player is not in trigger at start

        // Set initial state to chase
        agent.speed = chaseSpeed;
        if (tankAnimator.gameObject.activeInHierarchy)
            tankAnimator.SetTrigger("TrRun");
    }

    void Update()
    {
        if (!AmAlive.alive)
        {
            // CoinManager coinManager = FindFirstObjectByType<CoinManager>();
            // if (coinManager != null)
            // {
            CoinManager.Instance.AddCoin(10);
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
        if (player == null) //|| destination == null) // Check if target is still valid
        {
            // Optionally, handle target loss (e.g., go to idle, search)
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }
        if (agent.isOnNavMesh) agent.isStopped = false;


        if (isSlamming)
        {
            agent.speed = 0f; // Ensure speed is 0 during slam
            RotateTowards(player.position, attackRotationSpeed);
        }
        else // Not slamming
        {
            if (playerCurrentlyInTrigger)
            {
                // Player is in trigger, but not slamming (e.g., just finished slam or waiting for conditions)
                agent.speed = 0f; // Stay put
                RotateTowards(player.position, chaseRotationSpeed); // Keep facing player
            }
            else // Player not in trigger, and not slamming: Chase
            {
                agent.speed = chaseSpeed;
                // RotateTowards(agent.steeringTarget, chaseRotationSpeed); // Rotate towards NavMeshAgent's desired direction
                // Or directly towards player if preferred for chase visuals
                RotateTowards(player.position, chaseRotationSpeed);


            }
            // Always update destination if not slamming, NavMeshAgent will move if speed > 0
            //if (agent.isOnNavMesh) agent.SetDestination(destination.transform.position);
            if (player == null)
            {
                if (agent.isOnNavMesh) agent.isStopped = true;
                return;
            }
            if (agent.isOnNavMesh) agent.isStopped = false;
            agent.SetDestination(player.position);
                    }
    }

    private void RotateTowards(Vector3 targetPosition, float speed)
    {
        Vector3 lookDirection = targetPosition - transform.position;
        lookDirection.y = 0; // Keep rotation horizontal

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCurrentlyInTrigger = true;
            // If not already slamming, OnTriggerStay will handle the attack initiation.
            // If it was running, its animation might switch to idle here or directly to slam via OnTriggerStay
            if (!isSlamming)
            {
                tankAnimator.ResetTrigger("TrRun"); // Optional: If you want to stop "TrRun" effect immediately
                                                    // agent.speed = 0f; // This will be handled by OnTriggerStay or Update based on playerCurrentlyInTrigger
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCurrentlyInTrigger = true; // Ensure flag is set
            if (!isSlamming)
            {
                // Conditions met to start a slam
                agent.speed = 0f; // Stop movement

                // Face player more directly before slamming
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection); // Snap look or quick Slerp
                }

                tankAnimator.SetTrigger("TrSlam");
                isSlamming = true;
            }
            // If isSlamming is true, Update() handles rotation and keeping speed at 0.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCurrentlyInTrigger = false;
            if (!isSlamming) // Only resume chase if not in the middle of a slam
            {
                agent.speed = chaseSpeed;
                tankAnimator.SetTrigger("TrRun");
            }
            // If isSlamming is true, SlamAnimationFinished() will handle resuming chase if player has indeed exited.
        }
    }

    public void SlamAnimationFinished()
    {
        isSlamming = false;

        if (playerCurrentlyInTrigger)
        {
            agent.speed = 0f;
        }
        else
        {
            agent.speed = chaseSpeed;
            tankAnimator.SetTrigger("TrRun");
        }
    }

    public void TankDamage()
    {
        if (hitDetect.playerCurrentlyInTrigger)
        {
            playerController.TakeDamage(damageAmount);
        }
    }

}