using Cinemachine;
using System.Collections;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject m_EnemyPrefab;
        [SerializeField] private GameObject[] m_Pickups;
        [SerializeField] private AudioClip m_ZombieDeathNoise;

        [Header("Variables")]
        [SerializeField] private float m_TimeBetweenRounds = 1f;

        private InputManager inputManager;
        private ScoreDisplay scoreDisplay;
        private ObjectiveDisplay objectiveDisplay;

        private int score = 0;
        private int round = 0;
        private int enemiesAtStartOfRound = 20;
        private int enemiesLeft = 20;

        private Vector3[] spawns = { 
            new Vector3(-78.75f, 17.5f, 78.75f), // Spawn A
            new Vector3(78.75f, 17.5f, 78.75f), // Spawn B
            new Vector3(78.75f, 17.5f, -78.75f), // Spawn C
            new Vector3(-78.75f, 17.5f, -78.75f), // Spawn D
            new Vector3(78.75f, 17.5f, 0f), // Spawn E
            new Vector3(-78.75f, 17.5f, 0f), // Spawn F
            new Vector3(0f, 17.5f, 78.75f), // Spawn G
            new Vector3(0f, 17.5f, -78.75f) // Spawn H
        };

        private static GameManager _instance;

        public static GameManager Instance
        {
            get { return _instance; }
        }

        public int GetRound()
        {
            return round;
        }

        public int GetScore()
        {
            return score;
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

            Time.timeScale = 1.0f;
            EventManager.Clear();
        }

        private void Start()
        {
            GameOverPanel.Instance.Hide();
            Crosshair.Instance.gameObject.SetActive(true);
            Camera.main.GetComponent<CinemachineBrain>().enabled = true;

            inputManager = InputManager.Instance;
            scoreDisplay = ScoreDisplay.Instance;
            objectiveDisplay = ObjectiveDisplay.Instance;
            scoreDisplay.UpdateWave(round);

            EventManager.AddListener<IncrementScoreEvent>(OnIncrementScore);
            EventManager.AddListener<GameOverEvent>(OnGameOver);

            WeaponController.Instance.ResetWeapons();
            HealthDisplay.Instance.MaxLives();
            PlayerController.Instance.ResetPlayer();

            round = 0;
            score = 0;
            enemiesAtStartOfRound = 20;

            StartGame();
        }

        private void StartGame()
        {
            scoreDisplay.UpdateWave(++round);
            enemiesLeft = enemiesAtStartOfRound;
            objectiveDisplay.UpdateProgress(enemiesAtStartOfRound);
            StartCoroutine(SpawnEnemies(enemiesAtStartOfRound));
            // Countdown to first wave
            Countdown.Instance.StartCountdown(m_TimeBetweenRounds + 7f);
        }

        private void BeginNewWave()
        {
            scoreDisplay.UpdateWave(++round);
            enemiesLeft = enemiesAtStartOfRound + (enemiesAtStartOfRound / 2);
            enemiesLeft = (enemiesLeft > 250) ? 250 : enemiesLeft; // clamp enemies to maximum 300 per wave
            enemiesAtStartOfRound = enemiesLeft;
            objectiveDisplay.UpdateProgress(enemiesAtStartOfRound);

            // spawn enemies in wave
            StartCoroutine(SpawnEnemies(enemiesAtStartOfRound));
        }

        private void OnIncrementScore(IncrementScoreEvent e)
        {
            scoreDisplay.UpdateScore(++score);
            objectiveDisplay.UpdateProgress(--enemiesLeft);

            GetComponent<AudioSource>().PlayOneShot(m_ZombieDeathNoise, 0.5f);

            if (enemiesLeft <= 0)
            {
                WaveOver();
            }
        }

        public void IncrementScoreBy(int value)
        {
            /* 
             * This method solves a readers-writers problem that occurs
             * when several IncrementScoreEvents are sent at once, 
             * all trying to read and increment the score simultaneously
             * (used for when a rocket explodes and kills several zombies 
             * at once).
             */

            if (enemiesLeft - value <= 0)
            {
                score += enemiesLeft;
                enemiesLeft = 0;

                WaveOver();
            }
            else
            {
                score += value;
                enemiesLeft -= value;
            }

            GetComponent<AudioSource>().PlayOneShot(m_ZombieDeathNoise, 0.5f);
            scoreDisplay.UpdateScore(score);
            objectiveDisplay.UpdateProgress(enemiesLeft);
        }

        private void WaveOver()
        {
            // play wave complete sound
            GetComponent<AudioSource>().Play();

            // Countdown to next wave
            Countdown.Instance.StartCountdown(m_TimeBetweenRounds + 7f);

            Invoke(nameof(BeginNewWave), m_TimeBetweenRounds);
        }

        private void OnGameOver(GameOverEvent e)
        {
            Time.timeScale = 0.0f;
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;
            GameOverPanel.Instance.Show();
            Crosshair.Instance.gameObject.SetActive(false);
            EventManager.Clear();
        }

        private IEnumerator SpawnEnemies(int numToSpawn)
        {
            while (numToSpawn > 0)
            {
                numToSpawn--;
                Instantiate(m_EnemyPrefab, spawns[Random.Range(0, spawns.Length)], Quaternion.identity);
                yield return new WaitForSeconds(0.25f);
            }
        }

        public void DropPickupAtRandom(Vector3 position)
        {
            if (transform.position.y < 10 && m_Pickups.Length > 0)
            {
                var rand = Random.Range(0, 1000);

                if (rand < 50)
                {
                    // 2% chance of life drop
                    Instantiate(m_Pickups[0], new Vector3(position.x, 0, position.z), Quaternion.identity);
                }
                else if (rand < 100)
                {
                    // 5% chance of ammo drop
                    Instantiate(m_Pickups[1], new Vector3(position.x, 0, position.z), Quaternion.identity);
                }
            }
        }
    }
}