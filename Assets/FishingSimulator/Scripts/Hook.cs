using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{

    public GameObject attachedFish;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a fish
        if (other.gameObject.CompareTag("Fish"))
        {
            Debug.Log("Fish got hooked!");
            // Attach the fish to the hook
            attachedFish = other.gameObject;
            attachedFish.transform.position = this.transform.position;
            //attachedFish.transform.localPosition = Vector3.zero;
            attachedFish.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
