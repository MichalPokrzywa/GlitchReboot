// CHANGE LOG
//
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{
    #region Variables
    InputInterface _input = new StandardInput();
    public InputInterface ImplementedInput => _input;

    public MovementSettings movementSettings;

    Rigidbody rb;

    #region Camera Movement Variables

    public Camera playerCamera;

    float _yaw = 0.0f; // Rotation around the y axis
    float _pitch = 0.0f; // Rotation around the x axis
    public float yaw => _yaw;
    public float pitch => _pitch;

    float fov = 60f;
    bool invertCamera = false;
    bool cameraCanMove = true;
    float mouseSensitivity = 2f;
    float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    Image crosshairObject;

    #region Camera Zoom Variables

    bool enableZoom = true;
    bool holdToZoom = false;
    KeyCode zoomKey = KeyCode.Mouse1;
    float zoomFOV = 30f;
    float zoomStepTime = 5f;

    // Internal Variables
    bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    bool playerCanMove = true;
    bool enableFriction = true;
    float frictionResistance = 5f;
    float walkSpeed = 5f;
    float maxVelocityChange = 10f;

    // Internal Variables
    bool isWalking = false;
    float defaultWalkSpeed;

    #region Sprint

    bool enableSprint = true;
    bool unlimitedSprint = false;
    KeyCode sprintKey = KeyCode.LeftShift;
    float sprintSpeed = 7f;
    float sprintDuration = 5f;
    float sprintCooldown = .5f;
    float sprintFOV = 80f;
    float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    CanvasGroup sprintBarCG;
    bool isSprinting = false;
    float sprintRemaining;
    float sprintBarWidth;
    float sprintBarHeight;
    bool isSprintCooldown = false;
    float sprintCooldownReset;

    #endregion

    #region Jump

    bool enableJump = true;
    KeyCode jumpKey = KeyCode.Space;
    float jumpPower = 5f;

    // Internal Variables
    bool isGrounded = false;

    #endregion

    #region Crouch

    bool enableCrouch = true;
    KeyCode crouchKey = KeyCode.LeftControl;
    float crouchHeight = .75f;
    float speedReduction = .5f;

    // Internal Variables
    bool isCrouched = false;
    bool holdToCrouch = true;
    Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public Transform joint;
    bool enableHeadBob = true;
    float bobSpeed = 10f;
    Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    Vector3 jointOriginalPos;
    float timer = 0;

    #endregion

    #region Animations

    public Animator animator;
    private float speed;

    #endregion

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        crosshairObject = GetComponentInChildren<Image>();

        var ghostInput = gameObject.GetComponent<GhostInput>();
        if (ghostInput)
            _input = ghostInput;

        // Get movement settings from scriptable object
        GetMovementSettings();

        // Set internal variables
        if (playerCamera != null)
            playerCamera.fieldOfView = fov;

        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;
        defaultWalkSpeed = walkSpeed;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }
    }

    void Start()
    {
        if(playerCamera != null && lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        HandleCrosshair();
        HandleSprintBar();
    }

    float camRotation;

    private void Update()
    {
        HandleZoom();
    }

    private void FixedUpdate()
    {
        if(cameraCanMove)
            HandleCameraMovement();

        if(enableSprint)
            HandleSprinting();

        // Gets input and calls jump method
        if(enableJump && ImplementedInput.IsJumping() && isGrounded)
            Jump();

        if (enableCrouch)
            HandleCrouch();

        CheckGround();

        if(enableHeadBob)
            HeadBob();

        if (playerCanMove)
            HandleMovement();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 2);
    }

    public void Rotate(float pitch, float yaw)
    {
        transform.localEulerAngles = new Vector3(0, yaw, 0);
        if (playerCamera != null)
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    void HandleCameraMovement()
    {
        _yaw = transform.localEulerAngles.y + ImplementedInput.GetMouseX() * mouseSensitivity;

        if (!invertCamera)
        {
            _pitch -= mouseSensitivity * ImplementedInput.GetMouseY();
        }
        else
        {
            // Inverted Y
            _pitch += mouseSensitivity * ImplementedInput.GetMouseY();
        }

        // Clamp pitch between lookAngle
        _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);

        transform.localEulerAngles = new Vector3(0, _yaw, 0);
        if (playerCamera != null)
            playerCamera.transform.localEulerAngles = new Vector3(_pitch, 0, 0);
    }

    void HandleCrosshair()
    {
        if (crosshairObject == null)
            return;

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }
    }

    void HandleSprintBar()
    {
        sprintBarCG = GetComponentInChildren<CanvasGroup>();
        if (sprintBarCG == null)
            return;

        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth = screenWidth * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull)
            {
                sprintBarCG.alpha = 0;
            }
        }
        else
        {
            sprintBarBG.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }
    }

    void HandleSprinting()
    {
        if (isSprinting)
        {
            isZoomed = false;
            if (playerCamera != null && playerCamera.fieldOfView != sprintFOV)
            {
                playerCamera.DOFieldOfView(sprintFOV, sprintFOVStepTime);
            }
                //playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.fixedDeltaTime);
                
            // Drain sprint remaining while sprinting
            if (!unlimitedSprint)
            {
                sprintRemaining -= 1 * Time.deltaTime;
                if (sprintRemaining <= 0)
                {
                    isSprinting = false;
                    isSprintCooldown = true;
                }
            }
        }
        else
        {
            // Regain sprint while not sprinting
            if (playerCamera != null && playerCamera.fieldOfView != fov)
                playerCamera.DOFieldOfView(fov, sprintFOVStepTime);
            sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
        }

        // Handles sprint cooldown
        // When sprint remaining == 0 stops sprint ability until hitting cooldown
        if (isSprintCooldown)
        {
            sprintCooldown -= 1 * Time.deltaTime;
            if (sprintCooldown <= 0)
            {
                isSprintCooldown = false;
            }
        }
        else
        {
            sprintCooldown = sprintCooldownReset;
        }

        // Handles sprintBar
        if (useSprintBar && !unlimitedSprint)
        {
            float sprintRemainingPercent = sprintRemaining / sprintDuration;
            sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
        }
    }

    void HandleMovement()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(ImplementedInput.GetHorizontalInput(), 0, ImplementedInput.GetVerticalInput());

        // Checks if player is walking and isGrounded
        // Will allow head bob
        if (targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        // All movement calculations while sprint is active
        if (enableSprint && ImplementedInput.IsSprinting() && sprintRemaining > 0f && !isSprintCooldown)
        {
            targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);

            // Continuous slow down when not moving
            if (enableFriction)
                velocityChange = HandleFriction(targetVelocity, velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            // Player is only moving when valocity change != 0
            // Makes sure fov change only happens during movement
            if (velocityChange.x != 0 || velocityChange.z != 0)
            {
                isSprinting = true;

                // If player is sprinting, stop crouching
                if (isCrouched)
                {
                    isCrouched = false;
                    Crouch();
                }

                if (sprintBarCG != null && hideBarWhenFull && !unlimitedSprint)
                {
                    sprintBarCG.alpha += 5 * Time.deltaTime;
                }
            }

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        // All movement calculations while walking
        else
        {
            isSprinting = false;

            if (sprintBarCG != null && hideBarWhenFull && sprintRemaining == sprintDuration)
            {
                sprintBarCG.alpha -= 3 * Time.deltaTime;
            }

            targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);

            // Continuous slow down when not moving
            if (enableFriction)
                velocityChange = HandleFriction(targetVelocity, velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    Vector3 HandleFriction(Vector3 targetVelocity, Vector3 velocity)
    {
        Vector3 velocityChange = (targetVelocity - velocity);
        if (targetVelocity.magnitude < 0.1f)
        {
            Vector3 friction = -velocity * frictionResistance * Time.fixedDeltaTime;

            friction.x = Mathf.Clamp(friction.x, -Mathf.Abs(velocity.x), Mathf.Abs(velocity.x));
            friction.z = Mathf.Clamp(friction.z, -Mathf.Abs(velocity.z), Mathf.Abs(velocity.z));

            velocityChange.x = friction.x;
            velocityChange.z = friction.z;

        }
        return velocityChange;
    }

    void HandleZoom()
    {
        if (playerCamera != null && enableZoom)
        {
            // Changes isZoomed when key is pressed
            // Behavior for toogle zoom
            if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
            {
                isZoomed = !isZoomed;
            }

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if (holdToZoom && !isSprinting)
            {
                isZoomed = Input.GetKey(zoomKey);
            }

            // Lerps camera.fieldOfView to allow for a smooth transistion
            float targetFOV = isZoomed ? zoomFOV : fov;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, zoomStepTime * Time.deltaTime);
        }
    }

            //playerCamera.DOFieldOfView(targetFOV, zoomStepTime);
    void HandleCrouch()
    {
        // If player is not grounded, stop crouching
        if (!isGrounded)
        {
            if (isCrouched)
            {
                isCrouched = false;
                Crouch();
            }
            return;
        }

        bool shouldCrouch = ImplementedInput.IsCrouching();

        // Behavior for hold to crouch
        if (holdToCrouch)
        {
            if (shouldCrouch != isCrouched)
            {
                isCrouched = !isCrouched;
                Crouch();
            }
        }
    }

    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        // When crouched, uncrouch for a jump
        if (isCrouched)
        {
            isCrouched = false;
            Crouch();
        }

        if (rb.linearVelocity.y <= 0)
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);

        isGrounded = false;
    }

    private void Crouch()
    {
        // Crouches player down to crouchHeight
        // Reduces walkSpeed
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed = defaultWalkSpeed * speedReduction;
        }
        // Stands player up to full height
        // Brings walkSpeed back up to original speed
        else
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed = defaultWalkSpeed;
        }
    }

    private void HeadBob()
    {
        if(isWalking)
        {
            // Calculates HeadBob speed during sprint
            if(isSprinting)
            {
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            // Calculates HeadBob speed during crouched movement
            else if (isCrouched)
            {
                timer += Time.deltaTime * (bobSpeed * speedReduction);
            }
            // Calculates HeadBob speed during walking
            else
            {
                timer += Time.deltaTime * bobSpeed;
            }
            // Applies HeadBob movement
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }

    private void GetMovementSettings()
    {
        if (movementSettings == null)
        {
            Debug.LogError("No movement settings found. Please create a movement settings scriptable object and assign it to the player controller.");
            return;
        }

        #region Camera Settings
        fov = movementSettings.cameraVariables.FOV;
        cameraCanMove = movementSettings.cameraVariables.enableCameraRotation;
        invertCamera = movementSettings.cameraVariables.invertCameraRotation;
        mouseSensitivity = movementSettings.cameraVariables.mouseSensitivity;
        maxLookAngle = movementSettings.cameraVariables.maxLookAngle;
        enableZoom = movementSettings.cameraVariables.enableZoom;
        holdToZoom = movementSettings.cameraVariables.holdToZoom;
        zoomFOV = movementSettings.cameraVariables.zoomFOV;
        zoomStepTime = movementSettings.cameraVariables.stepTimeZoom;
        zoomKey = movementSettings.cameraVariables.zoomKey;
        #endregion

        #region Movement Settings
        playerCanMove = movementSettings.movementVariables.enableWalk;
        enableFriction = movementSettings.movementVariables.enableFriction;
        frictionResistance = movementSettings.movementVariables.frictionResistance;
        walkSpeed = movementSettings.movementVariables.walkSpeed;
        enableSprint = movementSettings.movementVariables.enableSprint;
        unlimitedSprint = movementSettings.movementVariables.unlimitedSprint;
        sprintSpeed = movementSettings.movementVariables.sprintSpeed;
        sprintDuration = movementSettings.movementVariables.sprintDuration;
        sprintCooldown = movementSettings.movementVariables.sprintCooldown;
        sprintFOV = movementSettings.movementVariables.sprintFOV;
        sprintFOVStepTime = movementSettings.movementVariables.stepTimeSprint;
        sprintKey = movementSettings.movementVariables.sprintKey;
        #endregion

        #region Jump Settings
        enableJump = movementSettings.jumpVariables.enableJump;
        jumpPower = movementSettings.jumpVariables.jumpPower;
        jumpKey = movementSettings.jumpVariables.jumpKey;
        #endregion

        #region Crouch Settings
        enableCrouch = movementSettings.crouchVariables.enableCrouch;
        crouchHeight = movementSettings.crouchVariables.crouchHeight;
        speedReduction = movementSettings.crouchVariables.crouchSpeedReduction;
        crouchKey = movementSettings.crouchVariables.crouchKey;
        #endregion

        #region Head Bob Settings
        enableHeadBob = movementSettings.headBobVariables.enableHeadBob;
        bobSpeed = movementSettings.headBobVariables.speed;
        bobAmount = movementSettings.headBobVariables.bobAmount;
        #endregion
    }
}


// Custom Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
    public class FirstPersonControllerEditor : Editor
    {
    FirstPersonController fpc;
    SerializedObject SerFPC;

    private void OnEnable()
    {
        fpc = (FirstPersonController)target;
        SerFPC = new SerializedObject(fpc);
    }

    public override void OnInspectorGUI()
    {
        SerFPC.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Jess Case", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("** version customized for personal use **", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 10 });
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        #region Movement Settings
        GUILayout.Label("ScriptableObject for storing movement settings", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.movementSettings = (MovementSettings)EditorGUILayout.ObjectField(new GUIContent("Movement Settings", "Scriptable object that holds all movement settings."), fpc.movementSettings, typeof(MovementSettings), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        #endregion

        #region Animator
        GUILayout.Label("Animator Setup (NOT REQUIRED)", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();
        fpc.animator = (Animator)EditorGUILayout.ObjectField(new GUIContent("Animator", "Animator for character"), fpc.animator, typeof(Animator), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        #endregion

        #region Camera Setup
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.playerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera attached to the controller."), fpc.playerCamera, typeof(Camera), true);

        GUI.enabled = fpc.playerCamera != null;

        /*
        fpc.fov = EditorGUILayout.Slider(new GUIContent("Field of View", "The camera’s view angle. Changes the player camera directly."), fpc.fov, fpc.zoomFOV, 179f);
        fpc.cameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), fpc.cameraCanMove);

        GUI.enabled = fpc.cameraCanMove;
        fpc.invertCamera = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), fpc.invertCamera);
        fpc.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Look Sensitivity", "Determines how sensitive the mouse movement is."), fpc.mouseSensitivity, .1f, 10f);
        fpc.maxLookAngle = EditorGUILayout.Slider(new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."), fpc.maxLookAngle, 40, 90);
        GUI.enabled = true;
        */

        fpc.lockCursor = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), fpc.lockCursor);
        fpc.crosshair = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on, and sets is to the center of the screen."), fpc.crosshair);

        // Only displays crosshair options if crosshair is enabled
        if(fpc.crosshair)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Image", "Sprite to use as the crosshair."));
            fpc.crosshairImage = (Sprite)EditorGUILayout.ObjectField(fpc.crosshairImage, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.crosshairColor = EditorGUILayout.ColorField(new GUIContent("Crosshair Color", "Determines the color of the crosshair."), fpc.crosshairColor);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        #region Camera Zoom Setup
        /*
        GUILayout.Label("Zoom", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.enableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), fpc.enableZoom);

        GUI.enabled = fpc.enableZoom;
        fpc.holdToZoom = EditorGUILayout.ToggleLeft(new GUIContent("Hold to Zoom", "Requires the player to hold the zoom key instead if pressing to zoom and unzoom."), fpc.holdToZoom);
        fpc.zoomKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Zoom Key", "Determines what key is used to zoom."), fpc.zoomKey);
        fpc.zoomFOV = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), fpc.zoomFOV, .1f, fpc.fov);
        fpc.zoomStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while zooming in."), fpc.zoomStepTime, .1f, 10f);
        GUI.enabled = true
        */
        #endregion
        #endregion

        #region Movement Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        /*
        fpc.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), fpc.playerCanMove);

        GUI.enabled = fpc.playerCanMove;
        fpc.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), fpc.walkSpeed, .1f, fpc.sprintSpeed);
        GUI.enabled = true;
        */
        EditorGUILayout.Space();

        #region Sprint

        GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        /*
        fpc.enableSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), fpc.enableSprint);

        GUI.enabled = fpc.enableSprint;
        fpc.unlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), fpc.unlimitedSprint);
        fpc.sprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key is used to sprint."), fpc.sprintKey);
        fpc.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), fpc.sprintSpeed, fpc.walkSpeed, 20f);

        //GUI.enabled = !fpc.unlimitedSprint;
        fpc.sprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), fpc.sprintDuration, 1f, 20f);
        fpc.sprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), fpc.sprintCooldown, .1f, fpc.sprintDuration);
        //GUI.enabled = true;

        fpc.sprintFOV = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the field of view the camera changes to while sprinting."), fpc.sprintFOV, fpc.fov, 179f);
        fpc.sprintFOVStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while sprinting."), fpc.sprintFOVStepTime, .1f, 20f);
        */

        fpc.useSprintBar = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Determines if the default sprint bar will appear on screen."), fpc.useSprintBar);

        // Only displays sprint bar options if sprint bar is enabled
        if(fpc.useSprintBar)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            fpc.hideBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the sprint bar when sprint duration is full, and fades the bar in when sprinting. Disabling this will leave the bar on screen at all times when the sprint bar is enabled."), fpc.hideBarWhenFull);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
            fpc.sprintBarBG = (Image)EditorGUILayout.ObjectField(fpc.sprintBarBG, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as sprint bar foreground."));
            fpc.sprintBar = (Image)EditorGUILayout.ObjectField(fpc.sprintBar, typeof(Image), true);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            fpc.sprintBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Width", "Determines the width of the sprint bar."), fpc.sprintBarWidthPercent, .1f, .5f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.sprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the sprint bar."), fpc.sprintBarHeightPercent, .001f, .025f);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Jump
        /*
        GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.enableJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), fpc.enableJump);

        GUI.enabled = fpc.enableJump;
        fpc.jumpKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Jump Key", "Determines what key is used to jump."), fpc.jumpKey);
        fpc.jumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player will jump."), fpc.jumpPower, .1f, 20f);
        GUI.enabled = true;
        */

        EditorGUILayout.Space();
        #endregion

        #region Crouch
        /*
        GUILayout.Label("Crouch", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.enableCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Crouch", "Determines if the player is allowed to crouch."), fpc.enableCrouch);

        GUI.enabled = fpc.enableCrouch;
        fpc.crouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Crouch Key", "Determines what key is used to crouch."), fpc.crouchKey);
        fpc.crouchHeight = EditorGUILayout.Slider(new GUIContent("Crouch Height", "Determines the y scale of the player object when crouched."), fpc.crouchHeight, .1f, 1);
        fpc.speedReduction = EditorGUILayout.Slider(new GUIContent("Speed Reduction", "Determines the percent 'Walk Speed' is reduced by. 1 being no reduction, and .5 being half."), fpc.speedReduction, .1f, 1);
        GUI.enabled = true;
        */
        #endregion
        #endregion

        #region Head Bob
        /*
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.enableHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Enable Head Bob", "Determines if the camera will bob while the player is walking."), fpc.enableHeadBob);

        GUI.enabled = fpc.enableHeadBob;
        fpc.joint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Joint", "Joint object position is moved while head bob is active."), fpc.joint, typeof(Transform), true);
        fpc.bobSpeed = EditorGUILayout.Slider(new GUIContent("Speed", "Determines how often a bob rotation is completed."), fpc.bobSpeed, 1, 20);
        fpc.bobAmount = EditorGUILayout.Vector3Field(new GUIContent("Bob Amount", "Determines the amount the joint moves in both directions on every axes."), fpc.bobAmount);
        GUI.enabled = true;
        */
        #endregion

        //Sets any changes from the prefab
        if(GUI.changed)
        {
            EditorUtility.SetDirty(fpc);
            Undo.RecordObject(fpc, "FPC Change");
            SerFPC.ApplyModifiedProperties();
        }
    }

}

#endif