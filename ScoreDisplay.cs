using TMPro;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class ScoreDisplay : MonoBehaviour
    {
        private static ScoreDisplay _instance;

        public static ScoreDisplay Instance
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

        public void UpdateWave(int wave)
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Wave: " + wave;
        }

        public void UpdateScore(int score)
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Score: " + score;
        }
    }
}
