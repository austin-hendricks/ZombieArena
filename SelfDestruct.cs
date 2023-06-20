using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class SelfDestruct : MonoBehaviour
    {
        [SerializeField] private float m_SecondsUntilSelfDestruct = 2f;

        private float m_Seconds;

        private void Awake()
        {
            m_Seconds = 0f;
        }

        private void Update()
        {
            m_Seconds += Time.deltaTime;
            
            if (m_Seconds >= m_SecondsUntilSelfDestruct)
            {
                Destroy(gameObject);
            }
        }
    }
}