using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float impactDamage;
    [SerializeField]
    private float areaOfEffectDiameter;
    [SerializeField]
    private DamageType damageType;
    [SerializeField]
    private GameObject areaOfEffectPrototype;
    //private const int playerMask = LayerMask.GetMask("Player");
    //private int playerMaskValue;

    // Start is called before the first frame update
    private void Start()
    {
        //playerMask = LayerMask.GetMask("Player");
    }
    void OnEnable()
    {
        if (impactDamage <= 0.0f)
        {
            impactDamage = 1.0f;
        }
        areaOfEffectDiameter = areaOfEffectPrototype.GetComponent<AOE>().Diameter;
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(other.name);
        // Remember to change case values if layers get changed
        //Debug.Log("Collision with " + collision.GetContact(0).otherCollider.name);
        //Debug.Log(collision.GetContact(0).otherCollider.GetComponent<TerrainCollider>().attachedRigidbody);
        switch (collision.collider.gameObject.layer)
        {
            case 8:
                collision.collider.GetComponent<PlayerCore>().TakeDamage(impactDamage, damageType);
                createAreaOfEffect(collision.GetContact(0).point);
                gameObject.SetActive(false);
                break;
            case 9:
                createAreaOfEffect(collision.GetContact(0).point);
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 10:
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void createAreaOfEffect(Vector3 impactPoint)
    {
        if (areaOfEffectDiameter > delta)
        {
            Instantiate(areaOfEffectPrototype, impactPoint, Quaternion.identity);
        }
    }
    /*
    private void ResetProjectile()
    {
        gameObject.SetActive(false);
    }
    */
}
