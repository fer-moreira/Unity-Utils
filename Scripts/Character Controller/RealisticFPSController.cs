using Pixeye.Unity;
using System.Security.Cryptography;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FFM.Player {
    [RequireComponent(typeof(CharacterController))]
    public class RealisticFPSController : MonoBehaviour {

        [System.Serializable]
        public class LookObject {
            [SerializeField] private float m_LookDelay = 30;
            [SerializeField] private Transform m_Transform;

            Quaternion refRotation;

            public void DampRotation(Quaternion rotation) {
                m_Transform.rotation = QuaternionUtil.SmoothDamp(
                    m_Transform.rotation,
                    rotation,
                    ref refRotation,
                    m_LookDelay / 100
                );
            }
        }

        [System.Serializable]
        public class AnimationCurveObject {
            [SerializeField] private AnimationCurve m_Curve;
            [SerializeField] private float m_Frequency = 1f;
            [SerializeField] private float m_Size = 0.1f;

            public float EvalCurve(float time, float globalFrequency = 1, float globalSize = 1) {
                return (m_Curve.Evaluate((time * m_Frequency) * globalFrequency) * m_Size) * globalSize;
            }
        }


        [Foldout("Physics", true)]
        [SerializeField] private bool m_EnableSprinting;
        [SerializeField] private float m_WalkSpeed = 2;
        [SerializeField] private float m_RunSpeed = 5;

        [Foldout("Headbob", true)]
        [Header("Global")]
        [SerializeField] private Transform m_BobPivot;
        [SerializeField] private float m_BobSpeed = 1;
        [SerializeField] private float m_BobOverallSize = 1;
        [SerializeField] private float m_BobOverallFrequency = 1;
        [SerializeField] private AnimationCurveObject m_YCurve;
        [SerializeField] private AnimationCurveObject m_XCurve;

        [Foldout("Footsteps", true)]
        [SerializeField, Range(0, 1)] private float m_FootstepVolume = 1;
        [SerializeField] private Vector2 m_PitchChangeInterval = new Vector2(0.95f, 1.5f);
        [SerializeField] private AudioSource m_FootstepSource;
        [SerializeField] private AudioClip[] m_FootstepClips;

        [Foldout("Mouse Settings", true)]
        [SerializeField, Range(0, 1)] private float m_MouseGlobalSense = 1;
        [SerializeField, Range(0, 1)] private float m_MouseXSense = 1f;
        [SerializeField, Range(0, 1)] private float m_MouseYSense = 1f;
        [SerializeField] private float m_MouseXLimits = 80f;
        [SerializeField] private bool m_CursorVisible = false;
        [SerializeField] private CursorLockMode m_LockMode = CursorLockMode.Locked;

        [Foldout("Head Transforms", true)]
        [SerializeField] private Transform m_HorizontalTransform;
        [SerializeField] private Transform m_VerticalTransform;
        [SerializeField] private float m_ForwardDistance = 2;

        [Foldout("Follow Components")]
        [SerializeField] private LookObject[] m_FollowObjects;

        #region Privates
        private float mouseX, mouseY;
        private Vector3 movmentDirection;
        private CharacterController controller;

        private float bobStep;
        private float xBob, yBob;
        private Vector3 bobPosition;
        private float previousXBob;
        #endregion

        #region APIs
        public Quaternion LookingRotation => m_VerticalTransform.rotation;
        public Vector3 LookingDirection => m_VerticalTransform.TransformPoint(Vector3.forward * m_ForwardDistance);

        public Vector3 Forward => m_HorizontalTransform.forward;
        public Vector3 Right => m_HorizontalTransform.right;

        public float CurrentSpeed => (
            m_EnableSprinting ? Mathf.Lerp(m_WalkSpeed, m_RunSpeed, Sprinting)
            : m_WalkSpeed
        );

        public float MovmentMagnitudeRaw { get; private set; }
        public float MovmentMagnitude { get; private set; }

        public static RealisticFPSController Instance;
        #endregion

        #region Input System
        public static float Vertical { get; private set; }
        public static float Horizontal { get; private set; }
        public static float MouseDeltaX { get; private set; }
        public static float MouseDeltaY { get; private set; }
        public static float Sprinting { get; private set; }
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction sprintAction;
        #endregion


        private void Awake() {
            controller = GetComponent<CharacterController>();
            Instance = this;
        }

        void SetupInput() {
            void OnMove(InputAction.CallbackContext ctx) {
                var value = ctx.ReadValue<Vector2>();
                Vertical = value.y;
                Horizontal = value.x;
            }

            void OnLook(InputAction.CallbackContext ctx) {
                var value = ctx.ReadValue<Vector2>();
                MouseDeltaX = value.y * -1;
                MouseDeltaY = value.x;
            }

            moveAction = InputSystem.actions.FindAction("Move");
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            lookAction = InputSystem.actions.FindAction("Look");
            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;

            sprintAction = InputSystem.actions.FindAction("Sprint");
            sprintAction.performed += (ctx) => Sprinting = ctx.ReadValue<float>();
            sprintAction.canceled += (ctx) => Sprinting = ctx.ReadValue<float>();
        }

        private void Start() {
            Cursor.visible = m_CursorVisible;
            Cursor.lockState = m_LockMode;
            m_FootstepSource.volume = m_FootstepVolume;

            SetupInput();
        }

        private void UpdateHead() {
            mouseY += (MouseDeltaY * m_MouseYSense) * m_MouseGlobalSense;

            mouseX += (MouseDeltaX * m_MouseXSense) * m_MouseGlobalSense;
            mouseX = Mathf.Clamp(mouseX, -m_MouseXLimits, m_MouseXLimits);

            m_HorizontalTransform.localRotation = Quaternion.Euler(0, mouseY, 0);
            m_VerticalTransform.localRotation = Quaternion.Euler(mouseX, 0, 0);
        }

        private void UpdateFollowObjects() {
            foreach (var item in m_FollowObjects) {
                item.DampRotation(LookingRotation);
            }
        }

        private void UpdatePhysics() {
            movmentDirection = (
                (Forward * Vertical) +
                (Right * Horizontal)
            ) * CurrentSpeed;

            MovmentMagnitudeRaw = (movmentDirection.magnitude / CurrentSpeed);

            MovmentMagnitude = Mathf.Lerp(
                MovmentMagnitude,
                MovmentMagnitudeRaw,
                10f / 100f
            );

            controller.SimpleMove(movmentDirection);
        }

        private void UpdateHeadbob() {
            if (MovmentMagnitudeRaw > 0) {
                bobStep += Time.deltaTime * m_BobSpeed;

                xBob = m_XCurve.EvalCurve(bobStep, m_BobOverallFrequency, m_BobOverallSize);
                yBob = m_YCurve.EvalCurve(bobStep, m_BobOverallFrequency, m_BobOverallSize);

                bobPosition = new Vector3(xBob, yBob, 0);

                if (Mathf.Sign(previousXBob) != Mathf.Sign(xBob)) {
                    m_FootstepSource.clip = m_FootstepClips[Random.Range(0, m_FootstepClips.Length)];
                    m_FootstepSource.pitch = Random.Range(m_PitchChangeInterval.x, m_PitchChangeInterval.y);
                    m_FootstepSource.Play();
                }
            }

            previousXBob = xBob;

            m_BobPivot.localPosition = Vector3.Lerp(Vector3.zero, bobPosition, MovmentMagnitude);
        }

        private void Update() {
            UpdateHead();
            UpdateFollowObjects();
            UpdateHeadbob();
            UpdatePhysics();
        }

        public void SetPose(Vector3 position, Quaternion rotation) {
            controller.enabled = false;
            transform.position = position;
            transform.rotation = rotation;
            controller.enabled = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(LookingDirection, 0.1f);
        }
#endif
    }
}
