using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Globals;

public class PlayerCore : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private float health;
    public float Health
    {
        get { return health; }
    }
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private float healthLerpPercent;

    [SerializeField]
    private float maxStamina;
    private float stamina;
    public float Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    }
    [SerializeField]
    private float staminaConsumptionRate;
    public float StaminaConsumptionRate
    {
        get { return staminaConsumptionRate; }
    }
    [SerializeField]
    private float staminaReplenishRate;
    public float StaminaReplenishRate
    {
        get { return staminaReplenishRate; }
    }
    [SerializeField]
    private Image staminaBar;
    [SerializeField]
    private float staminaLerpPercent;



    [SerializeField]
    private float maxFlightStamina;
    private float flightStamina;
    public float FlightStamina
    {
        get { return flightStamina; }
        set { flightStamina = value; }
    }
    [SerializeField]
    private float flightStaminaConsumptionRate;
    public float FlightStaminaConsumptionRate
    {
        get { return flightStaminaConsumptionRate; }
    }
    private float flightSprintStaminaConsumptionRate;
    public float FlightSprintStaminaConsumptionRate
    {
        get { return flightSprintStaminaConsumptionRate; }
    }
    [SerializeField]
    private float flightStaminaReplenishRate;
    public float FlightStaminaReplenishRate
    {
        get { return flightStaminaReplenishRate; }
    }
    [SerializeField]
    private Image flightStaminaBar;
    [SerializeField]
    private float flightStaminaLerpPercent;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        stamina = maxStamina;
        flightStamina = maxFlightStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if(health > maxHealth)
        {
            health = maxHealth;
        }

        if(stamina > maxStamina)
        {
            stamina = maxStamina;
        }

        if(flightStamina > maxFlightStamina)
        {
            flightStamina = maxFlightStamina;
        }

        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, health / 100f, healthLerpPercent * Time.deltaTime); // Health divided by 100, but might need to change. Basically needs to be between 0.0 and 1.0.
        staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, stamina / 100f, staminaLerpPercent * Time.deltaTime);
        flightStaminaBar.fillAmount = Mathf.Lerp(flightStaminaBar.fillAmount, flightStamina / 100f, flightStaminaLerpPercent * Time.deltaTime);
    }

    public void TakeDamage(float damage, DamageType element)
    {
        health -= damage;
    }
}
