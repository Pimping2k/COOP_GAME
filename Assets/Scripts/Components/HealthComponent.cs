using UnityEngine;

namespace Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;

        private void Awake()
        {
            health = maxHealth;
        }

        public float MaxHealth
        {
            get => maxHealth;
            set => maxHealth = Mathf.Max(0, value);
        }

        public float Health
        {
            get => health;
            set => health = Mathf.Clamp(value, 0, maxHealth);
        }

        public float IncreaseHealth(float amount)
        {
            health = Mathf.Clamp(health + amount, 0, maxHealth);
            return health;
        }

        public float DecreaseHealth(float amount)
        {
            health = Mathf.Clamp(health - amount, 0, maxHealth);
            return health;
        }

        public float IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
            if (health > maxHealth)
                health = maxHealth;
            return maxHealth;
        }

        public float DecreaseMaxHealth(float amount)
        {
            maxHealth = Mathf.Max(0, maxHealth - amount);
            health = Mathf.Clamp(health, 0, maxHealth);
            return maxHealth;
        }
    }
}