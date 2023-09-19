using DG.Tweening;
using System;
using UnityEngine;

public class PlayerFireCameraShake : MonoBehaviour
{
    // 사격 카메라 흔들림
    [SerializeField] private Transform _camera;
    [SerializeField] private Vector3 _rotationStrength;
    private Vector3 _shakeRotation;

    private PlayerInputs input;

    private void Awake()
    {
        _camera = GetComponent<PlayerMovement>().transform.GetChild(0); // 움직일 타겟 가져와주기 : 플레이어 하위의 홀더
        input = GetComponent<PlayerInputs>();
    }
    private static event Action Shake;

    private void Update()
    {
        // 조준상태이면 살짝 흔들리게하고
        if (input.aim)
        {
            _shakeRotation = new Vector3(0.3f, 0.3f, 0);
        }
        else
            _shakeRotation = _rotationStrength;
    }
    public static void Invoke()
    {
        Shake?.Invoke();
    }
    private void OnEnable() => Shake += CameraShake;
    private void OnDisable() => Shake -= CameraShake;

    private void CameraShake()
    {
        _camera.DOComplete();
        _camera.DOShakeRotation(0.3f, _shakeRotation);
    }

}
