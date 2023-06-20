using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class Predator : MonoBehaviour
    {
        [SerializeField] private int health = 2;
        [SerializeField] private float maxStraightLineTime = 1.5f;
        [SerializeField] private AudioClip hitAudio;
        [SerializeField] private float moveSpeed = 10f;

        private readonly float rotationSpeed = 300f;
        private readonly float viewDistance = 50f;

        private readonly int rayCount_obstacleAvoidance = 12; // Number of rays to cast
        private readonly float angleStep_obstacleAvoidance = 10.0f; // Angle between rays
        private readonly float maxDistance_obstacleAvoidance = 5f; // Maximum distance to cast rays

        private GameObject player = null;

        private PredatorState state;

        private float straightLineTimer = 0f;
        private float randomDirectionChange = 0f;

        private enum PredatorState
        {
            wander,
            bored,
            chase,
            ouchie
        }

        private void Start()
        {
            state = PredatorState.wander;
            EventManager.AddListener<GameOverEvent>(OnGameOver);
            GetComponent<AudioSource>().Play();
        }

        private void OnGameOver(GameOverEvent e)
        {
            GetComponent<AudioSource>().Stop();
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<GameOverEvent>(OnGameOver);
        }

        private void Update()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                state = PredatorState.wander;
            }

            switch (state)
            {
                case PredatorState.wander:
                    WanderAround();
                    if (TargetWithinRange())
                    {
                        state = PredatorState.chase;
                    }
                    if (straightLineTimer > maxStraightLineTime)
                    {
                        float random = 0f;
                        bool onLeft = ObstacleOnLeft();
                        bool onRight = ObstacleOnRight();
                        if (onLeft && !onRight)
                        {
                            random = Random.Range(60f, 120f);
                        }
                        else if (onRight && !onLeft)
                        {
                            random = Random.Range(-120f, -60f);
                        }
                        else if (!onLeft && !onRight)
                        {
                            random = Random.Range(-120f, 120f);
                        }
                        randomDirectionChange = transform.rotation.eulerAngles.y + random;
                        randomDirectionChange %= 360;
                        state = PredatorState.bored;
                        straightLineTimer = 0f;
                    }
                    break;

                case PredatorState.bored:
                    RandomDirectionChange();
                    break;

                case PredatorState.chase:
                    ChaseTarget();
                    if (!TargetWithinRange())
                    {
                        state = PredatorState.wander;
                    }
                    break;

                case PredatorState.ouchie:
                    break;

                default:
                    break;
            }

            GetComponent<Rigidbody>().velocity = moveSpeed / 2.0f * transform.forward + GetComponent<Rigidbody>().velocity.y * Vector3.up;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        private void RandomDirectionChange()
        {
            if (NeedToTurn() || (transform.rotation.eulerAngles.y - randomDirectionChange) % 360f < 1f)
            {
                state = PredatorState.wander;
            }

            Quaternion rotationTarget = Quaternion.Euler(0, randomDirectionChange, 0);
            transform.localRotation =
                Quaternion.RotateTowards(transform.localRotation, rotationTarget, rotationSpeed * Time.deltaTime);
        }

        private bool ObstacleOnLeft()
        {
            return Physics.Raycast(transform.position, transform.right * -1, maxDistance_obstacleAvoidance, 1 | 1<<6);
        }

        private bool ObstacleOnRight()
        {
            return Physics.Raycast(transform.position, transform.right, maxDistance_obstacleAvoidance, 1 | 1<<6);
        }

        private float DetermineAvoidObjectDirection()
        {
            int hitsLeft = 0;
            int hitsRight = 0;

            // Cast rays in a 120 degree field of view
            for (int i = 0; i <= rayCount_obstacleAvoidance; i++)
            {
                float rayAngle = i * angleStep_obstacleAvoidance - 90.0f;
                Vector3 rayDirection = Quaternion.Euler(0.0f, rayAngle, 0.0f) * transform.forward;

                if (Physics.Raycast(transform.position, rayDirection, maxDistance_obstacleAvoidance, 1 | 1<<6))
                {
                    if (rayAngle < 0.0f)
                    {
                        hitsLeft++;
                    }
                    else if (rayAngle > 0.0f)
                    {
                        hitsRight++;
                    }
                }
            }

            if (hitsRight < hitsLeft)
            {
                // Turn right
                return 1.0f;
            }
            // Turn left otherwise
            return -1.0f;
        }

        private bool NeedToTurn()
        {
            for (int r = -2; r <= 2; r += 2)
            {
                var offset = r * transform.right;
                if (Physics.Raycast(transform.position + offset, transform.forward, maxDistance_obstacleAvoidance, 1 | 1<<6))
                {
                    return true;
                }
            }
            if (Physics.Raycast(transform.position, transform.right, 1, 1 | 1<<6) 
                || Physics.Raycast(transform.position, transform.right * -1, 1, 1 | 1<<6)
                || Physics.Raycast(transform.position, (transform.right + transform.forward).normalized, 2, 1 | 1<<6)
                || Physics.Raycast(transform.position, (transform.right * -1 + transform.forward).normalized, 2, 1 | 1 << 6)
                ) 
            { 
                return true;
            }
            return false;
        }

        private void AvoidObjects()
        {
            float rotation = transform.eulerAngles.y;
            rotation += Time.deltaTime * rotationSpeed * DetermineAvoidObjectDirection();
            rotation %= 360;
            Quaternion rotationTarget = Quaternion.Euler(0, rotation, 0);
            transform.localRotation =
                Quaternion.RotateTowards(transform.localRotation, rotationTarget, rotationSpeed);
        }

        private void WanderAround()
        {
            // Avoid objects
            if (NeedToTurn())
            {
                straightLineTimer = 0f;
                AvoidObjects();
            }
            else
            {
                straightLineTimer += Time.deltaTime;
            }
        }

        private void ChaseTarget()
        {
            // Avoid objects
            if (NeedToTurn())
            {
                AvoidObjects();
            }
            // Chase player
            else if (TargetWithinRange())
            {
                Vector3 directionToPrey = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPrey.normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private bool TargetWithinRange()
        {
            if (transform.position.y > 2) return true;

            Vector3 directionToTarget = player.transform.position - transform.position;
            if (directionToTarget.magnitude <= viewDistance)
            {
                return true;
            }
            return false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponentInParent<PlayerController>().Harm();
            }
        }

        public void LoseHealth()
        {
            // Play hit noise
            GetComponent<AudioSource>().PlayOneShot(hitAudio);

            health--;
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            EventManager.Broadcast(new IncrementScoreEvent());

            GameManager.Instance.DropPickupAtRandom(transform.position);
            
            Destroy(gameObject);
        }

        public void Destroy()
        {
            GameManager.Instance.DropPickupAtRandom(transform.position);

            Destroy(gameObject);
        }
    }
}