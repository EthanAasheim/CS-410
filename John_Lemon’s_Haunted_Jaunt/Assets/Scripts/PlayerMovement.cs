using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    public float walkspeed = 1f;
    public float runspeed = 3f;
    public float turnSpeed = 20f;
    public float acc_start = 0.05f;
    public float acc_stop = 0.2f;

    public TextMeshProUGUI UI;
    public GameObject ending;
    public float compass_threshold_1 = 20f;
    public float compass_threshold_2 = 10f;
    public float compass_threshold_3 = 0f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    float currentSpeed;
    float targetSpeed;
    bool isWalking;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();
        m_Movement.Set(0f, 0f, 0f);
        isWalking = false;
    }

    void FixedUpdate ()
    {
        float horizontal = Input.GetAxis ("Horizontal");
        float vertical = Input.GetAxis ("Vertical");

        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);

        isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);

        // Speed
        float targetSpeed = walkspeed;
        if (Input.GetKey(KeyCode.Space))
            targetSpeed = runspeed;

        // Walking
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
                m_AudioSource.Play();
        }
        else {
            m_AudioSource.Stop ();
            targetSpeed = 0f;
        }

        // LERP
        float acc = acc_start;
        if (targetSpeed <= currentSpeed)
            acc = acc_stop;
        currentSpeed = Lerp(currentSpeed, targetSpeed, acc);

        // Set movement to object
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();
        m_Movement.Set(currentSpeed * m_Movement.x, 0f, currentSpeed * m_Movement.z);

        // Set movement to object
        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward);

        // "Compass" feature with dot product
        Compass();
    }

    void OnAnimatorMove ()
    {
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation (m_Rotation);
    }


    private float Lerp(float a, float b, float t)
    {
        t = Mathf.Clamp01(t);
        return a + (b - a) * t;
    }


    private void Compass()
    {

        if (ending)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 toOther = ending.transform.position - transform.position;

            UI.text = "Compass: ";
            if (Vector3.Dot(forward, toOther) > compass_threshold_1)
                UI.text += "hot!";
            else if (Vector3.Dot(forward, toOther) > compass_threshold_2)
                UI.text += "warm";
            else if (Vector3.Dot(forward, toOther) > compass_threshold_3)
                UI.text += "neutral";
            else UI.text += "cold";
        }
    }
}
