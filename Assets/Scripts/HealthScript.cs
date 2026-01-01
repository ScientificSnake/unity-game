using System;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float Health;
    public Action OnDeath;
    public Action OnDamage;

    public void ApplyDamage(float damage)
    {
        Health -= damage;
        HealthCheck();

        if (OnDamage != null)
        {
            OnDamage();
        }
    }

    private void HealthCheck()
    {
        if (Health <= 0)
        {
            if (OnDeath != null)
            {
                OnDeath();
            }
            else
            {
                print($"No custom on death assigned falling back to regular destruction");
                BasicDestroy();
            }
        }
    }

    public void BasicDestroy()
    {
        Destroy(gameObject);
    }
}
