using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PlayerSetting
{
    [Serializable]

    public class PlayerSettingsModel
    {
        [Header("ViewSetting")]
        public float viewXSensitivity;  // X�� �ΰ���
        public float viewYSensitivity;  // Y�� �ΰ���

        public bool viewXInverted;      // X�� �þ߰�
        public bool viewYInverted;      // Y�� �þ߰�

        public float viewClampYMin;
        public float viewClampYMax;
    }
}
