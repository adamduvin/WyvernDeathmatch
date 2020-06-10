using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    private Image image;
    private bool isActive;
    [SerializeField]
    private float lerpValue = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        MakeActive();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Color color = image.color;
            color.a = Mathf.Lerp(color.a, 0.0f, lerpValue * Time.deltaTime);
            if(color.a < Globals.delta)
            {
                color.a = 0.0f;
                isActive = false;
            }
            image.color = color;
        }
    }

    public void MakeActive()
    {
        isActive = true;
        Color color = image.color;
        color.a = 1.0f;
        image.color = color;
    }
}
