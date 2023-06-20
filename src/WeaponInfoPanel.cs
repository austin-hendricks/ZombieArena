using TMPro;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class WeaponInfoPanel : MonoBehaviour
    {
        private static WeaponInfoPanel _instance;

        public static WeaponInfoPanel Instance
        {
            get { return _instance; }
        }

        private TextMeshProUGUI _loadedAmmoDisplay;
        private TextMeshProUGUI _reserveAmmoDisplay;

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

            _loadedAmmoDisplay = GetComponentsInChildren<TextMeshProUGUI>()[0];
            _reserveAmmoDisplay = GetComponentsInChildren<TextMeshProUGUI>()[1];
        }

        public void UpdateLoadedAmmoDisplay(int loadedRounds)
        {
            _loadedAmmoDisplay.text = loadedRounds + "";
            _loadedAmmoDisplay.color = (loadedRounds < 5) ? Color.red : Color.white;
            _loadedAmmoDisplay.ForceMeshUpdate();
        }

        public void UpdateReserveAmmoDisplay(int amt)
        {
            _reserveAmmoDisplay.text = amt + "";
            _reserveAmmoDisplay.color = (amt < 5) ? Color.red : Color.white;
            _reserveAmmoDisplay.alpha = 0.75f;
            _reserveAmmoDisplay.ForceMeshUpdate();
        }
    }
}