using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [Header("Player Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool dash;

    public bool analogMovement;


    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    // 플레이어 이동 입력
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    // 플레이어 뷰 마우스 델타값 입력
    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }
    // 점프 입력
    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    // 대시 입력
    public void OnDash(InputValue value)
    {
        dashInput(value.isPressed);
    }
    // 카메라 변경
    public void OnChangeCamera(InputValue value)
    {
        ChangeCameraInput(value.isPressed);
    }


    // 인풋을 변환
    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }
    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void dashInput(bool newDashState)
    {
        dash = newDashState;
    }
    public void ChangeCameraInput(bool newCameraState)
    {

    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

}
