using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingHook : MonoBehaviour
{
    public GameObject controller;

    void Start()
    {
        // Attach this object to the controller as a child object
        //transform.parent = controller.transform;

        // Add a HingeJoint component to the object
        //HingeJoint hinge = gameObject.AddComponent<HingeJoint>();
        //hinge.axis = Vector3.forward;
        // hinge.useSpring = true;
    }
}
