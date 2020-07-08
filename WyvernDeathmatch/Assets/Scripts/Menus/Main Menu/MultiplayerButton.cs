using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class MultiplayerButton : ButtonBehavior
{
    /*[SerializeField]
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(StartMultiplayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    // May need to send player's information through here
    protected override void Action()
    {
        SceneManager.LoadScene("InstructionsScreen");
    }
}
