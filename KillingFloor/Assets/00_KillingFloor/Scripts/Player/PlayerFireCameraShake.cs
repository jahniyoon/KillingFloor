using DG.Tweening;
using System;
using UnityEngine;

public class PlayerFireCameraShake : MonoBehaviour
{
    // 사격 카메라 흔들림
    [SerializeField] private Transform _camera;
    [SerializeField] private Vector3 _rotationStrength;

    private void Awake()
    {
        _camera = GetComponent<PlayerMovement>().cinemachineCameraTarget.transform; // 카메라 타겟 가져와주기
    }
    private static event Action Shake;
    public static void Invoke()
    {
        Shake?.Invoke();
    }
    private void OnEnable() => Shake += CameraShake;
    private void OnDisable() => Shake -= CameraShake;

    private void CameraShake()
    {
        _camera.DOComplete();
        _camera.DOShakeRotation(0.3f, _rotationStrength);
    }

}
