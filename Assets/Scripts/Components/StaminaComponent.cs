using System;
using System.Collections;
using UnityEngine;

namespace Components
{
    public class StaminaComponent : MonoBehaviour
    {
        [SerializeField] private float stamina = 100f;
        [SerializeField] private float maxStamina = 100f;

        private Coroutine regenerationCoroutine;

        private void Awake()
        {
            stamina = maxStamina;
        }

        public float Stamina
        {
            get => stamina;
            set => stamina = value;
        }

        public float MaxStamina
        {
            get => maxStamina;
            set => maxStamina = value;
        }

        public float IncreaseStamina(float amount)
        {
            stamina = Mathf.Clamp(stamina + amount, 0, maxStamina);
            return stamina;
        }

        public float DecreaseStamina(float amount)
        {
            stamina = Mathf.Clamp(stamina - amount, 0, maxStamina);
            return stamina;
        }

        public float IncreaseMaxStamina(float amount)
        {
            maxStamina += amount;
            if (stamina > maxStamina)
                stamina = maxStamina;
            return stamina;
        }

        public float DecreaseMaxStamina(float amount)
        {
            maxStamina = Mathf.Max(0, maxStamina - amount);
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            return maxStamina;
        }

        private IEnumerator RegenStamina()
        {
            while (stamina < maxStamina)
            {
                stamina += 1f * Time.deltaTime;
                yield return null;
            }

            regenerationCoroutine = null;
        }

        public void StartRegeneration()
        {
            if (regenerationCoroutine == null)
            {
                regenerationCoroutine = StartCoroutine(RegenStamina());
            }
        }

        public void StopRegeneration()
        {
            if (regenerationCoroutine != null)
            {
                StopCoroutine(regenerationCoroutine);
                regenerationCoroutine = null;
            }
        }
    }
}