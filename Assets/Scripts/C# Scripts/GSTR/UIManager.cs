using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject drawUI; // Reference to the drawing UI Canvas

    // Update is called once per frame
    void Update()
    {
        // Check if the designated key is being held down
        if (Input.GetKey(KeyCode.F)) // 'F' can be replaced with any key you want to use
        {
            OpenDrawUI(true);
        }
        else
        {
            OpenDrawUI(false);
        }
    }

    // Method to open or close the drawing UI
    public void OpenDrawUI(bool isOpen)
    {
        drawUI.SetActive(isOpen);
    }
}
