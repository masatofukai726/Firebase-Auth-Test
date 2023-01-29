using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 5.0f;
    private Animator anim;
    private Rigidbody rb;
    public float jump = 5.0f;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    

    void Update()
    {
        Transform myTransform = this.transform;
        Vector3 worldPos = myTransform.position;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * speed * 2 * Time.deltaTime;
            anim.SetBool("blwalk", true);
        }else
        {
            anim.SetBool("blwalk", false);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= transform.forward * speed * 2 * Time.deltaTime;
            anim.SetBool("blwalk", true);
        }else
        {
            anim.SetBool("blwalk", false);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Rotate(0, 1, 0);
            anim.SetBool("blwalk", true);
        }else
        {
            anim.SetBool("blwalk", false);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Rotate(0, -1, 0);
            anim.SetBool("blwalk", true);
        }else
        {
            anim.SetBool("blwalk", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Start");
            rb.AddForce(transform.up * jump, ForceMode.Impulse);
            Debug.Log("Finish");
            anim.SetBool("Jump", true);

        }else
        {
            anim.SetBool("Jump", false);
        }

        //Debug.Log(blwalk);
    }
}
