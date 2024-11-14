using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnHandler : MonoBehaviour
{
    public Renderer buttonRenderer; // Reference to the 3D button’s Renderer component
    public Color hoverColor = Color.gray;
    public Color normalColor = Color.white;
    public string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = normalColor;
        }
        Debug.Log("Click milieu souris");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void reloadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("Click milieu souris");
    }
    // Call this method on hover or select
    public void OnHover()
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = hoverColor;
        }
    }

    // Call this method to reset color when not hovering
    public void OnExit()
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = normalColor;
        }
    }
}
