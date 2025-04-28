using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator zombAnimator;
    private bool isMoving = false;
    private bool attacking;
    public int zombieSpeed = 4;
    private zombieHitbox AmAlive;

    private player_controller playerController; 
    public float damageAmount = 10f;
    

    void Start()
    {
        attacking = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombAnimator = GetComponent<Animator>();
        navMeshAgent.updateRotation = true;
        navMeshAgent.angularSpeed = 180f; // Increase rotation speed
        
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
        
        Transform zombieHealthbox = transform.Find("ZombieMesh");

        if (zombieHealthbox != null)
        {
            // Get the amIAliveScript component attached to the amIAlive object
            AmAlive = zombieHealthbox.GetComponent<zombieHitbox>();

            if (AmAlive == null)
            {
                Debug.LogError("ChildScript not found on the child object.");
            }
        }
        else
        {
            Debug.LogError("Child object not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!AmAlive.alive){
            Destroy(gameObject);
        }

        if(player != null)
        {
            navMeshAgent.SetDestination(player.position);

                if(attacking)
            {
                // Calculate direction to player (ignoring Y axis)
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0;
                
                if(lookDirection != Vector3.zero)
                {
                    // Create rotation to face player
                    Quaternion rotation = Quaternion.LookRotation(lookDirection);
                    // Apply rotation
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
                }
            }
            
            float speed = navMeshAgent.velocity.magnitude; //gets the current speed of the zombie
            if(!attacking && !isMoving){
                navMeshAgent.speed = zombieSpeed;
                zombAnimator.SetTrigger("TrMove");
                isMoving = true;
            }
            else if(speed > 0.1f && !isMoving){
                navMeshAgent.speed = zombieSpeed;
                zombAnimator.SetTrigger("TrMove");
                isMoving = true;
            }
        }


    }

    public void OnAnimationComplete()
    {
        if (playerController != null)
        {
            playerController.TakeDamage(damageAmount);  // Apply damage to player
        }

        navMeshAgent.speed = zombieSpeed;
        isMoving = false;
        attacking = false;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")){
            //Debug.Log("Attempting Hit");
            if(!attacking)
            {
                navMeshAgent.speed = 0;
                zombAnimator.SetTrigger("TrAttack");
                attacking = true;
            }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        attacking = false;
    }

}
