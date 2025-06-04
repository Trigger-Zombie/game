using System.Collections;
using UnityEngine;

public class SpeedBoostManager : MonoBehaviour
{
    public float dashForce = 800f;
    public float dashCooldown = 5f;
    public int maxCharges = 2;
    private int currentCharges;

    private bool isDashing = false;
    private float dashDuration = 0.2f;
    private float rechargeTimer = 0f;

    private Rigidbody rb;
    private player_controller player;

    public static SpeedBoostManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<player_controller>();
        rb = playerObj.GetComponent<Rigidbody>();
        currentCharges = maxCharges;
    }

    void Update()
    {
        // Recharge dash charges
        if (currentCharges < maxCharges)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= dashCooldown)
            {
                currentCharges++;
                rechargeTimer = 0f;
                Debug.Log("Dash charge replenished. Charges: " + currentCharges);
            }
        }

        // Dash with Left Shift
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentCharges > 0 && !isDashing)
        {
            Debug.Log("Dash triggered!");
            StartCoroutine(DashForward());
        }
    }

    private IEnumerator DashForward()
    {
        isDashing = true;
        currentCharges--;

        Vector3 dashDirection = player.transform.forward;
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }
}
