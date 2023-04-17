using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingRod : MonoBehaviour
{
    public GameObject hook;
    public Slider powerBar;
    public float hookSpeedMultiplier = 5f;
    public float windUpSpeed = 3f;
    public float fishSwimSpeed = 1.5f;
    public float messageDisplayDuration = 5.0f;
    public float battleBarIncreaseSpeed = 0.3f;
    public float battleBarDecreaseSpeed = 0.15f;
    public float battleBarTimeThreshold = 3f;
    public float battleBarUpdateInterval = 3f;
    public GameObject waterPlane;
    public GameObject player_camera;
    public GameObject rodTip;
    public TextMeshProUGUI TMPDistance;
    public TextMeshProUGUI TMPMessage;
    public Image Tutorial;

    public Scrollbar battleBar;
    public GameObject targetBar;

    private bool isCasting = false;
    private bool hookFlying = false;

    private float initialFishDistance = -1;

    private float battleBarLastUpdateTime;
    float battleBarRandomWidth;
    float battleBarCenterOffset;
    private float battleBarTimer;

    // Start is called before the first frame update
    void Start()
    {
        battleBarLastUpdateTime = battleBarUpdateInterval;
        battleBarTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // get attached fish
        GameObject caughtFish = hook.GetComponent<Hook>().attachedFish;
        // fish move
        if (caughtFish != null)
        {
            Vector3 directionFish = (caughtFish.transform.position - rodTip.transform.position).normalized;
            caughtFish.transform.LookAt(rodTip.transform.position);
            caughtFish.transform.Translate(directionFish * fishSwimSpeed * Time.deltaTime, Space.World);
            hook.transform.position = caughtFish.transform.position;
            float fishDistance = Vector3.Distance(hook.transform.position, rodTip.transform.position);
            if (initialFishDistance == -1)
            {
                initialFishDistance = fishDistance;
                ShowMessage("Fish got hooked!", new Color(255, 165, 0), messageDisplayDuration);
                // start battle
                battleBar.gameObject.SetActive(true);
            }
            // --- battle bar ---
            if (battleBar.IsActive())
            {
                battleBarLastUpdateTime += Time.deltaTime;
                // update 
                if (battleBarLastUpdateTime >= battleBarUpdateInterval)
                {
                    // Randomize the target bar start position and width
                    battleBarRandomWidth = UnityEngine.Random.Range(0.2f, 0.4f);
                    battleBarCenterOffset = UnityEngine.Random.Range(battleBarRandomWidth / 2, 1 - battleBarRandomWidth / 2) - 0.5f;

                    RectTransform battleBarRect = battleBar.GetComponent<RectTransform>();
                    RectTransform targetBarRect = targetBar.GetComponent<RectTransform>();
                    // pos x
                    Vector2 anchoredPosition = targetBarRect.anchoredPosition;
                    anchoredPosition.x = battleBarRect.rect.width * battleBarCenterOffset;
                    targetBarRect.anchoredPosition = anchoredPosition;

                    // width
                    Vector2 sizeDelta = targetBarRect.sizeDelta;
                    sizeDelta.x = battleBarRect.rect.width * battleBarRandomWidth;
                    targetBarRect.sizeDelta = sizeDelta;

                    battleBarLastUpdateTime = 0f;
                }
                // check if in range
                float minValue = battleBarCenterOffset + 0.5f - battleBarRandomWidth / 2;
                float maxValue = battleBarCenterOffset + 0.5f + battleBarRandomWidth / 2;
                if (battleBar.value < minValue || battleBar.value > maxValue) 
                {
                    Debug.Log(minValue + " " + maxValue + " -- " + battleBarTimer);
                    battleBarTimer += Time.deltaTime;
                }
            }
            // --- battle bar ---
            if (fishDistance < 20f)
            {
                Debug.Log("Success!");
                ShowMessage("Success!", new Color(50, 205, 50), messageDisplayDuration);
                caughtFish.SetActive(false);
                hook.GetComponent<Hook>().attachedFish = null;
                caughtFish = null;
                EndFishing();
            }
            else if (fishDistance > initialFishDistance + 200f || battleBarTimer > battleBarTimeThreshold)
            {
                Debug.Log("Failed");
                ShowMessage("Failed!", new Color(255, 69, 0), messageDisplayDuration);
                caughtFish.SetActive(false);
                caughtFish = null;
                hook.GetComponent<Hook>().attachedFish = null;
                EndFishing();
            }
        }
        // left controller
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
                ShowDistance();
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

        // right controller
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = rightHandDevices[0];
            // Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
            bool triggerValue;
            // wind up
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue))
            {
                if (triggerValue)
                {
                    Debug.Log("Trigger button is pressed.");
                    if (caughtFish != null)
                    {
                        Vector3 windUpDirection = (rodTip.transform.position - caughtFish.transform.position).normalized;
                        if (caughtFish.transform.position.y > waterPlane.transform.position.y - 5f)
                        {
                            windUpDirection.y = 0;
                        }
                        caughtFish.transform.LookAt(rodTip.transform.position);
                        caughtFish.transform.Translate(windUpDirection * windUpSpeed * Time.deltaTime, Space.World);
                        hook.transform.position = caughtFish.transform.position;
                        if (battleBar.value < 1)
                        {
                            battleBar.value += battleBarIncreaseSpeed * Time.deltaTime;
                        }
                    }
                } else
                {
                    if (caughtFish != null && battleBar.value > 0)
                    {
                        battleBar.value -= battleBarDecreaseSpeed * Time.deltaTime;
                    }
                }
            }
            // tutorial
            bool primaryButtonValue;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primaryButtonValue) && primaryButtonValue)
            {
                Tutorial.gameObject.SetActive(true);
            } else
            {
                if (Tutorial.gameObject.activeSelf)
                {
                    Tutorial.gameObject.SetActive(false);
                }
            }
        }
        else if (rightHandDevices.Count > 1)
        {
            Debug.Log("Found more than one right hand!");
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

        // show distance
        TMPDistance.gameObject.SetActive(true);
    }
    
    // deactive fishing hook and fishing line
    void EndFishing()
    {
        hook.SetActive(false);
        FishingLine fishingLine = rodTip.GetComponent<FishingLine>();
        fishingLine.enabled = false;
        rodTip.GetComponent<LineRenderer>().enabled = false;
        TMPDistance.gameObject.SetActive(false);
        initialFishDistance = -1;
        battleBar.gameObject.SetActive(false);
        battleBarTimer = 0;
        battleBarLastUpdateTime = battleBarUpdateInterval;
    }

    void ShowDistance()
    {
        float distance = Vector3.Distance(hook.transform.position, rodTip.transform.position);
        TMPDistance.text = "Distance: " + Math.Round(distance, 2);
    }

    public void ShowMessage(string message, Color color, float duration)
    {
        TMPMessage.color = color;
        TMPMessage.SetText(message);
        TMPMessage.gameObject.SetActive(true);

        // Start the coroutine to hide the text after the specified duration
        StartCoroutine(HideMessage(duration));
    }

    private IEnumerator HideMessage(float duration)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);
        // Hide the text
        TMPMessage.gameObject.SetActive(false);
    }
}
