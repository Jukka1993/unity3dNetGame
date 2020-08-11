using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour {

    protected bool isMoving = false;
    private Vector3 targetPosition;
    public float speed = 1.2f;
    private Animator animator;
    private float nextTime = 10f;
    private float curTime = 0f;


    public string desc = "";
    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }
    public void MoveUpdate()
    {
        if(isMoving == false)
        {
            return;
        }
        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(pos, targetPosition) < 0.5f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }
    //public 
    // Use this for initialization
    public void Start () {
        animator = GetComponent<Animator>();
        //MoveTo(new Vector3(5, 0, 5));
	}

    // Update is called once per frame
    public void Update () {
        MoveUpdate();

        //curTime += Time.deltaTime;
        //if(curTime>nextTime&&curTime<nextTime + 1)
        //{
        //    MoveTo(new Vector3(0, 0, 0));
        //}
	}
}
