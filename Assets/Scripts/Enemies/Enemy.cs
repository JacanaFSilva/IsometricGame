using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public int maxHealth = 200;
    int currentHealth;
    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start(){
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage){

        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);

        // play hurt animation
        animator.SetTrigger("Hurt");
        
        if(currentHealth <= 0){
            Die();
        }
        
    }

    void Die(){
        Debug.Log("Enemy die");

        // die animation
        animator.SetBool("isDead", true);

        // disable the enemy
        GetComponent<BoxCollider>().enabled = false;
        this.enabled = false;
        
    }
}
