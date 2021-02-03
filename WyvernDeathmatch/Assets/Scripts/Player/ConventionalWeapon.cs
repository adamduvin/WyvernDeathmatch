using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConventionalWeapon : Weapon
{
    [SerializeField]
    private Mesh weaponMesh;

    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void SetupWeapon(PlayerShoot playerShoot, Image uiElement, Image counter, Text counterText)
    {
        base.SetupWeapon(playerShoot, uiElement, counter, counterText);

        //weaponMesh = weaponData.weaponMesh;
        weaponMesh = GetComponentInChildren<MeshFilter>().mesh;
    }
}
