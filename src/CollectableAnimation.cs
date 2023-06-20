using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class CollectableAnimation : MonoBehaviour
    {
        public bool isAnimated = false;

        public bool isRotating = false;
        public bool isBobbing = false;
        public bool isScaling = false;

        [SerializeField] private Vector3 rotationAngle;
        public float rotationSpeed = 10f;

        [SerializeField] private float floatSpeed = 0.001f;
        private readonly float floatDistanceDelta = 0.3f;
        private bool ascending = true;

        [SerializeField] private Vector3 startScale;
        [SerializeField] private Vector3 endScale;

        [SerializeField] private float scaleSpeed = 1f;
        [SerializeField] private float scaleRate = 1f;
        private bool scalingUp = true;
        private float scaleTimer;

        private float initialY;

        private void Start()
        {
            initialY = transform.position.y;
        }

        private void Update()
        {
            var deltaT = Time.deltaTime;

            if (deltaT > 0) // Do not animate on pause
            {
                if (isAnimated)
                {
                    if (isRotating)
                    {
                        transform.Rotate(deltaT * rotationSpeed * rotationAngle);
                    }

                    if (isBobbing)
                    {
                        Vector3 moveDir = new(0.0f, floatSpeed, 0.0f);
                        transform.Translate(moveDir);

                        if (ascending && transform.position.y >= initialY + floatDistanceDelta)
                        {
                            ascending = false;
                            floatSpeed = -floatSpeed;
                        }
                        else if (!ascending && transform.position.y <= initialY - floatDistanceDelta)
                        {
                            ascending = true;
                            floatSpeed = -floatSpeed;
                        }
                    }

                    if (isScaling)
                    {
                        scaleTimer += deltaT;

                        if (scalingUp)
                        {
                            transform.localScale = Vector3.Lerp(transform.localScale, endScale, scaleSpeed * deltaT);
                        }
                        else if (!scalingUp)
                        {
                            transform.localScale = Vector3.Lerp(transform.localScale, startScale, scaleSpeed * deltaT);
                        }

                        if (scaleTimer >= scaleRate)
                        {
                            if (scalingUp) 
                            { 
                                scalingUp = false; 
                            }
                            else if (!scalingUp) 
                            { 
                                scalingUp = true; 
                            }
                            scaleTimer = 0;
                        }
                    }
                }
            }    
        }
    }
}