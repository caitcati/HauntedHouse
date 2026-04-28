using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Components
    Animator m_Animator;
    public InputAction MoveAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    // Speed Boost Settings
    [Header("Speed Boost")]
    public float boostSpeed = 3.0f;
    public float boostDuration = 2.0f;
    public float boostCooldown = 4.0f;

    private float boostTimer;
    private float cooldownTimer;

    private bool isBoosting = false;
    private bool canBoost = true;

    // UI
    [Header("UI")]
    public Image boostIcon;
    public Color readyColor = Color.green;
    public Color activeColor = Color.yellow;
    public Color cooldownColor = Color.red;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        MoveAction.Enable();
    }

    void FixedUpdate()
    {
        HandleBoost();
        UpdateBoostUI();

        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        m_Rigidbody.MoveRotation(m_Rotation);

        float currentSpeed = isBoosting ? boostSpeed : walkSpeed;
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * currentSpeed * Time.deltaTime);
    }

    void HandleBoost()
    {
        // Start boost when holding Left Shift
        if (Input.GetKey(KeyCode.LeftShift) && canBoost && !isBoosting)
        {
            isBoosting = true;
            canBoost = false;
            boostTimer = boostDuration;
        }

        // While boosting
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0f)
            {
                isBoosting = false;
                cooldownTimer = boostCooldown;
            }
        }
        // Cooldown phase
        else if (!canBoost)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                canBoost = true;
            }
        }
    }

    void UpdateBoostUI()
    {
        if (boostIcon == null) return;

        if (isBoosting)
            boostIcon.color = activeColor;
        else if (!canBoost)
            boostIcon.color = cooldownColor;
        else
            boostIcon.color = readyColor;
    }
}