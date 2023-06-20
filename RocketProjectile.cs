using System.Collections.Generic;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class RocketProjectile : MonoBehaviour
    {
        [SerializeField] private GameObject explosion;
        [SerializeField] private float explosionRadius = 8;

        private bool exploded = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (!exploded)
            {
                if (collision.gameObject.CompareTag("Predator") || collision.gameObject.CompareTag("Environment"))
                {
                    exploded = true;

                    if (explosion)
                        Instantiate(explosion, transform.position, Quaternion.identity);

                    HashSet<GameObject> dead = new();
                    Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
                    foreach (Collider collider in hits)
                    {
                        if (collider && collider.gameObject.CompareTag("Predator") && !dead.Contains(collider.gameObject))
                        {
                            dead.Add(collider.gameObject);
                            collider.gameObject.GetComponent<Predator>().Destroy();
                        }
                    }

                    GameManager.Instance.IncrementScoreBy(dead.Count);
                    dead.Clear();
                    Destroy(gameObject);
                }
            }
        }
    }
}