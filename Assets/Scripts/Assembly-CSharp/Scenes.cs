using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    // ����� �������� ����� �� �������
    public void Scene(int numberScenes)
    {
        SceneManager.LoadScene(numberScenes);
    }
}