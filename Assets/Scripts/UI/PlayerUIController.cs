using System;
using Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerScripts
{
    public class PlayerUIController : MonoBehaviour
    {
        [SerializeField] private Image staminaBar;
        [SerializeField] private StaminaComponent _staminaComponent;
        
        private void Update()
        {
            staminaBar.fillAmount = _staminaComponent.Stamina / _staminaComponent.MaxStamina;
        }
    }
}