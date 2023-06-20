using UnityEngine;
using UnityEngine.SceneManagement;

namespace HendricksAustin.Lab6
{
    public class LoadSceneButton : MonoBehaviour
    {
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}