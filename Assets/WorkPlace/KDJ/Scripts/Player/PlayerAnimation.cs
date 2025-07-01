using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private bool _isGrabbing => PlayerManager.Instance.SelectItem != null;
    private bool _isMoving => InputManager.Instance.MoveDir != Vector3.zero;

    private void Update()
    {
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        MoveAnim();
        GrabAnim();
        SwingAnim();
        MiningAnim();
    }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void MoveAnim()
    {
        _animator.SetBool("IsWalking", _isMoving);
    }

    public void GrabAnim()
    {
        _animator.SetBool("IsGrabbing", _isGrabbing);
    }

    public void SwingAnim()
    {
        // ���߿� _isGrabbing�� ���ǿ� �߰��ǵ��� ����
        if (Input.GetMouseButtonDown(0) && _isGrabbing && PlayerManager.Instance.SelectItem as MaterialItem)
        {
            _animator.SetTrigger("Swing");
        }
    }

    public void MiningAnim()
    {
        _animator.SetBool("IsMining", InputManager.Instance.IsUsingTool);
    }
}
