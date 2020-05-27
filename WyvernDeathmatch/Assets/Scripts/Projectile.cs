using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private int impactDamage;
    [SerializeField]
    private float areaOfEffectDamage;
    [SerializeField]
    private float areaOfEffectRange;
    [SerializeField]
    private DamageType damageType;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (impactDamage <= 0)
        {
            impactDamage = 1;
        }
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
        if(areaOfEffectRange >= delta)
        {
            // Create AOE
        }
    }
}
