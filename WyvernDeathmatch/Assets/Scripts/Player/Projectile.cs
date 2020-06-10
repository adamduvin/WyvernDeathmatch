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

    // Start is called before the first frame update
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
        switch (other.tag)
        {
            case "Player":
                other.GetComponent<PlayerCore>().TakeDamage(impactDamage, damageType);
                break;
            default:
                break;
        }

        createAreaOfEffect();
        Destroy(gameObject);
    }

    public void createAreaOfEffect()
    {
        if(areaOfEffectDiameter >= delta)
        {
            GameObject aoe = Instantiate(areaOfEffectPrototype, transform.position, Quaternion.identity);
        }
    }
}
