using System.Collections;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class Countdown : MonoBehaviour
    {
        private static Countdown _instance;

        public static Countdown Instance
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

        private TMPro.TextMeshProUGUI timerText;

        private void Start()
        {
            timerText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        public void StartCountdown(float countdownFrom)
        {
            if (!timerText)
                timerText = GetComponentInChildren<TMPro.TextMeshProUGUI>();

            timerText.gameObject.SetActive(true);

            int seconds = Mathf.FloorToInt(countdownFrom);

            StartCoroutine(CountdownCoroutine(timerText, seconds));
        }

        private IEnumerator CountdownCoroutine(TMPro.TextMeshProUGUI timerText, int seconds)
        {
            int i = seconds;
            while (i > 0)
            {
                timerText.text = "Wave Incoming in: " + i.ToString();
                timerText.ForceMeshUpdate();
                yield return new WaitForSeconds(1);
                i--;
            }
            timerText.gameObject.SetActive(false);
        }
    }
}
