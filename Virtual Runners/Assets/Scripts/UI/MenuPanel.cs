using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    private Canvas canvas = null;    
    private MenuManager menuManager = null; // Reference to menu manager


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Retrieve canvas childed in Menu/UI prefab and call Setup function
        canvas = GetComponent<Canvas>();
    }
    
    // Gives reference to Menu Manager and establishes default menu page to display
    public void Setup(MenuManager menuManager)
    {
        // Establish Menu Manager reference
        this.menuManager = menuManager;
        Hide();
    }

    // Display desired current menu page
    public void Show()
    {
        canvas.enabled = true;
    }

    // Hides unused menu page(s)
    public void Hide()
    {
        canvas.enabled = false;
    }
}
