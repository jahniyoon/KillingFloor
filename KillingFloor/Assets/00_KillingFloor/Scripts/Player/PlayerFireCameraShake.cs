using DG.Tweening;
using System;
using UnityEngine;

public class PlayerFireCameraShake : MonoBehaviour
{
    // ��� ī�޶� ��鸲
    [SerializeField] private Transform _camera;
    [SerializeField] private Vector3 _rotationStrength;

    private void Awake()
    {
        _camera = GetComponent<PlayerMovement>().cinemachineCameraTarget.transform; // ī�޶� Ÿ�� �������ֱ�
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
