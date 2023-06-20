using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class ObjectiveDisplay : MonoBehaviour
    {
        TMPro.TextMeshProUGUI progress;

        private static ObjectiveDisplay _instance;

        public static ObjectiveDisplay Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
            
            progress = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        }

        public void UpdateProgress(int enemiesRemaining)
        {
            progress.text = "Remaining: " + enemiesRemaining;
            progress.ForceMeshUpdate();
        }
    }
}