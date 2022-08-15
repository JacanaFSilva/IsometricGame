using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    bool comboPossible;
    public int comboStep;
    bool inputSmash;

    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;
    //float nextAttackTime = 0f;
    bool isAtkPressed = false;
    bool isHAtkPressed = false;

    void Awake(){
        // initially set reference variables
        animator = GetComponent<Animator>();
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        playerInput.CharacterControls.lightAttack.started += onAtk;
        playerInput.CharacterControls.lightAttack.canceled += onAtk;
        playerInput.CharacterControls.heavyAttack.started += onHATk;
        playerInput.CharacterControls.heavyAttack.canceled += onHATk;

    }

    void onAtk(InputAction.CallbackContext context){
        isAtkPressed = context.ReadValueAsButton();
        Debug.Log(isAtkPressed);

    }
    void onHATk(InputAction.CallbackContext context){
        isHAtkPressed = context.ReadValueAsButton();
        Debug.Log(isAtkPressed);
    }

    public void ComboPossible(){
        comboPossible = true;
    }

    public void NextAtk(){
        if(!inputSmash){
            if(comboStep == 2)
                animator.Play("attack2");
            if(comboStep == 3)
                animator.Play("attack3");
        }
        if(inputSmash){
            if(comboStep == 1)
                animator.Play("heavyAttack1");
            if(comboStep == 2)
                animator.Play("heavyAttack2");
            if(comboStep == 3)
                animator.Play("heavyAttack3");
        }
    }

    public void ResetCombo(){
        comboPossible = false;
        inputSmash = false;
        comboStep = 0;
    }

    void NormalAttack(){
        if(comboStep == 0){
            animator.Play("attack1");
            comboStep = 1;
            return;
        }
        if(comboStep != 0){
            if(comboPossible){
                comboPossible = false;
                comboStep += 1;
            }
        }
    }

    void SmashAttack(){
        if(comboPossible){
            comboPossible = false;
            inputSmash = true;
        }
    }

    void Update(){
        if(isAtkPressed){
            NormalAttack();
            lightAttack();
        }     
        if(isHAtkPressed){
            SmashAttack();
            lightAttack();
        }
        /*if(Time.time >= nextAttackTime){
            Keyboard kb = InputSystem.GetDevice<Keyboard>();
            if(kb.kKey.wasPressedThisFrame){
                lightAttack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }*/
    }

    void lightAttack(){
        Debug.Log("Atacado");
        // play an attack animation
        //animator.SetTrigger("Attack");
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

    void OnEnable(){
        // enable the character controls action map
        playerInput.CharacterControls.Enable();
    }

    void OnDisable(){
        // disable the character controls action map
        playerInput.CharacterControls.Disable();
    }
}