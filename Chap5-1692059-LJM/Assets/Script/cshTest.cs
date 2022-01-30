using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshTest : MonoBehaviour
{
    Animator anim;
    float speed = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anim.SetBool("Grounded", false);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anim.SetBool("Grounded", true);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            speed += 0.001f;
            if (speed >= 1.333f) speed = 1.333f;
            anim.SetFloat("MoveSpeed", speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            speed -= 0.001f;
            if (speed <= 0.0f) speed = 0;
            anim.SetFloat("MoveSpeed", speed);
        }

    }
}
