using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public Animator animator;

    public int maxHealth = 100;
    public int currentHealth;

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

        Debug.Log("acertado");
        
        if(currentHealth <= 0){
            Die();
        }
        
    }

    void Die(){
        Debug.Log("Tu morreu");

        // die animation
        //animator.SetBool("isDead", true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   
    }
}
