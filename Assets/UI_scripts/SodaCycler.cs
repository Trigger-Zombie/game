using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SodaCycler : MonoBehaviour
{
    [Header("Soda Cycling Settings")]
    public Renderer vendingMachineRenderer;
    public List<GameObject> sodaPrefabs;
    public Transform dropPoint;
    public float totalCycleTime = 5f;
    public float hueSpeed = 1.5f;

    public void StartSodaCycle()
    {
        if (dropPoint == null || sodaPrefabs == null || sodaPrefabs.Count == 0)
        {
            Debug.LogError("SodaCycler: Missing dropPoint or sodaPrefabs!");
            return;
        }

        StartCoroutine(CycleEmissionBeforeDrop());
    }

    IEnumerator CycleEmissionBeforeDrop()
    {
        float timer = 0f;

        while (timer < totalCycleTime)
        {
            float hue = Mathf.Repeat(Time.time * hueSpeed, 1f);
            Color pulsingColor = Color.HSVToRGB(hue, 1f, 1f);

            if (vendingMachineRenderer != null)
            {
                vendingMachineRenderer.material.SetColor("_EmissionColor", pulsingColor * 3f); // Emissive pulse
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Spawn final soda
        int finalIndex = Random.Range(0, sodaPrefabs.Count);
        Quaternion rotated = Quaternion.Euler(0, 0, 90);

        GameObject finalSoda = Instantiate(
            sodaPrefabs[finalIndex],
            dropPoint.position + Vector3.up * 0.3f,
            rotated
        );

        finalSoda.transform.localScale = Vector3.one * 1.5f;
        PerkSoda.sodaSpawned = true;

        Rigidbody rb = finalSoda.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * 1f + transform.up * 2f, ForceMode.Impulse);
        }

        Debug.Log("✅ Final soda spawned: " + finalSoda.name);
    }
}
