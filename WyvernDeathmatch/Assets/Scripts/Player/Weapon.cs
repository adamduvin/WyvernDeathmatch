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
    [SerializeField]
    private float reloadTimeRemaining = 0.0f;
    public float cooldownTime = 0.0f;      // Essentially Rate of Fire
    [SerializeField]
    private float cooldownTimeRemaining;
    public float swapTime = 0.0f;     // Make everyting const?
    private float swapTimeRemaining;
    public float maxAmmo = 0;
    [SerializeField]
    private float currentAmmo;
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

    [SerializeField]
    private GameObject projectileSpawnPoint;

    private Vector3 target;
    private PlayerShoot playerShoot;
    
    public Image UIElement;
    [SerializeField]
    private Image counter;
    [SerializeField]
    private Text counterText;

    private bool isReloading = false;

    #region Projectile Object Pool
    private int poolIndex = 0;
    private GameObject[] projectiles;
    #endregion

    void OnEnable()
    {
        currentDeviation = 0.0f;
        currentMaxDeviation = maxDeviation;
        currentAmmo = maxAmmo;
        // Instantiate projectiles and add them to the list
        /*if(counter.fillAmount <= 0)
        {
            Debug.Log(currentAmmo);
            counter.fillAmount = currentAmmo / maxAmmo;
            counterText.text = currentAmmo.ToString();
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();
        projectiles = new GameObject[(int)maxAmmo];
        CreateProjectiles();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(counter.fillAmount);
        if(swapTimeRemaining <= delta)
        {
            if ((Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo) || currentAmmo == 0)
            {
                isReloading = true;
                currentAmmo = -1;
                reloadTimeRemaining = reloadTime;
                cooldownTimeRemaining = 0.0f;
                counterText.text = "Reloading";
            }
            if (!isAutomatic && !Input.GetMouseButton(0) && cooldownTimeRemaining <= delta)
            {
                semiautoReadyToFire = true;
            }

            if (isActive)
            {
                if (!isReloading)
                {
                    if(cooldownTimeRemaining > delta)
                    {
                        cooldownTimeRemaining -= Time.deltaTime;
                    }
                    else if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
                    {
                        currentDeviation -= deviationDecreaseModifier;
                    }
                    else if((isAutomatic || semiautoReadyToFire) && cooldownTimeRemaining <= delta)
                    {
                        playerShoot.FindProjectileTarget(ref target);
                        Shoot();
                    }

                    currentDeviation = Mathf.Clamp(currentDeviation, 0.0f, currentMaxDeviation);
                }
                else
                {
                    //Debug.Log("Reloading");
                    reloadTimeRemaining -= Time.deltaTime;
                    counter.fillAmount = 1.0f - (reloadTimeRemaining / reloadTime);

                    if (reloadTimeRemaining <= delta)
                    {
                        isReloading = false;
                        currentAmmo = maxAmmo;
                        counter.fillAmount = currentAmmo / maxAmmo;
                        counterText.text = currentAmmo.ToString();
                    }
                }
            }
        }
        else
        {
            swapTimeRemaining -= Time.deltaTime;
        }
    }

    // Need to change to object pooling
    public void Shoot()
    {
        currentDeviation += deviationIncreaseModifier;
        GameObject projectile = projectiles[poolIndex]; //Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, Quaternion.identity);   // Not so sure about generating 3 random numbers here, performance wise. Maybe change this later.
        projectile.SetActive(true);
        projectile.transform.position = projectileSpawnPoint.transform.position;
        Rigidbody projRB = projectile.GetComponent<Rigidbody>();
        projRB.transform.LookAt(target);
        projRB.transform.Rotate(Quaternion.Euler(Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f), Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f), Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f)).eulerAngles);
        projRB.velocity = projRB.transform.forward.normalized * maxVelocity;
        if (!isAutomatic)
        {
            semiautoReadyToFire = false;
        }

        cooldownTimeRemaining = cooldownTime;

        currentAmmo--;
        counter.fillAmount = currentAmmo / maxAmmo;
        counterText.text = currentAmmo.ToString();

        poolIndex++;
        poolIndex %= (int)maxAmmo;
    }

    public void MakeWeaponActive()
    {
        isActive = true;
        cooldownTimeRemaining = 0.0f;
        counter.fillAmount = currentAmmo / maxAmmo;
        counterText.text = currentAmmo.ToString();
    }

    public void SwitchToWeapon()
    {
        MakeWeaponActive();
        swapTimeRemaining = swapTime;
        if (isReloading)
        {
            counter.fillAmount = 1.0f - (reloadTimeRemaining / reloadTime);
            counterText.text = "Reloading";
        }
    }

    private void CreateProjectiles()
    {
        for(int i = 0; i < projectiles.Length; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectiles[i] = projectile;
            projectile.SetActive(false);
        }
    }
}
