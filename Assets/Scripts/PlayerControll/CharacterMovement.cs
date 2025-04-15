using UnityEngine;

namespace Script.Input_System
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        //[SerializeField] private CameraController cameraController;

        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float rotationSmoothSpeed = 10f;
        [SerializeField] private float timeChangeState = 0.1f;
        [SerializeField] private float gravity = 9.81f;

        private MenuInputHandler _inputHandler;
        private Vector3 _verticalVelocity;
        private bool _isGrounded;

        public bool isLockedOn { get; set; }
        public Transform lockOnTarget { get; set; }

        public void Initialize(MenuInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            _isGrounded = _characterController.isGrounded;

            if (_isGrounded && _verticalVelocity.y < 0)
            {
                _verticalVelocity.y = -0.5f;
            }
            else
            {
                _verticalVelocity.y -= gravity * Time.deltaTime;
            }

            Vector2 movementInput = _inputHandler.moveInput;
            bool isSprinting = _inputHandler.isSprinting;

            Vector3 horizontalMovement = Vector3.zero;

            if (isLockedOn && lockOnTarget != null)
            {
                // Xoay nhân vật về phía mục tiêu
                Vector3 directionToTarget = (lockOnTarget.position - transform.position).normalized;
                directionToTarget.y = 0f;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);

                // Di chuyển tương đối với hướng camera
                //Vector3 cameraForward = cameraController.GetCameraForwardDirection();
                //Vector3 cameraRight = cameraController.transform.right;
                //cameraForward.y = 0f;
                //cameraRight.y = 0f;
                //cameraForward.Normalize();
                //cameraRight.Normalize();

                //Vector3 moveDirection = (cameraForward * movementInput.y + cameraRight * movementInput.x).normalized;
                //float currentSpeed = isSprinting ? runSpeed : walkSpeed;
                //horizontalMovement = moveDirection * currentSpeed * Time.deltaTime;
            }
            else if (movementInput.magnitude > 0.1f)
            {
                // Di chuyển bình thường
                //Vector3 cameraForward = cameraController.GetCameraForwardDirection();
                //Vector3 cameraRight = cameraController.transform.right;
                //cameraForward.y = 0f;
                //cameraRight.y = 0f;
                //cameraForward.Normalize();
                //cameraRight.Normalize();

                //Vector3 moveDirection = (cameraForward * movementInput.y + cameraRight * movementInput.x).normalized;
                //Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);

                //float currentSpeed = isSprinting ? runSpeed : walkSpeed;
                //horizontalMovement = moveDirection * currentSpeed * Time.deltaTime;
            }

            Vector3 finalMovement = horizontalMovement + _verticalVelocity * Time.deltaTime;
            _characterController.Move(finalMovement);

            UpdateAnimator(movementInput, isSprinting);
        }

        private void UpdateAnimator(Vector2 movement, bool isSprinting)
        {
            bool isMoving = movement.magnitude > 0.1f;
            float moveTarget = isMoving ? 1f : 0f;
            float moveManagerTarget = isSprinting ? 1f : 0f;
            float sprintTarget = isSprinting ? 1f : 0f;

            float currentMove = _animator.GetFloat("move");
            float currentMoveManager = _animator.GetFloat("moveManager");
            float currentSprint = _animator.GetFloat("sprint");

            float moveDamped = Mathf.MoveTowards(currentMove, moveTarget, Time.deltaTime / timeChangeState);
            float moveManagerDamped = Mathf.MoveTowards(currentMoveManager, moveManagerTarget, Time.deltaTime / timeChangeState);
            float sprintDamped = Mathf.MoveTowards(currentSprint, sprintTarget, Time.deltaTime / timeChangeState);

            moveDamped = Mathf.Clamp01(moveDamped);
            moveManagerDamped = Mathf.Clamp01(moveManagerDamped);
            sprintDamped = Mathf.Clamp01(sprintDamped);

            _animator.SetFloat("move", moveDamped);
            _animator.SetFloat("moveManager", moveManagerDamped);
            _animator.SetFloat("sprint", sprintDamped);
        }
    }
}