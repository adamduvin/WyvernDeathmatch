using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsContinueButton : ButtonBehavior
{
    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    protected override void Action()
    {
        SceneManager.LoadScene("MultiplayerScene");
    }
}
