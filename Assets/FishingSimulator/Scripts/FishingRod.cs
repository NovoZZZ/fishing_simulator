using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingRod : MonoBehaviour
{
    public GameObject hook;
    public Slider powerBar;
    public float hookSpeedMultiplier = 5f;
    public GameObject waterPlane;
    public GameObject player_camera;
    public GameObject rodTip;

    private bool isCasting = false;
    private bool hookFlying = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        if (leftHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = leftHandDevices[0];
            // Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
            bool triggerValue;
            // cast fishing
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            {
                Debug.Log("Trigger button is pressed.");
                if (!isCasting)
                {
                    isCasting = true;
                    InitPowerBar();
                }
                Casting();
            } else
            {   
                if (isCasting)
                {
                    Debug.Log("Trigger button is released.");
                    EndCasting();
                }
                if (hookFlying && hook.transform.position.y <= waterPlane.transform.position.y - 10f)
                {
                    hook.GetComponent<Rigidbody>().useGravity = false;
                    hook.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    hookFlying = false;
                    // check if the hook is in the lake
                    if (!hookFlying)
                    {
                        float distanceFromLakeCenter = Vector3.Distance(hook.transform.position, waterPlane.transform.position);
                        MeshRenderer waterMeshRenderer = waterPlane.GetComponent<MeshRenderer>();
                        float lakeRadius = waterMeshRenderer.bounds.size.x / 2f;
                        if (distanceFromLakeCenter > lakeRadius)
                        {
                            EndFishing();
                            return;
                        }
                    }
                }
            }
            // end fishing
            bool primaryButtonValue;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primaryButtonValue) && primaryButtonValue)
            {
                Debug.Log("Primary button pressed");
                EndFishing();
            }
        }
        else if (leftHandDevices.Count > 1)
        {
            Debug.Log("Found more than one left hand!");
        }
    }

    void InitPowerBar()
    {
        powerBar.gameObject.SetActive(true);
        powerBar.value = 0f;
    }

    void Casting()
    {
        powerBar.value += Time.deltaTime;
    }

    void EndCasting()
    {
        isCasting = false;
        powerBar.gameObject.SetActive(false);
        hook.transform.position = rodTip.transform.position;
        hook.SetActive(true);

        // show fishing line
        FishingLine fishingLine = rodTip.GetComponent<FishingLine>();
        fishingLine.enabled = true;
        rodTip.GetComponent<LineRenderer>().enabled = true;

        float hookSpeed = powerBar.value * hookSpeedMultiplier;
        // GameObject hook = Instantiate(hook, transform.position, Quaternion.identity);
        hook.GetComponent<Rigidbody>().velocity = player_camera.transform.forward * hookSpeed;
        hook.GetComponent<Rigidbody>().useGravity = true;
        hookFlying = true;
    }
    
    // deactive fishing hook and fishing line
    void EndFishing()
    {
        hook.SetActive(false);
        FishingLine fishingLine = rodTip.GetComponent<FishingLine>();
        fishingLine.enabled = false;
        rodTip.GetComponent<LineRenderer>().enabled = false;
    }
}
