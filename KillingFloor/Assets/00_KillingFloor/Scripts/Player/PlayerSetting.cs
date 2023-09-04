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
        public float viewXSensitivity;  // X축 민감도
        public float viewYSensitivity;  // Y축 민감도

        public bool viewXInverted;      // X축 시야각
        public bool viewYInverted;      // Y축 시야각

        public float viewClampYMin;
        public float viewClampYMax;
    }
}
