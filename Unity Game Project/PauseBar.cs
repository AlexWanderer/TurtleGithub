/*
 * Written by Sam Arutyunyan, 2014 for 'Whiskers of Death Game' project. 
 * pauses the game, displays options. 
*/
using UnityEngine;
using System.Collections;

public class PauseBar : MonoBehaviour 
{
    public Texture2D darkenBG;  // fills entire screen with dark
    public Texture2D pauseFrame;// the thingy that holds menu buttons

    int toolbarInt = 0;
    string[] toolbarStrings = { "Controls", "Quit Level", "Exit Game" };

    public GUIStyle pauseStyle; // define all style infos in inspector (instead of guiskin)
    float position = 1600;      // start offscreen right
    float desiredPosition = 1600;
    float alpha = 0;            // alpha of the background
    float desiredAlpha = 0;
    bool paused = false;
    bool displayPause = false;  // after we unpause, we will still display the pause gui while it animates
    float lerpPerc = 0;

    float originalWidth = 1600;
    float originalHeight = 900;
    Vector3 scale = Vector3.zero;

    void Update () 
    {
        if (paused)
        {
            if (position < 1297 + .05f)
            {
                position = 1297;
                alpha = 1;
            }

            if (Input.GetKeyDown(KeyCode.BackQuote))//`
            {
                //OffPause
                desiredPosition = 1600;//head offscreen to right
                desiredAlpha = 0;
                paused = false;
                lerpPerc = 0;
            }
        }
        else //not paused
        { 
            //not paused but still displaying (lerping offscreen)
            if (displayPause)
            {
                if (position > 1600 - .05f)
                {
                    position = 1600;
                    alpha = 0;
                    displayPause = false;
                }
            }

            if( Input.GetKeyDown(KeyCode.BackQuote) )//`
            {
                //OnPause
                desiredPosition = 1297;
                desiredAlpha = 1;
                paused = true;
                displayPause = true;
                lerpPerc = 0;
            }            
        }

        if (displayPause)
        {
            scale = new Vector3(Screen.width / originalWidth, Screen.height / originalHeight, 1);
            lerpPerc += Time.deltaTime;
            position = Mathf.Lerp(position, desiredPosition, lerpPerc);
            alpha = Mathf.Lerp(alpha, desiredAlpha, lerpPerc);
        }
	}
   	
	// Update is called once per frame
	void OnGUI () 
    {
        if (displayPause)
        {
            Matrix4x4 matrix = GUI.matrix; // save current matrix
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
            GUI.depth = 1;
            // darken background
            GUI.color = new Color(1, 1, 1, alpha);
            GUI.DrawTexture(new Rect(0, 0, 1600, 900), darkenBG);
            GUI.color = Color.white;


            GUI.BeginGroup(new Rect(position, 293, 303, 461), pauseFrame);

            if (GUI.Button(new Rect(13, 112, 253, 86), "Controls", pauseStyle))
            {
            }
            if (GUI.Button(new Rect(13, 215, 253, 86), "Quit Level", pauseStyle))
            {
            }
            if (GUI.Button(new Rect(13, 318, 253, 86), "Exit Game", pauseStyle))
            {
            }

            GUI.EndGroup();

            GUI.matrix = matrix; // restore original matrix
            GUI.depth = 0;
        }	
	}
}
