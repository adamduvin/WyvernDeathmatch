using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Globals;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected WeaponData weaponData;

    public int impactDamage;
    public int areaOfEffectDamage;
    public float areaOfEffectDiameter;
    public DamageType damageType;
    public float force;
    public float maxVelocity;
    public float reloadTime = 0.0f;
    [SerializeField]
    protected float reloadTimeRemaining = 0.0f;
    public float cooldownTime = 0.0f;      // Essentially Rate of Fire
    [SerializeField]
    protected float cooldownTimeRemaining;
    public float swapTime = 0.0f;
    protected float swapTimeRemaining;
    public float maxAmmo = 0;
    [SerializeField]
    protected float currentAmmo;
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
    public Vector3 localPosition = Vector3.zero;

    [SerializeField]
    protected GameObject projectileSpawnPoint;

    protected Vector3 target;
    protected PlayerShoot playerShoot;
    
    public Image uiElement;
    [SerializeField]
    protected Image counter;
    [SerializeField]
    protected Text counterText;

    protected bool isReloading = false;

    #region Projectile Object Pool
    protected int poolIndex = 0;
    protected GameObject[] projectiles;
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
        playerShoot = GetComponentInParent<PlayerShoot>().GetComponent<PlayerShoot>();
        projectiles = new GameObject[(int)maxAmmo];
        CreateProjectiles();
    }

    // Update is called once per frame
    protected virtual void Update()
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
                        //playerShoot.FindProjectileTarget(ref target, playerShoot.PlayerCamera, playerShoot.PlayerCamera.transform.forward, playerShoot.PlayerCamera);
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
        projectile.GetComponent<Projectile>().Shoot(projectileSpawnPoint, target, currentDeviation, maxVelocity);
        /*projectile.transform.position = projectileSpawnPoint.transform.position;
        Rigidbody projRB = projectile.GetComponent<Rigidbody>();
        projRB.transform.LookAt(target);
        projRB.transform.Rotate(Quaternion.Euler(Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f), Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f), Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f)).eulerAngles);
        projRB.velocity = projRB.transform.forward.normalized * maxVelocity;*/
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

    protected void CreateProjectiles()
    {
        for(int i = 0; i < projectiles.Length; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectiles[i] = projectile;
            projectile.SetActive(false);
        }
    }




    /// <summary>
    /// Initializes member fields from a WeaponData scriptable object.
    /// Damage and AOE related fields do not currently affect the projectile. An editor tool is currently planned to assist in creating weapons that will assign these values to the prefab during creation.
    /// </summary>
    /// <param name="playerShoot">The PlayerShoot component attached to the player</param>
    /// <param name="uiElement">UI Image shown when switching weapons</param>
    /// <param name="counter">UI Image indicating remaining ammo and reload progress</param>
    /// <param name="counterText">UI Text displaying remaining ammo and the reloading message</param>
    public virtual void SetupWeapon(PlayerShoot playerShoot, Image uiElement, Image counter, Text counterText)
    {
        impactDamage = weaponData.impactDamage;
        areaOfEffectDamage = weaponData.areaOfEffectDamage;
        areaOfEffectDiameter = weaponData.areaOfEffectDiameter;
        damageType = weaponData.damageType;
        force = weaponData.force;
        maxVelocity = weaponData.maxVelocity;
        reloadTime = weaponData.reloadTime;
        cooldownTime = weaponData.cooldownTime;
        swapTime = weaponData.swapTime;
        maxAmmo = weaponData.maxAmmo;
        ADSDistance = weaponData.ADSDistance;
        projectilePrefab = weaponData.projectilePrefab;
        maxDeviation = weaponData.maxDeviation;
        maxDeviationADS = weaponData.maxDeviationADS;
        deviationIncreaseModifier = weaponData.deviationIncreaseModifier;
        deviationDecreaseModifier = weaponData.deviationDecreaseModifier;
        isAutomatic = weaponData.isAutomatic;
        //transform.position = weaponData.positionOffset;

        currentDeviation = 0.0f;
        currentMaxDeviation = maxDeviation;
        currentAmmo = maxAmmo;
        projectiles = new GameObject[(int)maxAmmo];
        CreateProjectiles();

        this.playerShoot = playerShoot;
        this.uiElement = uiElement;
        this.counter = counter;
        this.counterText = counterText;
    }
}
