using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;

    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;
    float nextAttackTime = 0f;

    void Update(){
        if(Time.time >= nextAttackTime){
            Keyboard kb = InputSystem.GetDevice<Keyboard>();
            if(kb.kKey.wasPressedThisFrame){
                lightAttack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void lightAttack(){
        Debug.Log("Atacado");
        
        // play an attack animation
        animator.SetTrigger("Attack");

        // detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
    
        // damage then
        foreach(Collider enemy in hitEnemies){

            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            Debug.Log("Machucado" + enemy.name);
        }
    }

    void OnDrawGizmosSelected(){
        if(attackPoint == null)
        return; 

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}