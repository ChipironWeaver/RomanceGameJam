using DG.Tweening;
using UnityEngine;

public class SettingSlide : MonoBehaviour
{
    [SerializeField] private Vector2 _hiddenPosition;
    [SerializeField] private float _moveTime = 0.5f;
    [SerializeField] private Ease _moveEase;

    private bool _isMoving;
    private bool _isHidden = true;

    public void SetActive(bool active)
    {
        if (active == !_isHidden | (_isMoving)) return;
        _isHidden = !active;
        _isMoving = true;
        transform.DOLocalMove(active? Vector2.zero : _hiddenPosition, _moveTime).SetEase(_moveEase).OnComplete((() =>  { _isMoving = false; }));
    }
}
