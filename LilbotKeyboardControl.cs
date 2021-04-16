using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilbotKeyboardControl : MonoBehaviour
{
    public float acceleration;
    public float maxSpeed;

    private Rigidbody rigidBody;
    private KeyCode[] inputKeys;
    private Vector3[] directionsForKeys;

    // Start is called before the first frame update
    void Start()
    {
        //inputKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        inputKeys = new KeyCode[] { KeyCode.D, KeyCode.W, KeyCode.A, KeyCode.S };
        directionsForKeys = new Vector3[] { Vector3.forward, Vector3.left, Vector3.back, Vector3.right };
        //directionsForKeys = new Vector3[] { Vector3.forward, Vector3.left, Vector3.back, Vector3.right };
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < inputKeys.Length; i++)
        {
            var key = inputKeys[i];

            // 2
            if (Input.GetKey(key))
            {
                // 3
                Vector3 movement = directionsForKeys[i] * acceleration * Time.deltaTime;
                transform.Translate(movement);
            }
        }
    }

}
