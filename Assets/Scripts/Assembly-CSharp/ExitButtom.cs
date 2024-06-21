using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ExitButton : MonoBehaviour, IPointerClickHandler
{
    public void ShowConfirmationScene()
    {
        try
        {
            SceneManager.LoadScene("_Menu");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load scene: " + ex.Message);
        }
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        ShowConfirmationScene();
    }
}