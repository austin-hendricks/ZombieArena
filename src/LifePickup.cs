using System.Collections;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class LifePickup : MonoBehaviour
    {
        [SerializeField] private AudioClip pickupCollectAudio;
        private AudioSource audioSource;
        private CollectableAnimation collectableAnimator;

        private bool alreadyCollected = false;

        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            collectableAnimator = transform.GetComponentInParent<CollectableAnimation>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!alreadyCollected && other.CompareTag("Player") && !PlayerHasMaxLives())
            {
                alreadyCollected = true;
                EventManager.Broadcast(Events.AddLifeEvent);
                StartCoroutine(CollectPickup(0.5f));
            }
        }

        private IEnumerator CollectPickup(float time)
        {
            // stop all gem animations except rotation
            collectableAnimator.isBobbing = false;
            collectableAnimator.isScaling = false;
            collectableAnimator.rotationSpeed *= 5;
            
            // play gem collect audio and increment score
            audioSource.PlayOneShot(pickupCollectAudio);

            // float gem for given time
            Vector3 currentPosition = collectableAnimator.transform.position;
            Vector3 finalPosition = collectableAnimator.transform.position + (Vector3.up * 1.5f);
            float elapsedTime = 0f;
            while (elapsedTime < time)
            {
                collectableAnimator.transform.position = Vector3.Lerp(currentPosition, finalPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // after float time is up, destroy gem
            Destroy(gameObject);
        }

        private bool PlayerHasMaxLives()
        {
            return PlayerController.LivesRemaining == PlayerController.MaxLives;
        }
    }
}
