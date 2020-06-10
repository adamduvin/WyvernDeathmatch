using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class StartButton : MonoBehaviour
{
    [SerializeField]
    private GameObject parentCanvas;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(MakeMenuVisible);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MakeMenuVisible()
    {
        menu.SetActive(true);
        parentCanvas.SetActive(false);
    }
}
