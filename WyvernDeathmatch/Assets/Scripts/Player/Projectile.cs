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
    private Rigidbody rb;


    //private const int playerMask = LayerMask.GetMask("Player");
    //private int playerMaskValue;

    // Start is called before the first frame update
    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //playerMask = LayerMask.GetMask("Player");
    }
    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        //gravity = Physics.gravity * gravityModifier;
        if (impactDamage <= 0.0f)
        {
            impactDamage = 1.0f;
        }
        areaOfEffectDiameter = areaOfEffectPrototype.GetComponent<AOE>().Diameter;
    }

    // Update is called once per frame
    /*void Update()
    {
        direction = transform.position;
        direction += velocity * Time.deltaTime;
        direction += gravity * Time.deltaTime;
        transform.LookAt(transform.position + (direction.normalized - transform.position));
        transform.position = direction;
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("AOE Debug");
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

    /*private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        // Remember to change case values if layers get changed
        //Debug.Log("Collision with " + collision.GetContact(0).otherCollider.name);
        //Debug.Log(collision.GetContact(0).otherCollider.GetComponent<TerrainCollider>().attachedRigidbody);
        switch (other.gameObject.layer)
        {
            case 8:
                other.GetComponent<PlayerCore>().TakeDamage(impactDamage, damageType);
                createAreaOfEffect(transform.position);
                gameObject.SetActive(false);
                break;
            case 9:
                createAreaOfEffect(transform.position);
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }*/

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

    public void Shoot(GameObject projectileSpawnPoint, Vector3 target, float currentDeviation, float maxVelocity)
    {
        rb.transform.position = projectileSpawnPoint.transform.position;
        rb.transform.LookAt(target);
        rb.transform.Rotate(Quaternion.Euler(Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f), Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f), Random.Range(-currentDeviation / 3.0f, currentDeviation / 3.0f)).eulerAngles);
        rb.velocity = rb.transform.forward.normalized * maxVelocity;
        //Debug.Log(transform.forward);
    }
    /*
    private void ResetProjectile()
    {
        gameObject.SetActive(false);
    }
    */
}
