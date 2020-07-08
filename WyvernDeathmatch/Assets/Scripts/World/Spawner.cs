using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Editor Debug Variables
    [SerializeField]
    private float debugRadius;
    #endregion

    [SerializeField]
    private float radius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        Gizmos.DrawWireSphere(transform.position, debugRadius);
    }

    public void SpawnPlayer(GameObject player)
    {

    }
}
