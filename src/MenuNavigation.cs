using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HendricksAustin.Lab6
{
    // A script that allows to target a specific selectable.
    // Used to preselect the main button.
    // Also allows the UI navigation with WASD and arrow keys.

    public class MenuNavigation : MonoBehaviour
    {
        public Selectable DefaultSelection;
        public bool ForceSelection = false;

        void Start()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        void LateUpdate()
        {
            if (EventSystem.current.currentSelectedGameObject == null || ForceSelection)
            {
                EventSystem.current.SetSelectedGameObject(DefaultSelection.gameObject);
            }
        }

        void OnDisable()
        {
            if (ForceSelection && EventSystem.current.currentSelectedGameObject == DefaultSelection.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
