using Cinemachine;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class RocketLauncher : MonoBehaviour
    {
        [Header("Bullet Behavior")]
        [SerializeField] private GameObject bullet;
        [SerializeField] private float shootForce = 50f;

        [Header("Magazine Stats")]
        [SerializeField] private int magazineCapacity = 1;
        [SerializeField] private int loadedRounds = 1;
        [SerializeField] private int initialAmmoReserve = 2;
        [SerializeField] private float reloadTime = 2.0f;

        [Header("Weapon Behavior")]
        [SerializeField] private bool isFullAuto = false;
        [SerializeField] private float timeBetweenShots = 1f;

        [Header("Recoil Strength")]
        [SerializeField] private float recoilResetTime = 0.5f;

        [Header("References")]
        [SerializeField] private Transform gun;
        [SerializeField] private Transform attackPoint;
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

        private readonly Vector3 gunPositionHip = new(0.51f, -0.2f, 0.270f);
        private readonly Quaternion gunRotationHip = Quaternion.Euler(-85f, -90f, 0f);
        private readonly Vector3 gunPositionAim = new(0.33f, -0.26f, 0.270f);
        private readonly Quaternion gunRotationAim = Quaternion.Euler(-69.69f, -90f, 0f);
        private readonly Vector3 gunPositionReload = new(-0.17f, -0.47f, 0.4f);
        private readonly Quaternion gunRotationReload = Quaternion.Euler(-133.56f, -94.4f, -82.5f);
        private readonly Vector3 gunPositionHipRecoil = new(0.51f, -0.2f, 0.15f);
        private readonly Vector3 gunPositionAimRecoil = new(0.51f, -0.2f, 0.15f);
        private readonly float hipFOV = 60f;
        private readonly float aimFOV = 40f;

        private bool readyToShoot, reloading, aiming;

        public bool Reloading { get { return reloading || !readyToShoot; } }

        private int bulletsShot, ammoReserve;

        private GameObject rocketVisual;

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

            // Instantiate bullet visual and place in muzzle
            rocketVisual = transform.GetChild(1).gameObject;
            ShowRocketVisual();
        }

        private void OnEnable()
        {
            if (WeaponController.Instance)
                WeaponController.Instance.FullAuto = isFullAuto;
        }

        private void ShowRocketVisual()
        {
            if (!rocketVisual)
                rocketVisual = transform.GetChild(1).gameObject;

            rocketVisual.SetActive(true);
        }

        private void HideRocketVisual()
        {
            rocketVisual.SetActive(false);
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
                gun.localRotation = gunRotationAim;
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
            if (!reloading)
            {
                gun.localPosition = gunPositionHip;
                gun.localRotation = gunRotationHip;
            }
        }

        public void Shoot()
        {
            if (readyToShoot && !reloading && loadedRounds > 0)
            {
                bulletsShot = 0;
                readyToShoot = false;

                // Find hit position with raycast
                Vector3 targetPoint;
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit))
                {
                    targetPoint = hit.point;
                }
                else
                {
                    targetPoint = cameraTransform.forward * 750;
                }

                // Direction from attackPoint to targetPoint
                Vector3 directionToHit = targetPoint - attackPoint.position;

                // Instantiate bullet/projectile
                GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
                currentBullet.transform.forward = directionToHit.normalized;

                // Add forces to bullet
                currentBullet.GetComponent<Rigidbody>().AddForce(directionToHit.normalized * shootForce, ForceMode.Impulse);

                // Prepare to load new bullet
                HideRocketVisual();

                // Play shoot sound
                gun.GetComponent<AudioSource>().PlayOneShot(gunshotAudio);

                // Instantiate muzzle flash if present
                if (muzzleFlash)
                {
                    Instantiate(muzzleFlash, attackPoint.position, Quaternion.Euler(0,-90,0));
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
            var recoilRotation = (aiming) ? gunRotationAim : gunRotationHip;
            gun.SetLocalPositionAndRotation(recoilPosition, recoilRotation);
        }

        private void ResetRecoil()
        {
            if (aiming)
                gun.SetLocalPositionAndRotation(gunPositionAim, gunRotationAim);
            else
                gun.SetLocalPositionAndRotation(gunPositionHip, gunRotationHip);
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
            // Instantiate new rocket in muzzle
            ShowRocketVisual();

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