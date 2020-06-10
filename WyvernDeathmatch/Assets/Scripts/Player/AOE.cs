using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class AOE : MonoBehaviour
{
    [SerializeField]
    private float duration;
    private float timer;

    [SerializeField]
    private float damage;
    [SerializeField]
    private DamageType damageType;
    [SerializeField]
    private float diameter;
    public float Diameter
    {
        get { return diameter; }
    }

    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    private void OnEnable()
    {
        timer = 0.0f;
        transform.localScale *= diameter;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            other.gameObject.GetComponent<PlayerCore>().TakeDamage(damage, damageType);
        }
    }
}
