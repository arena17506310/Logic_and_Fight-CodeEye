using UnityEngine;
public class BLockClick : MonoBehaviour
{
    public Color changeColor= Color.red;

    private Renderer myRenderer;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        
    }


    void OnMouseDown()
    {

        Debug.Log("Mouse Clicked");
        if (myRenderer != null)
        {
            myRenderer.material.color = changeColor;
        }

        
    }
    void OnMouseUp()
    {

        Debug.Log("Mouse Clicked");
        if (myRenderer != null)
        {
            myRenderer.material.color = Color.white;
        }

        
    }
}

