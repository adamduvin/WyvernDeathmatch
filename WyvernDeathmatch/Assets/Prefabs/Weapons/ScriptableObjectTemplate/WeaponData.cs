using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public int impactDamage;
    public int areaOfEffectDamage;
    public float areaOfEffectDiameter;
    public DamageType damageType;
    public float force;
    public float maxVelocity;
    public float reloadTime = 0.0f;
    public float cooldownTime = 0.0f;      // Essentially Rate of Fire
    public float swapTime = 0.0f;
    public float maxAmmo = 0;
    public Vector3 ADSDistance;
    public GameObject projectilePrefab;
    public float maxDeviation;
    public float maxDeviationADS;
    public float deviationIncreaseModifier;    // Use to speed up or slow down rate of deviation increase
    public float deviationDecreaseModifier;
    public bool isAutomatic = false;
    public Vector3 positionOffset;              // Where it should be in relation to the player (in player's local space) TEMP MAYBE
    
    /*public Mesh weaponMesh;                     // Only set if it's a conventional weapon
    public ParticleSystem weaponParticles;      // Only set if it's a magic weapon*/
}
