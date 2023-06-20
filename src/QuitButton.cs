using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class QuitButton : MonoBehaviour
    {
        public void QuitApplication()
        {
            EventManager.Clear();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}