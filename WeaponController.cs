using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class WeaponController : MonoBehaviour
    {
        private static WeaponController _instance;

        public static WeaponController Instance
        {
            get { return _instance; }
        }

        private bool _fullAuto = false;

        public bool FullAuto
        {
            get { return _fullAuto; }
            set { _fullAuto = value; }
        }

        public enum Weapon
        {
            Pistol,
            Rifle,
            RocketLauncher
        }

        [SerializeField] private Weapon currentWeapon = Weapon.Rifle;

        private InputManager inputManager;

        private Pistol equippedPistol;
        private Rifle equippedRifle;
        private RocketLauncher equippedRocketLauncher;

        private bool switching = false;
        private readonly float weaponSwitchTime = 0.2f;

        private bool gameOver = false;

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
            inputManager = InputManager.Instance;

            EventManager.AddListener<AddAmmoEvent>(OnPickupAmmo);
            EventManager.AddListener<GameOverEvent>(OnGameOver);

            equippedPistol = Camera.main.GetComponentInChildren<Pistol>();
            equippedRifle = Camera.main.GetComponentInChildren<Rifle>();
            equippedRocketLauncher = Camera.main.GetComponentInChildren<RocketLauncher>();

            ResetWeapons();

            gameOver = false;
        }

        private void OnGameOver(GameOverEvent e)
        {
            gameOver = true;
        }

        private void Update()
        {
            if (!gameOver && !CurrentlyReloading())
            {
                OperateWeapon();

                if (inputManager.PlayerSwitchedToWeapon1ThisFrame() && !switching && currentWeapon != Weapon.Rifle)
                {
                    SwitchToRifle();
                }

                if (inputManager.PlayerSwitchedToWeapon2ThisFrame() && !switching && currentWeapon != Weapon.Pistol)
                {
                    SwitchToPistol();
                }

                if (inputManager.PlayerSwitchedToWeapon3ThisFrame() && !switching && currentWeapon != Weapon.RocketLauncher)
                {
                    SwitchToRocketLauncher();
                }

                if (inputManager.PlayerCycledWeapons() > 0f && !switching)
                {
                    SwitchWeaponsUp();
                }
                else if (inputManager.PlayerCycledWeapons() < 0f && !switching)
                {
                    SwitchWeaponsDown();
                }
                
            }
        }

        private void OperateWeapon()
        {
            switch (currentWeapon)
            {
                case Weapon.Pistol:
                    if (inputManager.GetPlayerAim())
                    {
                        equippedPistol.Aim();
                    }
                    else
                    {
                        equippedPistol.StopAim();
                    }

                    if (inputManager.GetPlayerShoot())
                    {
                        equippedPistol.Shoot();
                    }

                    if (inputManager.PlayerReloadedThisFrame())
                    {
                        equippedPistol.Reload();
                    }
                    break;

                case Weapon.Rifle:
                    if (inputManager.GetPlayerAim())
                    {
                        equippedRifle.Aim();
                    }
                    else
                    {
                        equippedRifle.StopAim();
                    }

                    if (inputManager.GetPlayerShoot())
                    {
                        equippedRifle.Shoot();
                    }

                    if (inputManager.PlayerReloadedThisFrame())
                    {
                        equippedRifle.Reload();
                    }
                    break;

                case Weapon.RocketLauncher:
                    if (inputManager.GetPlayerAim())
                    {
                        equippedRocketLauncher.Aim();
                    }
                    else
                    {
                        equippedRocketLauncher.StopAim();
                    }

                    if (inputManager.GetPlayerShoot())
                    {
                        equippedRocketLauncher.Shoot();
                    }

                    if (inputManager.PlayerReloadedThisFrame())
                    {
                        equippedRocketLauncher.Reload();
                    }
                    break;

                default:
                    break;
            }
        }

        private void SwitchWeaponsUp()
        {
            if (currentWeapon == Weapon.Pistol)
            {
                SwitchToRifle();
            }
            else if (currentWeapon == Weapon.Rifle)
            {
                SwitchToRocketLauncher();
            }
            else if (currentWeapon == Weapon.RocketLauncher)
            {
                SwitchToPistol();
            }
        }

        private void SwitchWeaponsDown()
        {
            if (currentWeapon == Weapon.Pistol)
            {
                SwitchToRocketLauncher();
            }
            else if (currentWeapon == Weapon.Rifle)
            {
                SwitchToPistol();
            }
            else if (currentWeapon == Weapon.RocketLauncher)
            {
                SwitchToRifle();
            }
        }

        private void SwitchToPistol()
        {
            switching = true;
            Invoke(nameof(FinishWeaponSwitch), weaponSwitchTime);
            currentWeapon = Weapon.Pistol;
            equippedRocketLauncher.gameObject.SetActive(false);
            equippedRifle.gameObject.SetActive(false);
            equippedPistol.gameObject.SetActive(true);
            equippedPistol.UpdateAmmoDisplay();
        }

        private void SwitchToRifle()
        {
            switching = true;
            Invoke(nameof(FinishWeaponSwitch), weaponSwitchTime);
            currentWeapon = Weapon.Rifle;
            equippedRocketLauncher.gameObject.SetActive(false);
            equippedPistol.gameObject.SetActive(false);
            equippedRifle.gameObject.SetActive(true);
            equippedRifle.UpdateAmmoDisplay();
        }

        private void SwitchToRocketLauncher()
        {
            switching = true;
            Invoke(nameof(FinishWeaponSwitch), weaponSwitchTime);
            currentWeapon = Weapon.RocketLauncher;
            equippedPistol.gameObject.SetActive(false);
            equippedRifle.gameObject.SetActive(false);
            equippedRocketLauncher.gameObject.SetActive(true);
            equippedRocketLauncher.UpdateAmmoDisplay();
        }

        private void FinishWeaponSwitch()
        {
            switching = false;
        }

        private void OnPickupAmmo(AddAmmoEvent e)
        {
            if (currentWeapon == Weapon.Pistol)
                equippedPistol.MaxOutAmmo();
            else if (currentWeapon == Weapon.Rifle)
                equippedRifle.MaxOutAmmo();
            else
                equippedRocketLauncher.MaxOutAmmo();
        }

        public bool EquippedHasMaxAmmo()
        {
            if (currentWeapon == Weapon.Pistol)
                return equippedPistol.HasMaxAmmo();
            else if (currentWeapon == Weapon.Rifle)
                return equippedRifle.HasMaxAmmo();
            else
                return equippedRocketLauncher.HasMaxAmmo();
        }

        private bool CurrentlyReloading()
        {
            if (currentWeapon == Weapon.Pistol)
                return equippedPistol.Reloading;
            else if (currentWeapon == Weapon.Rifle)
                return equippedRifle.Reloading;
            else
                return equippedRocketLauncher.Reloading;
        }

        public void ResetWeapons()
        {
            equippedPistol.ResetWeapon();
            equippedRifle.ResetWeapon();
            equippedRocketLauncher.ResetWeapon();

            equippedRifle.gameObject.SetActive(false);
            equippedPistol.gameObject.SetActive(false);
            equippedRocketLauncher.gameObject.SetActive(false);

            currentWeapon = Weapon.Rifle;
            equippedRifle.gameObject.SetActive(true);
            equippedRifle.UpdateAmmoDisplay();
        }
    }
}