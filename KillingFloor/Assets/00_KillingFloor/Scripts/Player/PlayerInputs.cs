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
    public bool shoot;
    public bool reload;
    public bool changeCamera;
    public bool weaponSlot1;
    public bool weaponSlot2;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    // ��ǲ�ý��� �Է� ================================================

    // �÷��̾� �̵� �Է�
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    // �÷��̾� �� ���콺 ��Ÿ�� �Է�
    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }
    // ���� �Է�
    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    // ��� �Է�
    public void OnDash(InputValue value)
    {
        dashInput(value.isPressed);
    }
 
    // ��� �Է�
    public void OnShoot(InputValue value)
    {
        ShootInput(value.isPressed);
    }
    // ������ �Է�
    public void OnReload(InputValue value)
    {
        ReloadInput(value.isPressed);
    }

    // ī�޶� ����
    public void OnChangeCamera(InputValue value)
    {
        ChangeCameraInput(value.isPressed);
    }

    public void OnWeaponSlot1(InputValue value)
    {
        WeaponSlot1Input(value.isPressed);
    }
    public void OnWeaponSlot2(InputValue value)
    {
        WeaponSlot2Input(value.isPressed);
    }

    // �Է��� ��ȯ ================================================
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
   
    public void ShootInput(bool newShoot)
    {
        shoot = newShoot;
    }
    public void ReloadInput(bool newReload)
    {
        reload = newReload;
    }
    public void ChangeCameraInput(bool newCameraState)
    {
        changeCamera = newCameraState;
    }
    public void WeaponSlot1Input(bool newSlot)
    {
        weaponSlot1 = newSlot;
    }
    public void WeaponSlot2Input(bool newSlot)
    {
        weaponSlot2 = newSlot;
    }
    // ī�޶� ���콺 ���� ================================================
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }
    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

}
