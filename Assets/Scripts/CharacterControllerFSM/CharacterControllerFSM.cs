using Demo.Helpers;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demo.CharacterControllerFSM
{
    [RequireComponent(typeof(UnityEngine.CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    
    public class CharacterControllerFSM : CharacterControllerFSMBase
    {
        #region Fields ThirdPersonController
        
        [SerializeField] private AnimationEventReceiver _animationEventReceiver;
        
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Tooltip("How fast character is flying")]
        [Range(0f, 20f)]
        private float _flyingSpeed = 2f;
        
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _mouseCameraSpeedMultiplier = 2f;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        protected float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif 
        public Animator Animator;
        private UnityEngine.CharacterController _controller;
        protected StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        
        #endregion
        
        
        
        #region Properties
        
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }
        
        public StarterAssetsInputs Input
        {
            get => _input;
            set => _input = value;
        }

        public float VerticalVelocity
        {
            get => _verticalVelocity;
            set => _verticalVelocity = value;
        }

        public float FlyingSpeed
        {
            get => _flyingSpeed;
            set => _flyingSpeed = value;
        }

        #endregion
        
        
        #region Properties

        public Animator MyAnimator
        {
            get => Animator;
        }

        public float TargetRotation
        {
            get => _targetRotation;
            set => _targetRotation = value;
        }

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public float FallTimeoutDelta
        {
            get => _fallTimeoutDelta;
            set => _fallTimeoutDelta = value;
        }

        public int AnimIDJump => _animIDJump;

        public int AnimIDFreeFall
        {
            get => _animIDFreeFall;
            set => _animIDFreeFall = value;
        }

        public float JumpTimeoutDelta
        {
            get => _jumpTimeoutDelta;
            set => _jumpTimeoutDelta = value;
        }

        public CharacterController Controller
        {
            get => _controller;
            set => _controller = value;
        }

        public float AnimationBlend
        {
            get => _animationBlend;
            set => _animationBlend = value;
        }

        public GameObject MainCamera
        {
            get => _mainCamera;
            set => _mainCamera = value;
        }

        public float RotationVelocity
        {
            get => _rotationVelocity;
            set => _rotationVelocity = value;
        }

        public int AnimIDSpeed
        {
            get => _animIDSpeed;
            set => _animIDSpeed = value;
        }

        public int AnimIDMotionSpeed
        {
            get => _animIDMotionSpeed;
            set => _animIDMotionSpeed = value;
        }

        #endregion
        
        
        
        #region Unity LifeCycle
        
        
        private void Awake()
        {
            _stateFactory = new CharacterControllerFSMStateFactory(this);
            _currentState = _stateFactory.Grounded();
            _currentState.EnterState();
            
            // get a reference to our main camera
            if (MainCamera == null)
            {
                MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _animationEventReceiver.OnLanded += AnimationEventReceiver_OnLanded;
            _animationEventReceiver.OnFootStep += AnimationEventReceiver_OnFootstepEvent;
        }

        private void OnDisable()
        {
            _animationEventReceiver.OnLanded -= AnimationEventReceiver_OnLanded;
            _animationEventReceiver.OnFootStep -= AnimationEventReceiver_OnFootstepEvent;
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = true; //TryGetComponent(out Animator);
            
            Controller = GetComponent<UnityEngine.CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            
            AssignAnimationIDs();

            // reset our timeouts on start
            JumpTimeoutDelta = JumpTimeout;
            FallTimeoutDelta = FallTimeout;
        }

        void Update()
        {
            if (_currentState != null)
            {
                _currentState.UpdateState();
            }
            
            JumpAndGravity();
            GroundedCheck();
        }
        
        

        private void LateUpdate()
        {
            CameraRotation();
        }

        #endregion

        
        #region Methods State Machine
        
        #endregion



        #region CharacterController
        
        public void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(Controller.velocity.x, 0.0f, Controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                Speed = Mathf.Round(Speed * 1000f) / 1000f;
            }
            else
            {
                Speed = targetSpeed;
            }

            AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (AnimationBlend < 0.01f) AnimationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  MainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, TargetRotation, 0.0f) * Vector3.forward;

            // move the player
            Controller.Move(targetDirection.normalized * (Speed * Time.deltaTime) + new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

            // update animator
            Animator.SetFloat(AnimIDSpeed, AnimationBlend);
            Animator.SetFloat(AnimIDMotionSpeed, inputMagnitude);
        }
        
        private void AssignAnimationIDs()
        {
            AnimIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

//             Debug.Log($"Grounded {Grounded}");

            // update animator if using character
            Animator.SetBool(_animIDGrounded, Grounded);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * _mouseCameraSpeedMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * _mouseCameraSpeedMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
        
        private void JumpAndGravity()
        {
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        #endregion



        #region Callbacks

        private void AnimationEventReceiver_OnLanded(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
        
        private void AnimationEventReceiver_OnFootstepEvent(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Controller.center), FootstepAudioVolume);
                }
            }
        }

        #endregion
    }
    
}
