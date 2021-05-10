using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform playerToFollow;

    private float yPosDiff;
    // Start is called before the first frame update
    void Start()
    {
        if(playerToFollow != null)
            yPosDiff = Mathf.Abs(transform.position.y - playerToFollow.transform.position.y);
        else
            yPosDiff = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y + yPosDiff, transform.position.z);
    }
}
