using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public WaveManager waveManager;
    private NavMeshAgent navMeshAgent;
    private Animator zombAnimator;
    private bool isMoving = false;
    private bool attacking;
    public int zombieSpeed = 4;
    private zombieHitbox AmAlive;
    private player_controller playerController;
    public int damageAmount = 10;

    public AudioClip[] deathSounds;
    private AudioSource audioSource;
    private Renderer zombieRenderer;
    private bool isDying = false;

    private bool inRange = false;   
    void Start()
    {
        attacking = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombAnimator = GetComponent<Animator>();
        navMeshAgent.updateRotation = true;
        navMeshAgent.angularSpeed = 180f;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        if (player != null)
        {
            playerController = player.GetComponent<player_controller>();
        }

        Transform zombieHealthbox = transform.Find("ZombieMesh");
        if (zombieHealthbox != null)
        {
            AmAlive = zombieHealthbox.GetComponent<zombieHitbox>();
            zombieRenderer = zombieHealthbox.GetComponent<Renderer>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (AmAlive != null && !AmAlive.alive && !isDying)
        {
            Debug.Log("Starting HandleDeath()");
            StartCoroutine(HandleDeath());
            return;
        }

        if (player != null && AmAlive != null && AmAlive.alive)
        {
            navMeshAgent.SetDestination(player.position);

            if (attacking)
            {
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0;

                if (lookDirection != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
                }
            }

            float speed = navMeshAgent.velocity.magnitude;
            if (!attacking && !isMoving)
            {
                navMeshAgent.speed = zombieSpeed;
                zombAnimator.SetTrigger("TrMove");
                isMoving = true;
            }
            else if (speed > 0.1f && !isMoving)
            {
                navMeshAgent.speed = zombieSpeed;
                zombAnimator.SetTrigger("TrMove");
                isMoving = true;
            }
        }
    }

    private IEnumerator HandleDeath()
    {
        isDying = true;

        // Stop movement and attacks
        if (navMeshAgent != null) navMeshAgent.enabled = false;
        attacking = false;
        isMoving = false;

        // Disable collider(s)
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Hide visuals
        if (zombieRenderer != null)
        {
            zombieRenderer.enabled = false;
        }

        // Award coin and notify wave manager
        CoinManager.Instance?.AddCoin(1);
        if (waveManager != null)
        {
            waveManager.EnemyDied();
        }

        // Play death sound
        if (deathSounds.Length > 0)
        {
            int index = Random.Range(0, deathSounds.Length);
            AudioClip clip = deathSounds[index];
            audioSource.PlayOneShot(clip);
            Debug.Log("Playing death sound: " + clip.name);
            yield return new WaitForSeconds(clip.length);
        }

        Destroy(gameObject);
    }

    public void OnAnimationComplete()
    {
        if (playerController != null && inRange)
        {
            playerController.TakeDamage(damageAmount);
        }

        navMeshAgent.speed = zombieSpeed;
        isMoving = false;
        attacking = false;
    }

    public void OnTriggerStay(Collider other)
    {
        if (isDying || !AmAlive.alive) return;  //  Block attacks while dead or dying

        if (other.CompareTag("Player") && !attacking)
        {
            navMeshAgent.speed = 0;
            zombAnimator.SetTrigger("TrAttack");
            attacking = true;
            inRange = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        attacking = false;
        inRange = false;
    }
}
