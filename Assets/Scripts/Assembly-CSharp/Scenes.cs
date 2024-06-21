using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    // Метод загрузки сцены по индексу
    public void Scene(int numberScenes)
    {
        SceneManager.LoadScene(numberScenes);
    }
}