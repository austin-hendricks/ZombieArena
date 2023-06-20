using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class Crosshair : MonoBehaviour
    {
        private static Crosshair _instance;

        public static Crosshair Instance
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
    }
}