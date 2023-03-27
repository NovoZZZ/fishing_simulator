using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGenerator : MonoBehaviour
{

    public GameObject fishPrefab;
    public float distanceThreshold = 5f;
    public int maxFishCount = 10;
    private int fishCount = 0;

    public float swimSpeed = 2f;

    void Start()
    {
        InvokeRepeating("ChangeFishDirection", 5f, 5f);
    }


    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.Find("Main Camera");
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance < distanceThreshold && fishCount < maxFishCount)
        {
            Vector3 randomPos = new Vector3(Random.Range(-200f, 200f), -5f, Random.Range(-200f, 200f));
            GameObject fish = Instantiate(fishPrefab, transform.position + randomPos, Quaternion.identity);
            fish.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere.normalized * swimSpeed;
            fishCount++;
        }
    }

    void ChangeFishDirection()
    {
        GameObject[] fishes = GameObject.FindGameObjectsWithTag("Fish");
        foreach (GameObject fish in fishes)
        {
            Vector3 newDirection = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)).normalized;
            fish.GetComponent<Rigidbody>().velocity = newDirection * swimSpeed;
        }
    }
}
