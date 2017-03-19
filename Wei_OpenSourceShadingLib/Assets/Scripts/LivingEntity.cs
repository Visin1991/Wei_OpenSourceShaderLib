using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visin1_1
{
    public class LivingEntity : MonoBehaviour, IDamageable
    {

        public float maxHealth = 100;
        public float health;
        protected bool dead;

        public virtual void Start()
        {
            health = maxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            health -= damage;
            Debug.Log("I Took Damage");
            if (health <= 0 && !dead)
            {
                //AudioManager.instance.PlayerSound("WhatEverSourceDic",transform.position);
                Die();
                return;
            }
            //AudioManager.instance.PlaySound("Impact",transform.position);
        }

        protected virtual void Die()
        {
            dead = true;
        }
    }
}
