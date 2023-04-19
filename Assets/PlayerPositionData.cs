using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionData : MonoBehaviour
{
    public PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 savedPosition = playerData.GetSavedPlayerPosition();
        transform.position = savedPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        // Save the player's data to local storage
        playerData.SavePlayerPosition(transform.position);
    }
}
