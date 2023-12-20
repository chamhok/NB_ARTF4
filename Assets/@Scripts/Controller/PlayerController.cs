using System.Collections;
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
    public static PlayerController instance;
    private static readonly int Throw = Animator.StringToHash("Throw");

    private Item _item;

    public Vector2 CurMovementInput
    {
        get { return _curMovementInput; }
        set { _curMovementInput = value; }
    }

    private void Awake()
    {
        instance = this;
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

            _animator.SetTrigger(Throw);

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
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.1f) , Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+ (Vector3.up * 0.1f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.1f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.1f), Vector3.down),
        };

        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(rays[i], out hit, 0.2f, groundLayerMask))
            {
                Debug.Log(hit.transform.gameObject.name);
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.1f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.1f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.1f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.1f), Vector3.down);
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

    /// <summary>
    /// 아이템의 효과를 플레이어에 적용하거나 제거하는 메서드
    /// apply가 true이면 효과를 적용하고, false이면 효과를 제거합니다.
    /// </summary>
    /// <param name="item">효과를 적용할 아이템</param>
    /// <param name="apply">효과를 적용할 것인지 불값을 받습니다.</param>
    private void UpdatePlayerWithItemEffect(Item item, bool apply)
    {
        float factor = apply ? item.Power : 1 / item.Power;

        switch (item.Id)
        {
            case 0:
                // Main.PlayerControl.SetMoveSpeed(_player.GetSpeed() * factor); 
                break;
            case 1:
                // Main.PlayerController.SetJumpForce(_player.GetJumpForc() * factor);
                break;
        }
    }

    /// <summary>
    ///  아이템의 효과를 플레이어에게 적용하는 메서드
    /// </summary>
    /// <param name="item">적용할 아이템</param>
    private void ApplyItemsEffectToPlayer(Item item)
    {
        UpdatePlayerWithItemEffect(item, true);
    }

    /// <summary>
    ///  아이템의 효과를 원래 대로 되돌리는 메서드
    /// </summary>
    /// <param name="item">적용할 아이템</param>
    public void RemoveItemsEffectFromPlayer(Item item)
    {
        UpdatePlayerWithItemEffect(item, false);
    }
    public void ActivateItem(Item item)
    {
        // 아이템을 활성화합니다.
        item.IsActivate = true;
        ApplyItemsEffectToPlayer(item);
        Debug.Log($"{item.Name} 아이템이 활성화되었습니다.");
        this._item = item;
        // DeactivateItem 메서드를 3초 후에 호출합니다.
        Invoke("DeactivateItem", 3f);
    }

    public void DeactivateItem()
    {
        // 아이템을 비활성화합니다.
        _item.IsActivate = false;
        Debug.Log($"{_item.Name} 아이템이 비활성화되었습니다.");

        // 아이템을 목록에서 제거합니다.
        Main.Item.RemoveItem(_item);
    }

    public void StartActivateItem(Item item)
    {
        Item crruentItem = item;
        Debug.Log("dddddd");
        ActivateItem(crruentItem);
    }
}