using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Globals;

public class Weapon : MonoBehaviour
{
    public int impactDamage;
    public int areaOfEffectDamage;
    public float areaOfEffectDiameter;
    public DamageType damageType;
    public float force;
    public float maxVelocity;
    public float reloadTime = 0.0f;
    public float cooldownTime = 0.0f;      // Essentially Rate of Fire
    public float swapTime = 0.0f;     // Make everyting const?
    public float maxAmmo = 0.0f;
    public Vector3 ADSDistance;
    public GameObject projectilePrefab;
    public float maxDeviation;
    public float maxDeviationADS;
    public float currentMaxDeviation;
    public float currentDeviation;      // Gradually goes up to maxDeviation or maxDeviationADS while firing and goes down gradually when you stop shooting
    public float deviationIncreaseModifier;    // Use to speed up or slow down rate of deviation increase
    public float deviationDecreaseModifier;
    public bool isActive = false;
    public bool isAutomatic = false;
    public bool semiautoReadyToFire = false;        // Set to true in inspector if this weapon is semi-auto
    public Image UIElement;

    // Start is called before the first frame update
    void OnEnable()
    {
        currentDeviation = 0.0f;
        currentMaxDeviation = maxDeviation;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAutomatic && Input.GetMouseButtonUp(0))
        {
            semiautoReadyToFire = true;
        }
        if (isActive)
        {
            if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
            {
                currentDeviation -= deviationDecreaseModifier;
            }

            currentDeviation = Mathf.Clamp(currentDeviation, 0.0f, currentMaxDeviation);
        }
    }

    // Need to change to object pooling
    public void Shoot(Vector3 spawnPoint, Quaternion direction)
    {
        currentDeviation += deviationIncreaseModifier;
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint, direction * Quaternion.Euler(Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f), Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f), Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f)));   // Not so sure about generating 3 random numbers here, performance wise. Maybe change this later.
        Rigidbody projRB = projectile.GetComponent<Rigidbody>();
        projRB.velocity = projRB.transform.forward.normalized * maxVelocity;
        if (!isAutomatic)
        {
            semiautoReadyToFire = false;
        }
    }
}
