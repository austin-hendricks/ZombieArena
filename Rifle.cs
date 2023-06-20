using Cinemachine;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class Rifle : MonoBehaviour
    {
        [Header("Bullet Behavior")]
        [SerializeField] private GameObject bullet;
        [SerializeField] private GameObject casing;
        [SerializeField] private GameObject bulletHole;
        [SerializeField] private float shootForce = 500f;
        [SerializeField] private float upwardForce;
        [SerializeField] private float casingEjectionForce = 1f;
        [SerializeField] private float casingEjectionUpwardForce = 1.5f;

        [Header("Magazine Stats")]
        [SerializeField] private int magazineCapacity = 45;
        [SerializeField] private int loadedRounds = 45;
        [SerializeField] private int initialAmmoReserve = 180;
        [SerializeField] private float reloadTime = 1.0f;

        [Header("Weapon Behavior")]
        [SerializeField] private bool isFullAuto = true;
        [SerializeField] private int bulletsPerTap = 1;
        [SerializeField] private float bulletSpread = 0f;
        [SerializeField] private float timeBetweenShots = 0.1f;

        [Header("Recoil Strength")]
        [SerializeField] private float muzzleFlipAngle = 1f;
        [SerializeField] private float recoilResetTime = 0.08f;

        [Header("References")]
        [SerializeField] private Transform gun;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private Transform ejectionPoint;
        [SerializeField] private GameObject muzzleFlash;

        [Header("Audio")]
        [SerializeField] private AudioClip gunshotAudio;
        [SerializeField] private AudioClip reloadAudio;
        [SerializeField] private AudioClip emptyGunAudio;

        [Header("Debug")]
        [SerializeField] private bool allowInvoke = true;

        public bool FullAuto { get { return isFullAuto; } }

        public int MagazineCapacity { get { return magazineCapacity; } }

        private Transform cameraTransform;
        private CinemachineVirtualCamera fpsCam;

        private readonly Vector3 gunPositionHip = Vector3.zero;
        private readonly Vector3 gunPositionAim = new(-0.25f, 0.07f, 0.21f);
        private readonly Vector3 gunPositionReload = new(0.13f, 0.09f, 0f);
        private readonly Quaternion gunRotationReload = Quaternion.Euler(11.09f, -30.09f, -11.68f);
        private readonly Vector3 gunPositionHipRecoil = new(0f, 0f, -0.1f);
        private readonly Vector3 gunPositionAimRecoil = new(-0.25f, 0.07f, 0.11f);
        private readonly float hipFOV = 60f;
        private readonly float aimFOV = 40f;

        private bool readyToShoot, reloading, aiming;

        public bool Reloading { get { return reloading; } }

        private int bulletsShot, ammoReserve;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;

            loadedRounds = magazineCapacity;
            ammoReserve = initialAmmoReserve;
            readyToShoot = true;
        }

        private void Start()
        {
            fpsCam = FindObjectOfType<CinemachineVirtualCamera>();
            WeaponController.Instance.FullAuto = isFullAuto;
        }

        private void OnEnable()
        {
            if (WeaponController.Instance)
                WeaponController.Instance.FullAuto = isFullAuto;
        }

        public void ResetWeapon()
        {
            loadedRounds = magazineCapacity;
            ammoReserve = initialAmmoReserve;
            ReloadFinish();
            reloading = false;
            StopAim();
            aiming = false;
            ResetRecoil();
            ResetShot();
            readyToShoot = true;
        }

        public void Aim()
        {
            if (!reloading)
            {
                aiming = true;

                // Zoom in
                fpsCam.m_Lens.FieldOfView = aimFOV;

                // Position gun
                gun.localPosition = gunPositionAim;
            }
        }

        public void StopAim()
        {
            aiming = false;

            // Reset zoom
            if (fpsCam)
                fpsCam.m_Lens.FieldOfView = hipFOV;
            else
                FindObjectOfType<CinemachineVirtualCamera>().m_Lens.FieldOfView = hipFOV;

            // Reposition gun
            if (!reloading) gun.localPosition = gunPositionHip;
        }

        public void Shoot()
        {
            if (readyToShoot && !reloading && loadedRounds > 0)
            {
                if (bulletsShot >= bulletsPerTap)
                {
                    bulletsShot = 0;
                }
                readyToShoot = false;

                // Find hit position with raycast
                Vector3 targetPoint;
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit))
                {
                    targetPoint = hit.point;
                    if (hit.transform.CompareTag("Predator"))
                    {
                        // Harm enemy
                        hit.transform.gameObject.GetComponent<Predator>().LoseHealth();
                    }
                    else if (hit.transform.CompareTag("Environment"))
                    {
                        // Instantiate bullet hole
                        Instantiate(bulletHole, hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
                    }
                }
                else
                {
                    targetPoint = cameraTransform.forward * 750;
                }

                // Direction from attackPoint to targetPoint
                Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

                // Calculate spread
                float x = Random.Range(-bulletSpread, bulletSpread);
                float y = Random.Range(-bulletSpread, bulletSpread);

                // Calculate new direction with spread
                Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

                // Instantiate bullet/projectile
                GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
                currentBullet.transform.forward = directionWithSpread.normalized;

                // Instantiate casing and apply initial velocity of player's velocity
                GameObject currentCasing = Instantiate(casing, ejectionPoint.position, Quaternion.identity);
                Physics.IgnoreCollision(currentCasing.GetComponentInChildren<Collider>(), PlayerController.Instance.GetComponentInChildren<Collider>(), true);
                currentCasing.transform.forward = directionWithoutSpread.normalized;

                // Add player's velocity to bullet casing
                if (InputManager.Instance.GetPlayerMovement().magnitude > 0)
                {
                    currentCasing.GetComponentInChildren<Rigidbody>().velocity = PlayerController.CurrentDirection * PlayerController.MoveSpeed;
                }

                // Add forces to bullet
                currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
                currentBullet.GetComponent<Rigidbody>().AddForce(cameraTransform.up * upwardForce, ForceMode.Impulse);

                // Add forces to casing
                currentCasing.GetComponentInChildren<Rigidbody>().AddForce(cameraTransform.right * casingEjectionForce, ForceMode.VelocityChange);
                currentCasing.GetComponentInChildren<Rigidbody>().AddForce(cameraTransform.up * casingEjectionUpwardForce, ForceMode.VelocityChange);

                // Play shoot sound
                gun.GetComponent<AudioSource>().PlayOneShot(gunshotAudio);

                // Instantiate muzzle flash if present
                if (muzzleFlash)
                {
                    Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity, gun);
                }

                loadedRounds--;
                bulletsShot++;

                // Handle Recoil
                SetRecoil();

                // Invoke ResetShot function and ResetRecoil function (if not already invoked)
                if (allowInvoke)
                {
                    Invoke(nameof(ResetRecoil), recoilResetTime);
                    Invoke(nameof(ResetShot), timeBetweenShots);
                    allowInvoke = false;
                }

                // If multiple bullets per trigger pull
                if (bulletsShot < bulletsPerTap && loadedRounds > 0)
                {
                    Debug.Log(bulletsShot + " / " + bulletsPerTap);
                    Invoke(nameof(Shoot), timeBetweenShots);
                }
            }
            else if (readyToShoot && !reloading && loadedRounds < 1)
            {
                WeaponController.Instance.FullAuto = false;
                gun.GetComponent<AudioSource>().PlayOneShot(emptyGunAudio);
            }
        }

        private void ResetShot()
        {
            UpdateAmmoDisplay();
            readyToShoot = true;
            allowInvoke = true;
            if (readyToShoot && !reloading && loadedRounds < 1)
                Reload();
        }

        private void SetRecoil()
        {
            var recoilPosition = (aiming) ? gunPositionAimRecoil : gunPositionHipRecoil;
            gun.SetLocalPositionAndRotation(recoilPosition, Quaternion.Euler(-muzzleFlipAngle, 0f, 0f));
        }

        private void ResetRecoil()
        {
            if (aiming)
                gun.SetLocalPositionAndRotation(gunPositionAim, Quaternion.identity);
            else
                gun.SetLocalPositionAndRotation(gunPositionHip, Quaternion.identity);
        }

        public void Reload()
        {
            if (loadedRounds < magazineCapacity && ammoReserve > 0 && !reloading)
            {
                reloading = true;

                if (aiming) StopAim();

                // Position gun
                gun.SetLocalPositionAndRotation(gunPositionReload, gunRotationReload);

                // Play reload sound
                gun.GetComponent<AudioSource>().PlayOneShot(reloadAudio);

                while (ammoReserve > 0 && loadedRounds < magazineCapacity)
                {
                    ammoReserve--;
                    loadedRounds++;
                }
                
                Invoke(nameof(ReloadFinish), reloadTime);
            }
        }

        private void ReloadFinish()
        {
            // Reposition gun
            gun.SetLocalPositionAndRotation(gunPositionHip, Quaternion.identity);

            UpdateAmmoDisplay();
            reloading = false;
        }

        public void UpdateAmmoDisplay()
        {
            WeaponInfoPanel.Instance.UpdateLoadedAmmoDisplay(loadedRounds);
            WeaponInfoPanel.Instance.UpdateReserveAmmoDisplay(ammoReserve);
        }

        public bool HasMaxAmmo()
        {
            return loadedRounds + ammoReserve == magazineCapacity + initialAmmoReserve;
        }

        public void MaxOutAmmo()
        {
            ammoReserve = initialAmmoReserve + (magazineCapacity - loadedRounds);
            if (loadedRounds <= 0)
            {
                Reload();
                WeaponController.Instance.FullAuto = isFullAuto;
            }
            UpdateAmmoDisplay();
        }
    }
}