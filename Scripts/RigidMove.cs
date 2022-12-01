using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMove : MonoBehaviour
{
    public Rigidbody mRig;
    public Vector3 v;
    private Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        dir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    dir.z = 1;
        //}
        //else if (Input.GetKeyDown(KeyCode.S))
        //{
        //    dir.z = -1;
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    dir.x = -1;
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    dir.x = 1;
        //}

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    dir.y = 1;
        //}
        //else
        //{
        //    dir.y = 0;
        //}
        mRig.MovePosition(new Vector3(1, 0, 0) * 2 * Time.deltaTime);
        //Debug.Log(dir.normalized * 2 * Time.deltaTime);
        //v = dir;
    }
}
