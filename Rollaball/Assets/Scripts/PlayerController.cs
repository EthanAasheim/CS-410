using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public float jumpStrength = 0;
    public int maxJumps;

    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public int maxCount;

    private float movementX;
    //private float movementY;
    private float movementZ;

    private bool onGround;
    private bool isJumpPressed;
    private int numJumps;

    private Rigidbody rb;
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        movementX = 0;
        //movementY = 0;
        movementZ = 0;

        onGround = true;
        numJumps = 0;
        isJumpPressed = false;

        count = 0;

        SetCountText();
        winTextObject.SetActive(false);
    }

    // Get movement input
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementZ = movementVector.y;
    }

    // If touching ground
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            onGround = true;
            numJumps = maxJumps;
        }
    }

    // If leaving ground
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            onGround = false;
            numJumps = maxJumps - 1;
        }
    }

    // Jumping
    void Update()
    {

        // If jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpPressed = true;
        }

        // Debug
        SetCountText();
    }

    // Move
    void FixedUpdate()
    {
        rb.AddForce(new Vector3(speed * movementX, 0.0f, speed * movementZ));

        if (isJumpPressed && (onGround || numJumps > 0))
        {
            rb.AddForce(new Vector3(0.0f, jumpStrength, 0.0f), ForceMode.Impulse);

            numJumps--;
            isJumpPressed = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString() + " / " + maxCount.ToString();;

        if (count >= maxCount)
        {
            winTextObject.SetActive(true);
        }
    }
}
