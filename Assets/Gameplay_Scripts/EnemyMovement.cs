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
    void Start()
    {
        attacking = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombAnimator = GetComponent<Animator>();
        navMeshAgent.updateRotation = true;
        navMeshAgent.angularSpeed = 180f; // Increase rotation speed
    }

    // Update is called once per frame
    void Update()
    {
        
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
        navMeshAgent.speed = zombieSpeed;
        isMoving = false;
        attacking = false;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")){
            Debug.Log("Attempting Hit");
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
