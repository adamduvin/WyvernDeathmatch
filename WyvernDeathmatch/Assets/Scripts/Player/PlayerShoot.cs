using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Globals;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private GameObject projectileSpawnPoint;

    [SerializeField]
    private Camera playerCamera;

    public Camera PlayerCamera
    {
        get { return playerCamera; }
        set { playerCamera = value; }
    }

    private float distance;

    public float Distance
    {
        set { distance = value; }
    }

    [SerializeField]
    private Weapon[] weaponPrefabs;     // Temp
    [SerializeField]
    private Weapon[] weapons;

    private Weapon currentWeapon;
    private int currentWeaponIndex;

    private float reloadTimeRemaining = 0.0f;
    private float cooldownTimeRemaining = 0.0f;
    private float swapTimeRemaining = 0.0f;

    private float currentReloadTime = 0.0f;
    private float currentCooldownTime = 0.0f;

    [SerializeField]
    private float spawnPointAdjustment = 0.0f;

    public static LayerMask maskExcludeSoftBoundary;

    [SerializeField]
    public Image[] weaponUIElements;
    [SerializeField]
    private Image counter;
    [SerializeField]
    private Text counterText;

    // Start is called before the first frame update
    private void Start()
    {
        /*maskExcludeSoftBoundary =~ LayerMask.GetMask("SoftBoundary");
        currentWeapon = weapons[0];
        currentWeapon.MakeWeaponActive();*/
    }

    void OnEnable()
    {
        //currentWeaponIndex = -1;
        /*currentWeapon = weapons[0];
        currentWeapon.MakeWeaponActive();*/
        //Debug.Log(currentWeapon.maxAmmo);
        //LoadNextWeapon();
        //projectileSpawnPoint = playerCamera.transform.forward * distance;
    }

    // Update is called once per frame
    void Update()
    {
        //projectileSpawnPoint = playerCamera.transform.position + (playerCamera.transform.forward * (distance + spawnPointAdjustment));

        OnSwapWeapon();

        // Is there a way to not do this?
        // Move this to weapon
        /*if(reloadTimeRemaining >= delta)
        {
            reloadTimeRemaining -= Time.deltaTime;
        }

        if(cooldownTimeRemaining >= delta)
        {
            cooldownTimeRemaining -= Time.deltaTime;
        }
        
        if(swapTimeRemaining >= delta)
        {
            swapTimeRemaining -= Time.deltaTime;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            if (reloadTimeRemaining < delta && cooldownTimeRemaining < delta && swapTimeRemaining < delta)
            {
                if(currentWeapon.isAutomatic || currentWeapon.semiautoReadyToFire)
                {
                    Shoot();
                    cooldownTimeRemaining = currentCooldownTime;
                }
            }
        }*/
    }

    // ToDo: Need to change camera numbers here
    private void OnSwapWeapon()
    {
        if(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > delta)
        {
            LoadNextWeapon();
            swapTimeRemaining = currentWeapon.swapTime;
            currentWeapon.uiElement.gameObject.GetComponent<WeaponUI>().MakeActive();
        }
    }

    private void LoadNextWeapon()
    {
        currentWeapon.currentDeviation = 0.0f;
        currentWeapon.isActive = false;
        currentWeaponIndex = ++currentWeaponIndex % 2;
        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.SwitchToWeapon();
        /*currentWeapon.isActive = true;
        currentReloadTime = currentWeapon.reloadTime;
        currentCooldownTime = currentWeapon.cooldownTime;
        reloadTimeRemaining = 0.0f;
        cooldownTimeRemaining = 0.0f;*/
    }

    public void FindProjectileTarget(ref Vector3 target)
    {
        RaycastHit hit;
        Vector3 hitLocation;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, Mathf.Infinity, maskExcludeSoftBoundary))
        {
            Debug.Log(hit.collider.gameObject.name);
            hitLocation = hit.point;
        }
        else
        {
            hitLocation = GetComponent<PlayerCore>().lookAtTarget.transform.position;
        }

        target = hitLocation;
    }

    // weaponPrefabs, counter, and counterText arguments will temporarily not be used. Later it will be popluated by the spawning system that posesses each player's loadout data
    public void SetupPlayerShoot(/*Weapon[] weaponPrefabs, ref Image[] weaponUIElements, ref Image counter, ref Text counterText*/)
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            Weapon weapon = Instantiate(weaponPrefabs[i], Vector3.zero, Quaternion.identity);
            weapon.transform.parent = gameObject.transform;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localPosition = Vector3.zero;
            weapon.SetupWeapon(this, weaponUIElements[i], counter, counterText);
            weapons[i] = weapon;
        }

        maskExcludeSoftBoundary = ~LayerMask.GetMask("SoftBoundary");
        currentWeapon = weapons[0];
        currentWeapon.MakeWeaponActive();
    }


    // See if this can be implemented better
    /*public static void FindProjectileTarget(ref Vector3 target, Vector3 start, Vector3 direction, Vector3 alternative)
    {
        RaycastHit hit;
        Vector3 hitLocation;
        if (Physics.Raycast(start, direction, out hit, Mathf.Infinity, maskExcludeSoftBoundary))
        {
            Debug.Log(hit.collider.gameObject.name);
            hitLocation = hit.point;
        }
        else
        {
            hitLocation = alternative;
        }

        target = hitLocation;
    }*/
}
