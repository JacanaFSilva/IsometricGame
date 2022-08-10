using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    // declare reference variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    // variables to store optimized stter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;

    //variables to store player input values
    public Vector2 currentMovementInput;
    public Vector3 currentMovement;
    public Vector3 currentRunMovement;
    public Vector3 appliedMovement;
    bool isMovementPressed;
    bool isRunPressed;

    //constants
    public float rotationFactorPerFrame = 18.0f;
    public float runMultiplier = 3.5f;
    //int zero = 0;

    //gravity variables
    public float gravity = -5.8f;
    public float groundedGravity = -.005f; 

    //jumping variables
    bool isJumpPressed = false;
    float initialJumpVelocity;
    public float maxJumpHeight = 2.0f;
    float maxJumpTime = 0.72f;
    bool isJumping = false;
    int isJumpingHash;
    int jumpCountHash;
    bool isJumpAnimating = false;
    int jumpCount = 0;
    Dictionary<int,float> initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int,float> jumpGravities = new Dictionary<int, float>();
    Coroutine currentJumpResetRoutine = null;

    // Awake is called earlier than Start in Unity's event life cycle
    void Awake(){

        // initially set reference variables
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        jumpCountHash = Animator.StringToHash("jumpCount");

        // set the player input callbacks
        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
    }

    void setupJumpVariables(){
        float timeToApex = maxJumpTime / 2;
        gravity =  (-2 * maxJumpHeight) / Mathf.Pow(timeToApex,2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        float secondJumpGravity = (-2 * (maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
        float secondJumpInitialVelocity = (2 * (maxJumpHeight + 2)) / (timeToApex * 1.25f);
        float thirdJumpGravity = (-2 * (maxJumpHeight + 2.75f)) / Mathf.Pow((timeToApex * 1.5f), 2);
        float thirdJumpInitialVelocity = (2 * (maxJumpHeight + 2.75f)) / (timeToApex * 1.5f);

        initialJumpVelocities.Add(1, initialJumpVelocity);
        initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

        jumpGravities.Add(0, gravity);
        jumpGravities.Add(1, gravity);
        jumpGravities.Add(2, secondJumpGravity);
        jumpGravities.Add(3, thirdJumpGravity);
    }

    void handleJump(){
        if(!isJumping && characterController.isGrounded && isJumpPressed){
            if(jumpCount < 3 && currentJumpResetRoutine != null){
                StopCoroutine(currentJumpResetRoutine);
            }
            //set animatior to start here
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            jumpCount += 1;
            animator.SetInteger(jumpCountHash, jumpCount);
            currentMovement.y = initialJumpVelocities[jumpCount];
            appliedMovement.y = initialJumpVelocities[jumpCount];
        }
        else if(isJumping && characterController.isGrounded && !isJumpPressed){
            isJumping = false;
        }
    }

    IEnumerator jumpResetRoutine(){
        yield return new WaitForSeconds(.5f);
        jumpCount = 0;
    }

    void onJump(InputAction.CallbackContext context){
        isJumpPressed = context.ReadValueAsButton();
        Debug.Log(isJumpPressed);
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

    void onMovementInput(InputAction.CallbackContext context){

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
        //bool isJumping = animator.GetBool(isJumpingHash);

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

    void handleGravity(){
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        //apply proper gravity if the player is grounded or not
        if(characterController.isGrounded){
            //set animator to switch here
            if(isJumpAnimating){
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
                currentJumpResetRoutine = StartCoroutine(jumpResetRoutine());
                if(jumpCount == 3){
                    jumpCount = 0;
                    animator.SetInteger(jumpCountHash, jumpCount);
                }
            }
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if(isFalling){
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (jumpGravities[jumpCount] * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * .5f,-20.0f);
        }
        else{
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (jumpGravities[jumpCount] * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + currentMovement.y) * .5f;
        }
    }

    // Update is called once per frame
    void Update(){
        handleRotation();
        handleAnimation();

        if(isRunPressed){
            appliedMovement.x = currentMovement.x;
            appliedMovement.x = currentMovement.z;
        }
        else{
            appliedMovement.x = currentMovement.x;
            appliedMovement.x = currentMovement.z;
        }

        characterController.Move(appliedMovement * Time.deltaTime);

        handleGravity();
        handleJump();
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