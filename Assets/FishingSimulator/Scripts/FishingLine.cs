using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public GameObject hook;
    private GameObject fishingLineEnd;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        fishingLineEnd = new GameObject("FishingLineEnd");
        fishingLineEnd.transform.parent = hook.transform;
        fishingLineEnd.transform.localPosition = Vector3.zero;

        // Set up the Line Renderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        fishingLineEnd.transform.position = hook.transform.position;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, fishingLineEnd.transform.position);
    }
}
