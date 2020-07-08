using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : ButtonBehavior
{
    [SerializeField]
    private GameObject menu;
    // Start is called before the first frame update
    /* void Start()
     {
         button.onClick.AddListener(MakeMenuVisible);
     }

     // Update is called once per frame
     void Update()
     {

     }*/

    protected override void Action()
    {
        menu.SetActive(true);
        parentCanvas.SetActive(false);
    }
}
