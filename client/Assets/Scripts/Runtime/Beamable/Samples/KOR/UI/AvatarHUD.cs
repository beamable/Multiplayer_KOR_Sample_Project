using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Beamable.Samples.KOR.UI
{
    public class AvatarHUD : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text aliasText;

        [SerializeField]
        private TMP_Text healthText;

        [SerializeField]
        private float onScreenTextSize = 0.1f;

        [SerializeField]
        private float onScreenTextDistance = 0.1f;

        [SerializeField]
        private Color healthLeftColor = Color.red;

        [SerializeField]
        private Color healthLostColor = Color.black;

        [SerializeField]
        private Vector3 parentCenterOffset = new Vector3(0.0f, 1.3f, 0.0f);

        private Camera _mainCamera = null;
        private string _healthLeftColorString = null;
        private string _healthLostColorString = null;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _healthLeftColorString = "<color=#" + ColorUtility.ToHtmlStringRGBA(healthLeftColor) + ">";
            _healthLostColorString = "<color=#" + ColorUtility.ToHtmlStringRGBA(healthLostColor) + ">";
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(_mainCamera.transform.forward, _mainCamera.transform.up);
            float dCamera = Vector3.Distance(_mainCamera.transform.position, transform.parent.position + parentCenterOffset); ;
            float fov = _mainCamera.fieldOfView * Mathf.Deg2Rad;
            float scale = 2.0f * dCamera * Mathf.Tan(fov * 0.5f);
            float textScale = scale * onScreenTextSize;
            transform.localScale = new Vector3(textScale, textScale, textScale);

            Vector3 screenPos = _mainCamera.WorldToScreenPoint(transform.parent.position + parentCenterOffset);
            screenPos += new Vector3(0.0f, _mainCamera.pixelHeight * onScreenTextDistance, 0.0f);
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            transform.position = worldPos;
        }

        public void SetAlias(string newAlias)
        {
            aliasText.text = newAlias;
        }

        public void SetHealth(int newHealth)
        {
            const int totalBlockCount = 10;
            int blocksLeft = Mathf.CeilToInt(((float)newHealth / 100.0f) * (float)totalBlockCount);
            string filledBlocks = new string('-', blocksLeft);
            string emptyBlocks = new string('-', totalBlockCount - blocksLeft);
            healthText.text = string.Format("{0}{1}{2}{3}", _healthLeftColorString, filledBlocks, _healthLostColorString, emptyBlocks);
        }
    }
}