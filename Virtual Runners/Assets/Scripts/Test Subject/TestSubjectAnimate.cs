// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************************************************************************************
 * TestSubjectAnimate is a class that controls the animator of test subject based on OVR inputs
 ******************************************************************************************************/



public class TestSubjectAnimate : MonoBehaviour
{

    private TestSubjectMovement TestSubjectController;
    private Vector2 Thumbstick;
    private float LIndexTrigger;
    private Animator animator;

    void Start()
    {
        TestSubjectController = GetComponentInParent<TestSubjectMovement>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // call update to refresh OVR inputs
        OVRInput.Update();
        // obtain thumb position
        Thumbstick = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        // obtain left index trigger
        LIndexTrigger = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);

        // perform animations based on inputs
        animator.SetBool("isJumping", LIndexTrigger >= 0.5f);

        // running animations
        float absThumbx = Mathf.Abs(Thumbstick.x);
        float absThumby = Mathf.Abs(Thumbstick.y);
        // perform run if thumbstick is not at zero position
        if (absThumbx != 0 || absThumby != 0)
        {
            animator.SetBool("isRunning", true);
            // take greater of  the two vectors and use for animation speed
            if (absThumbx > absThumby)
            {
                animator.speed = 3.0f * absThumbx;

            } else
            {
                animator.speed = 3.0f * absThumby;
            }
        } else
        {
            animator.SetBool("isRunning", false);
            animator.speed = 1.0f;
        }

        // mark inAir so that running animation doesnt occur in the air
        animator.SetBool("inAir", TestSubjectController.GetInAir());

    }
}
