using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // declare reference variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    // variables to store optimized stter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;

    public Vector2 currentMovementInput;
    public Vector3 currentMovement;
    Vector3 currentRunMovement;

    bool isMovementPressed;
    bool isRunPressed;

    float rotationFactorPerFrame = 7.0f;
    public float runMultiplier = 1.5f;

    // Awake is called earlier than Start in Unity's event life cycle
    void Awake(){

        // initially set reference variables
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

    // set the player input callbacks
        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
    }

    void onRun(InputAction.CallbackContext context){
        isRunPressed = context.ReadValueAsButton();
    }

    void handleRotation(){

        Vector3 positionToLookAt;
        // the change in position our character should point to
        positionToLookAt.x = currentMovement.x; // mexer aqui pra ajeitar a rotação para a diagonal
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        

        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;
        if(isMovementPressed){
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    void onMovementInput (InputAction.CallbackContext context){

        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x; // mexer aqui pra ajeitar a movimentação para a diagonal
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleAnimation(){
        // get parameter values from animator
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        // start walking if movement pressed is true and not already walking
        if (isMovementPressed && !isWalking){

            animator.SetBool(isWalkingHash, true);
        }
        // stop walking if movement pressed is false and alredy walking
        else if (!isMovementPressed && isWalking){

            animator.SetBool(isWalkingHash, false);
        }

    // run if movement and run pressed are true and not currently running
        if((isMovementPressed && isRunPressed) && !isRunning){

            animator.SetBool(isRunningHash, true);
        }

        // stop running if movement or run pressed are false and currently running
        else if ((!isMovementPressed || !isRunPressed) && isRunning){

            animator.SetBool(isRunningHash, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();
        if(isRunPressed){
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else{
            characterController.Move(currentMovement * Time.deltaTime);
        }
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