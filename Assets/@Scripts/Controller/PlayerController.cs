using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 _curMovementInput;
    [SerializeField] private float jumpForce = 500f;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Look")]
    [SerializeField] private Transform cameraContainer;
    private float minXLook = -85f;
    private float maxXLook = 85f;
    private float camCurXRot;
    private float lookSensitivity = 0.2f;

    [Header("Throw")]
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwCooldown = 1f;
    private float lastThrowTime;

    private bool canMove = true;
    private bool isStuned = false;
    private bool wasStuned = false;
    private bool slide = false;
    private float pushForce;
    [SerializeField] private float gravity = 9.8f;
    private Vector3 pushDir;

    public Vector3 checkPoint = new Vector3(0f, 4f, 0f);

    private Vector2 mouseDelta;

    [HideInInspector]
    [SerializeField] private bool canLook = true;

    private Rigidbody _rigidbody;
    private Animator _animator;

    public Vector2 CurMovementInput
    {
        get { return _curMovementInput; }
        set { _curMovementInput = value; }
    }

    public float MoveSpeed // 플레이어 속력
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public float JumpForce // 플레이어 점프 파워
    {
        get { return jumpForce; }
        set { jumpForce = value; }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        checkPoint = transform.position;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
        // 넉백
        else
        {
            _rigidbody.velocity = pushDir * pushForce;
        }
        _rigidbody.AddForce(new Vector3(0, -gravity * _rigidbody.mass, 0));

    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * _curMovementInput.y + transform.right * _curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (IsGrounded())
            {
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
                //_animator.SetTrigger("Jump");
            }
        }
    }

    public void OnThrowInput(InputAction.CallbackContext context)
    {
        // 쿨타임 체크
        if (Time.time - lastThrowTime >= throwCooldown)
        {
            // 쿨타임 초기화
            lastThrowTime = Time.time;

            _animator.SetTrigger("Throw");

            // 플레이어 기준 공 발사 위치
            Vector3 throwPosition = transform.position + transform.forward * 0.8f + transform.right * 0.7f + Vector3.up * 1.2f;

            // 오브젝트 풀링 사용하면 더 좋음
            GameObject throwableInstance = Instantiate(throwablePrefab, throwPosition, Quaternion.identity);
            Rigidbody throwRigidbody = throwableInstance.GetComponent<Rigidbody>();

            throwRigidbody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f) , Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void LoadCheckPoint()
    {
        transform.position = checkPoint;
    }

    public void HitPlayer(Vector3 velocityF, float time)
    {
        _rigidbody.velocity = velocityF;

        pushForce = velocityF.magnitude;
        pushDir = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }

    private IEnumerator Decrease(float value, float duration)
    {
        if (isStuned)
            wasStuned = true;
        isStuned = true;
        canMove = false;

        float delta = 0;
        delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
            if (!slide) // 땅이 slide가 아니면 감소된 pushForce 적용
            {
                pushForce = pushForce - Time.deltaTime * delta;
                pushForce = pushForce < 0 ? 0 : pushForce;
                //Debug.Log(pushForce);
            }
            _rigidbody.AddForce(new Vector3(0, -gravity * _rigidbody.mass, 0));
        }

        if (wasStuned)
        {
            wasStuned = false;
        }
        else
        {
            isStuned = false;
            canMove = true;
        }
    }
}