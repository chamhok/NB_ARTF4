using UnityEngine;

/// <summary>
/// 게임 내의 개별 아이템을 표현하는 클래스입니다.
/// </summary>
/// 

public class Item : MonoBehaviour
{
    #region Field
    [SerializeField] private int _id; // 아이디
    [SerializeField] private string _name; //이름
    [SerializeField] private string _category; //속성
    [SerializeField] private string _description; // 설명
    private bool _isActive; // 활성상태
    [SerializeField] private float _power; // 아이템이 주는 효과
    [SerializeField] private float _duration; // 아이템 지속시간
    #endregion
    #region Initialization
    public void Initialize(int id, string name, string category, string description, float power, float duration)
    {
        _id = id;
        _name = name;
        _category = category;
        _description = description;
        _power = power;
        _duration = duration;
        _isActive = false; // 기본적으로 아이템은 비활성 상태로 시작
    }
    #endregion
    #region Property
    /// <summary>
    /// 아이템의 고유 식별자를 반환합니다.
    /// </summary>
    public int Id => _id;

    /// <summary>
    /// 아이템의 이름을 반환합니다.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// 아이템의 카테고리를 반환합니다.
    /// </summary>
    public string Category => _category;

    /// <summary>
    /// 아이템에 대한 설명을 반환합니다.
    /// </summary>
    public string Description => _description;

    /// <summary>
    /// 아이템이 주는  효과를 반환합니다.
    /// </summary>
    public float Power => _power;

    /// <summary>
    /// 아이템의 효과가 지속되는 시간입니다.
    /// </summary>
    /// <returns></returns>
    public float Duration
    {
        get => _duration;
        set
        {
            _duration = value;
        }
    }

    /// <summary>
    /// 아이템의 활성상태를 반환합니다.
    /// </summary>
    public bool IsActivate
    {
        get => _isActive;
        set
        {
            _isActive = value;
        }
    }
    #endregion
}