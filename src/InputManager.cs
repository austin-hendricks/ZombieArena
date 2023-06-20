using UnityEngine;
using HendricksAustin.Input;

namespace HendricksAustin.Lab6
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;

        public static InputManager Instance
        {
            get { return _instance; }
        }

        private Controls playerControls;
        private WeaponController weaponController;

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
            playerControls = new Controls();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _ = new QuitHandler(playerControls.Player.Quit);
        }

        private void OnEnable()
        {
            playerControls ??= new Controls();
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls?.Disable();
        }

        private void Start()
        {
            weaponController = WeaponController.Instance;
        }

        public Vector2 GetPlayerMovement()
        {
            return playerControls.Player.Move.ReadValue<Vector2>();
        }

        public Vector2 GetMouseDelta()
        {
            return playerControls.Player.Look.ReadValue<Vector2>();
        }

        public bool PlayerJumpedThisFrame()
        {
            return playerControls.Player.Jump.triggered;
        }

        public bool GetPlayerSprint()
        {
            return playerControls.Player.Sprint.IsPressed();
        }

        public bool GetPlayerAim()
        {
            return playerControls.Player.Aim.IsPressed();
        }

        public bool GetPlayerShoot()
        {
            if (weaponController.FullAuto)
            {
                return playerControls.Player.Shoot.IsPressed();
            }
            return playerControls.Player.Shoot.triggered;
        }

        public bool PlayerReloadedThisFrame()
        {
            return playerControls.Player.Reload.triggered;
        }

        public bool PlayerSwitchedToWeapon1ThisFrame()
        {
            return playerControls.Player.Weapon1.triggered;
        }

        public bool PlayerSwitchedToWeapon2ThisFrame()
        {
            return playerControls.Player.Weapon2.triggered;
        }

        public bool PlayerSwitchedToWeapon3ThisFrame()
        {
            return playerControls.Player.Weapon3.triggered;
        }

        public float PlayerCycledWeapons()
        {
            return playerControls.Player.CycleWeapons.ReadValue<Vector2>().y;
        }
    }
}