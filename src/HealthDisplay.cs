using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class HealthDisplay : MonoBehaviour
    {
        private bool[] lives = { true, true, true };

        private static HealthDisplay _instance;

        public static HealthDisplay Instance
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

            MaxLives();
        }

        public void LoseLife()
        {
            int lostLifeIndex = lives.Length - 1;
            while (!lives[lostLifeIndex])
            {
                lostLifeIndex--;

                if (lostLifeIndex < 0)
                {
                    Debug.Log("problem occured");
                }
            }
            transform.GetChild(lostLifeIndex).gameObject.SetActive(false);
            lives[lostLifeIndex] = false;
        }

        public void AddLife()
        {
            int addLifeIndex = 0;
            while (lives[addLifeIndex])
            {
                addLifeIndex++;

                if (addLifeIndex >= lives.Length)
                {
                    Debug.Log("problem occured");
                }
            }
            transform.GetChild(addLifeIndex).gameObject.SetActive(true);
            lives[addLifeIndex] = true;
        }

        public void MaxLives()
        {
            for (int i = 0; i < lives.Length; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                lives[i] = true;
            }
        }
    }
}
