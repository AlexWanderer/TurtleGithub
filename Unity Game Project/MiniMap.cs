/*
 * Written by Sam Arutyunyan, 2014 for 'Whiskers of Death Game' project. 
 * toggleable minimap that shows position of the player in relation to the map. instead of 
 * projecting map camera onto the terrain, a snapshot was taken of the terrain and used.
*/
using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour 
{
    public GUITexture mapGUI;
    public Transform camOccluder;
    float disabledYPos = -.26f;
    float desiredPosition = 0;  // stores current position and lerps to and fro
    bool mapEnabled = false;    // if true, the map gui is moving up or already up in position
    bool mapReady = true;       // if true, the map is in position and camera can turn on
    float lerpPerc = 0;

    
    public bool MapEnabled
    {
        get { return mapEnabled; } 
        set { mapEnabled = value; } 
    }
	
	void Update () 
    {   
        if (mapEnabled)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                mapEnabled = false;
                lerpPerc = 0;
            }

            desiredPosition = .5f;
            
            if (mapGUI.transform.position.y > desiredPosition - .01f)
            { 
                mapGUI.transform.position = new Vector3(.5f, desiredPosition, 0);
                mapReady = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                mapEnabled = true;
                lerpPerc = 0;
            }

            mapReady = false;
            desiredPosition = disabledYPos;

            if (mapGUI.transform.position.y < desiredPosition + .01f)
            {
                mapGUI.transform.position = new Vector3(.5f, desiredPosition, 0);
            }
        }

        lerpPerc += Time.deltaTime;
        mapGUI.transform.position = new Vector3(.5f, Mathf.Lerp(mapGUI.transform.position.y, desiredPosition, lerpPerc), 0);

        if (mapReady)
        {
            camOccluder.gameObject.SetActive(false);
        }
        else 
        {
            camOccluder.gameObject.SetActive(true);
        }
	}
}
