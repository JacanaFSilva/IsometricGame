using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDamage : MonoBehaviour
{
    public int attackDamage = 20;
    public int enragedAttackDamage = 40;

    public Transform attackPoint;

    public float attackRange = 1f;
    public LayerMask attackMask;

    public void Attack(){
        Collider[] colInfo = Physics.OverlapSphere(attackPoint.position, attackRange, attackMask);

        foreach(Collider enemy in colInfo){

            enemy.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            Debug.Log("Machucado" + enemy.name);
        }
    }

    void OnDrawGizmosSelected()
	{
        if(attackPoint == null)
        return; 

		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	}

}
