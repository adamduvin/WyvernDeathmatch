using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicWeapon : Weapon
{
    [SerializeField]
    private ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (isActive)
        {
            Debug.Log("hit");
            if (reloadTimeRemaining > 0.0f)
            {
                if (particles.isPlaying)
                {
                    particles.Stop();
                }
            }
            else if (particles.isStopped)
            {
                particles.Play();
            }
        }
        else if (particles.isPlaying)
        {
            particles.Stop();
        }
    }

    public override void SetupWeapon(PlayerShoot playerShoot, Image uiElement, Image counter, Text counterText)
    {
        base.SetupWeapon(playerShoot, uiElement, counter, counterText);

        //particles = weaponData.weaponParticles;
        particles = GetComponentInChildren<ParticleSystem>();
    }
}
