using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FlightKit
{
    /// <summary>
    /// Rectangular version of the fuel slider GUI.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FuelProgressBarCircle : MonoBehaviour
    {
        /// <summary>
        /// Texture of the full fuel fill bar.
        /// </summary>
        public Image barFuelFull;
        /// <summary>
        /// Texture of the low fuel fill bar.
        /// </summary>
        public Image barFuelLow;

        /// <summary>
        /// Normalized value where full fuel bar starts to disappear.
        /// </summary>
        [Tooltip("Normalized value where full fuel bar starts to disappear.")]
        [Space] public float blendStart = 0.5f;
        
        /// <summary>
        /// Normalized value where full fuel bar is no longer visible at all.
        /// </summary>
        [Tooltip("Normalized value where full fuel bar is no longer visible at all.")]
        public float blendEnd = 0.2f;

        /// <summary>
        /// Instead of changing fill amount of the bar, the script will change the scale.
        /// </summary>
        [Tooltip("Instead of changing fill amount of the bar, the script will change the scale.")]
        [Space] public bool decreaseByScaling = false;

        protected FuelController _fuelController;
        protected CanvasGroup _canvasGroup;
        protected bool _blinking = false;
        
        private float _blinkStartTime;
        private bool _isBlinking;
        
        void Start()
        {
            _fuelController = GameObject.FindObjectOfType<FuelController>();
            if (_fuelController == null)
            {
                Debug.LogError("FuelController not found.");
                this.enabled = false;
                return;
            }
            
            _canvasGroup = GetComponent<CanvasGroup>();
            
            // Hide itself.
            _canvasGroup.alpha = 0;
        }
        
        void OnEnable()
        {
            TakeOffPublisher.OnTakeOffEvent += FadeIn;
            PauseController.OnPauseEvent += FadeOut;
            PauseController.OnUnPauseEvent += FadeIn;
            RevivePermissionProvider.OnReviveGranted += FadeIn;
            FuelController.OnFuelEmptyEvent += FadeOut;
        }
        
        void OnDisable()
        {
            TakeOffPublisher.OnTakeOffEvent -= FadeIn;
            PauseController.OnPauseEvent -= FadeOut;
            PauseController.OnUnPauseEvent -= FadeIn;
            RevivePermissionProvider.OnReviveGranted -= FadeIn;
            FuelController.OnFuelEmptyEvent -= FadeOut;
        }
        
        private void FadeIn()
        {
            Fader.FadeAlpha(this, _canvasGroup, true, 1);
        }
        
        private void FadeOut()
        {
            Fader.FadeAlpha(this, _canvasGroup, false, 1);
        }
        
        void Update()
        {
            if (decreaseByScaling)
            {
                Vector3 newScale = new Vector3(_fuelController.fuelAmount, _fuelController.fuelAmount, 1f);
                barFuelFull.rectTransform.localScale = newScale;
                barFuelLow.rectTransform.localScale = newScale;
            }
            else
            {
                barFuelFull.fillAmount = _fuelController.fuelAmount;
                barFuelLow.fillAmount = barFuelFull.fillAmount;
            }

            if (_fuelController.fuelAmount < blendStart)
            {
                float alpha = (_fuelController.fuelAmount - blendEnd) / (blendStart - blendEnd);
                barFuelFull.color = new Color(1, 1, 1, alpha);
            }
            
            // Blink continuously when low fuel.
            if (_fuelController.fuelAmount < FuelController.LOW_FUEL_PERCENT) {
                if (!_isBlinking) {
                    _blinkStartTime = Time.time;
                }
                _isBlinking = true;
                _canvasGroup.alpha = Mathf.Clamp01(
                    Mathf.Cos((Time.time - _blinkStartTime) * 10f) * 0.5f + 0.5f + 0.25f);
            } else {
                if (_isBlinking) {
                    _canvasGroup.alpha = 1f;
                }
                _isBlinking = false;
            }
        }
        
    }
}

