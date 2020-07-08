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

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        // Remember to change case values if layers get changed
        switch (other.gameObject.layer)
        {
            case 8:
                other.GetComponent<PlayerCore>().TakeDamage(impactDamage, damageType);
                createAreaOfEffect();
                Destroy(gameObject);
                break;
            case 9:
                createAreaOfEffect();
                Destroy(gameObject);
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
                //Debug.Log("Exit");
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    public void createAreaOfEffect()
    {
        if (areaOfEffectDiameter > delta)
        {
            Instantiate(areaOfEffectPrototype, transform.position, Quaternion.identity);
        }
    }
}
