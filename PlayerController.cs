using Cinemachine;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    [RequireComponent(typeof(CharacterController))]

    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private static float playerSpeed = 8.0f;
        [SerializeField]
        private float jumpHeight = 3.0f;
        [SerializeField]
        private float gravityValue = -9.81f;

        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool playerIsGrounded;
        private InputManager inputManager;
        private Transform cameraTransform;

        private bool currentlyBeingHurt = false;

        private static readonly int maxLives = 3;
        private static int livesRemaining;

        private static PlayerController _instance;

        public static PlayerController Instance
        {
            get { return _instance; }
        }

        public static int MaxLives
        {
            get { return maxLives; }
        }

        public static int LivesRemaining
        {
            get { return livesRemaining; }
        }

        public static float MoveSpeed
        {
            get { return playerSpeed; }
        }

        private Vector3 initialPositon;
        private Quaternion initialRotation;

        private bool gameOver = false;

        private static Vector3 currentDirection;

        public static Vector3 CurrentDirection
        {
            get { return currentDirection; }
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

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            inputManager = InputManager.Instance;
            cameraTransform = Camera.main.transform;
            livesRemaining = maxLives;
            HealthDisplay.Instance.MaxLives();

            EventManager.AddListener<AddLifeEvent>(OnPickupLife);
            EventManager.AddListener<GameOverEvent>(OnGameOver);

            initialPositon = transform.position;
            initialRotation = transform.rotation;

            StopCameraShake();
            StopBeingHurt();
        }

        private void Update()
        {
            if (!gameOver)
            {
                playerIsGrounded = controller.isGrounded;
                if (playerIsGrounded && playerVelocity.y < 0)
                {
                    playerVelocity.y = 0f;
                }

                Vector2 movement = inputManager.GetPlayerMovement();
                Vector3 move = new(movement.x, 0f, movement.y);
                move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
                move.y = 0f;
                move = move.normalized;
                currentDirection = move;

                float currentSpeed = playerSpeed;
                if (inputManager.GetPlayerSprint())
                {
                    currentSpeed *= 1.5f;
                }

                controller.Move(currentSpeed * Time.deltaTime * move);

                // Changes the height position of the player
                if (inputManager.PlayerJumpedThisFrame() && playerIsGrounded)
                {
                    playerVelocity.y += Mathf.Sqrt(-jumpHeight * gravityValue);
                }

                playerVelocity.y += gravityValue * Time.deltaTime;
                controller.Move(playerVelocity * Time.deltaTime);
            }
        }

        public void Harm()
        {
            if (!currentlyBeingHurt)
            {
                currentlyBeingHurt = true;
                livesRemaining--;
                HealthDisplay.Instance.LoseLife();
                Invoke(nameof(StopBeingHurt), 0.25f);

                // play hurt sound
                GetComponent<AudioSource>().Play();
            }

            // shake camera
            var camshaker = FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            camshaker.m_AmplitudeGain = 5f;
            camshaker.m_FrequencyGain = 1f;
            Invoke(nameof(StopCameraShake), 0.1f);

            if (livesRemaining <= 0)
            {
                // Game over
                EventManager.Broadcast(Events.GameOverEvent);
            }
        }

        private void StopCameraShake()
        {
            var camshaker = FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            camshaker.m_AmplitudeGain = 0f;
            camshaker.m_FrequencyGain = 0f;
        }

        private void StopBeingHurt()
        {
            currentlyBeingHurt = false;
        }

        private void OnPickupLife(AddLifeEvent e)
        {
            livesRemaining++;
            HealthDisplay.Instance.AddLife();
        }

        public void ResetPlayer()
        {
            livesRemaining = maxLives;
            StopCameraShake();
            StopBeingHurt();
            transform.position = initialPositon;
            transform.rotation = initialRotation;
            gameOver = false;
        }

        private void OnGameOver(GameOverEvent e)
        {
            gameOver = true;
        }
    }
}