using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviourPun
{
    [Header("Player Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool dash;
    public bool shoot;
    public bool aim;
    public bool reload;
    public bool changeCamera;
    public bool weaponSlot1;
    public bool weaponSlot2;
    public bool weaponSlot3;
    public bool weaponSlot4;
    public float scroll;
    public bool grenade;
    public bool equip;
    public bool cancle;
    

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    // 인풋시스템 입력 ================================================

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
 
    // 사격 입력
    public void OnShoot(InputValue value)
    {
        ShootInput(value.isPressed);
    }
    public void OnAim(InputValue value)
    {
        AimInput(value.isPressed);
    }
    // 재장전 입력
    public void OnReload(InputValue value)
    {
        ReloadInput(value.isPressed);
    }
    public void OnGrenade(InputValue value)
    {
        GrenadeInput(value.isPressed);
    }

    // 카메라 변경
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
    public void OnWeaponSlot3(InputValue value)
    {
        WeaponSlot3Input(value.isPressed);
    }
    public void OnWeaponSlot4(InputValue value)
    {
        WeaponSlot4Input(value.isPressed);
    }
    public void OnScroll(InputValue value)
    {
        ScrollInput(value.Get<float>()); ;
    }
    public void OnEquip(InputValue value)
    {
        EquipInput(value.isPressed);
    }
    public void OnCancle(InputValue value)
    {
        CancleInput(value.isPressed);
    }

    // 입력을 변환 ================================================
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

    public void AimInput(bool newAim)
    {
        aim = newAim;
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
    public void WeaponSlot3Input(bool newSlot)
    {
        weaponSlot3 = newSlot;
    }
    public void WeaponSlot4Input(bool newSlot)
    {
        weaponSlot4 = newSlot;
    }
    public void ScrollInput(float newScroll)
    {
        scroll = newScroll;
    }
    public void GrenadeInput(bool newGrenade)
    {
        grenade = newGrenade;
    }
    public void EquipInput(bool newEquip)
    {
        equip = newEquip;
    }
    public void CancleInput(bool newCancle)
    {
        cancle = newCancle;
    }
    // 카메라 마우스 관련 ================================================
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }
    private void SetCursorState(bool newState)
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.

        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

}
