using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ButtonBehavior : MonoBehaviour
{
    [SerializeField]
    protected GameObject parentCanvas;
    [SerializeField]
    protected Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(Action);
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    protected abstract void Action();
}
