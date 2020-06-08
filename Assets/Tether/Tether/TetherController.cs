
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tether
{
    /// <summary>
    /// Controller for tethering player to an object or tethering rigidbodies to the player.
    /// </summary>
    public class TetherController : UdonSharpBehaviour
    {
        [Header("Properties")]
        [Tooltip("A TetherProperties script that determines the physical properties of grappling. Can be shared, so that all grapples in a scene share the same properties.")]
        public TetherProperties properties;

        private bool editorMode = true;
        private VRCPlayerApi localPlayer;

        private float tetherInput = 0.0f;

        private bool tethering = false;
        private bool tetheringRigidbody = false;

        private Vector3 tetherPoint = Vector3.zero;
        private Vector3 tetherNormal = Vector3.zero;
        private GameObject tetherObject;
        private Rigidbody tetherRigidbody;

        private float tetherLength;
        private float tetherUnwindRate;

        public void Start()
        {
            localPlayer = Networking.LocalPlayer;

            if (localPlayer != null)
            {
                editorMode = false;
            }
        }

        public void Update()
        {
            if (!editorMode)
            {
                if (tetherInput > properties.tetherInputDeadzone)
                {
                    if (!tethering)
                    {
                        bool detected = false;
                        RaycastHit hit = new RaycastHit();

                        // auto aim in incremental sizes
                        detected = Physics.Raycast(transform.position, transform.forward, out hit, properties.tetherMaximumLength, properties.tetherDetectionMask);
                        if (!detected)
                        {
                            for (int i = properties.tetherDetectionIncrements; detected == false && i > 0; i--)
                            {
                                detected = Physics.SphereCast(transform.position, properties.tetherDetectionSize / i, transform.forward, out hit, properties.tetherMaximumLength, properties.tetherDetectionMask);
                            }
                        }

                        if (detected)
                        {
                            // store tether point as local coords to game object so the gameobject can move and we will stay tethered
                            tetherObject = hit.collider.gameObject;
                            tetherRigidbody = hit.collider.gameObject.GetComponent<Rigidbody>();
                            tetherPoint = tetherObject.transform.InverseTransformPoint(hit.point);
                            tetherNormal = hit.normal;
                            tetherLength = Vector3.Distance(transform.position, hit.point);

                            tethering = true;
                            tetheringRigidbody = properties.manipulatesRigidbodies && tetherRigidbody != null;
                            if (tetheringRigidbody)
                            {
                                Networking.SetOwner(localPlayer, tetherObject);
                            }
                        }
                    }

                    if (tethering)
                    {
                        // unreeling
                        if (properties.allowUnwinding && !IsInputHeld())
                        {
                            tetherUnwindRate = properties.unwindRate * (1.0f - ((tetherInput - properties.tetherInputDeadzone) /  (properties.tetherHoldDeadzone - properties.tetherInputDeadzone)));
                            tetherLength = Mathf.Clamp(tetherLength + tetherUnwindRate * Time.deltaTime, 0.0f, properties.tetherMaximumLength);
                        }

                        Vector3 worldTetherPoint = GetTetherPoint();
                        float distance = Vector3.Distance(transform.position, worldTetherPoint);

                        // we are beyond the tether length, so project our velocity along the invisible sphere and push us back inside
                        if (distance > tetherLength)
                        {
                            if (!tetheringRigidbody || (tetheringRigidbody && properties.playerMass <= tetherRigidbody.mass))
                            {
                                Vector3 normal = worldTetherPoint - transform.position;
                                normal = normal.normalized;

                                Vector3 spring = normal * (distance - tetherLength) * properties.tetherSpringFactor;
                                spring = Vector3.ClampMagnitude(spring * Time.deltaTime, properties.tetherMaximumSpringForce);

                                Vector3 velocity = localPlayer.GetVelocity();
                                Vector3 projected = Vector3.ProjectOnPlane(velocity, normal);
                                localPlayer.SetVelocity(Vector3.MoveTowards(velocity, projected, properties.tetherProjectionRate * Time.deltaTime) + spring);
                            }
                        }
                    }
                }
                else
                {
                    if (tethering)
                    {
                        tethering = false;
                        tetheringRigidbody = false;
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            if (tetheringRigidbody)
            {
                Vector3 worldTetherPoint = GetTetherPoint();

                float distance = Vector3.Distance(transform.position, worldTetherPoint);

                // tether for rigidbodies that weigh less than us
                if (distance > tetherLength)
                {
                    Vector3 normal = transform.position - worldTetherPoint;
                    normal = normal.normalized;

                    Vector3 spring = normal * (distance - tetherLength) * properties.rigidbodySpringFactor * properties.playerMass;
                    spring = Vector3.ClampMagnitude(spring, properties.rigidbodyMaximumSpringForce * properties.playerMass);

                    Vector3 projected = Vector3.ProjectOnPlane(tetherRigidbody.velocity, normal);
                    tetherRigidbody.velocity = Vector3.MoveTowards(tetherRigidbody.velocity, projected, properties.rigidbodyProjectionRate * properties.playerMass * Time.deltaTime);
                    tetherRigidbody.AddForceAtPosition(spring, worldTetherPoint);
                }
            }
        }

        public void OnDisable()
        {
            tethering = false;
            tetheringRigidbody = false;
        }

        /// <summary>
        /// Gets input value as received from an input script.
        /// </summary>
        /// <returns>Input value as float with a deadzone applied.</returns>
        public float GetInput()
        {
            return tetherInput > properties.tetherInputDeadzone ? tetherInput : 0.0f;
        }

        /// <summary>
        /// Sets input value from external source.
        /// </summary>
        /// <param name="value">Float of input value</param>
        public void SetInput(float value)
        {
            tetherInput = value;
        }

        /// <summary>
        /// Whether the tether is locked at its current length.
        /// </summary>
        /// <returns>Boolean state whether the tether is locked at its current length.</returns>
        public bool IsInputHeld()
        {
            return tetherInput > properties.tetherHoldDeadzone;
        }

        /// <summary>
        /// Gets state of tether.
        /// </summary>
        /// <returns>Boolean state if player is tethered to something.</returns>
        public bool GetTethering()
        {
            return tethering;
        }

        /// <summary>
        /// Gets length of tether, from 0.0-1.0.
        /// </summary>
        /// <returns>From 0.0-1.0, the length of the tether proportional to its maximum possible length.</returns>
        public float GetTetherLength()
        {
            if (!tethering)
            {
                return 1.0f;
            }
            return tetherLength / properties.tetherMaximumLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Actual length of tether as float, in meters.</returns>
        public float GetActualTetherLength()
        {
            if (!tethering)
            {
                return 0.0f;
            }
            return tetherLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GameObject GetTetherObject()
        {
            return tetherObject;
        }

        /// <summary>
        /// Gets starting point of tether.
        /// </summary>
        /// <returns>Vector3 of the starting point of the tether.</returns>
        public Vector3 GetTetherStartPoint()
        {
            return transform.position;
        }

        /// <summary>
        /// Gets end point of tether.
        /// </summary>
        /// <returns>Vector3 of the end point the tether is connected to.</returns>
        public Vector3 GetTetherPoint()
        {
            return tetherObject.transform.TransformPoint(tetherPoint);
        }

        /// <summary>
        /// Gets surface normal at tether point. Useful for displaying a particle effect at the collision point.
        /// </summary>
        /// <returns>Vector3 surface normal at the point the tether connected to.</returns>
        public Vector3 GetTetherNormal()
        {
            return tetherNormal;
        }

        /// <summary>
        /// Gets unwinding rate, from 0.0-1.0.
        /// </summary>
        /// <returns>From 0.0-1.0, the speed at which the grapple gun is unwinding to its maximum length.</returns>
        public float GetTetherUnwindRate()
        {
            return tetherUnwindRate == 0.0f ? 0.0f : tetherUnwindRate / properties.unwindRate;
        }
    }
}