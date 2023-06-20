using UnityEditor;
using UnityEngine;

namespace HendricksAustin.Lab6 
{
    public class GameOverPanel : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI wavesSurvived;
        private TMPro.TextMeshProUGUI enemiesKilled;

        private static GameOverPanel _instance;

        public static GameOverPanel Instance
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
        }

        private void Start()
        {
            wavesSurvived = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
            enemiesKilled = transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>();
        }

        public void Show()
        {
            gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            wavesSurvived.text = "Waves Survived: " + (GameManager.Instance.GetRound() - 1);
            enemiesKilled.text = "Zombies Killed: " + GameManager.Instance.GetScore();
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

