using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Valve.VR;
using UnityEngine.SceneManagement;
using System.Collections;

public class LocationGazeScript : MonoBehaviour
{
    public bool isSelected;
    public bool timedClick = true;
    public bool clicker = true;
    public bool highlight = true;

    public float delay;
    float timer;
    float Video1Timer;
    float Video1scaleFactor; 
    float scaleFactor;
    float maximumFOV;
    float minFOV; 

    Transform camera;

    public Image ProgressBar;
    public Image Video1ProgressBar; 

    public GameObject MainPanel;
    public GameObject VideoPanel1;
    public GameObject VideoPanel2; 
    public GameObject Exit;

    public Text Video1ToLoad; 

    AudioSource ReadInfo; 

    void Awake()
    {
        camera = Camera.main.transform;

        if (GameObject.Find("Loc/Canvas/Panel"))
        {
            MainPanel = GameObject.Find("Loc/Canvas/Panel");
        }

        if (GameObject.Find("Loc/DoorExit/Canvas/Panel"))
        {
            Exit = GameObject.Find("Loc/DoorExit/Canvas/Panel");
        }

        if (GameObject.Find("Loc/DoorExit/Canvas/Panel/ProgressBar"))
        {
            ProgressBar = GameObject.Find("Loc/DoorExit/Canvas/Panel/ProgressBar").GetComponent<Image>();
        }

        if(GameObject.Find("Loc/Canvas/Video1Panel")) 
        {
            VideoPanel1 = GameObject.Find("Loc/Canvas/Video1Panel");
        }

        if (GameObject.Find("Loc/Canvas/Video1Panel/Video1Progress"))
        {
            Video1ProgressBar = GameObject.Find("Loc/Canvas/Video1Panel/Video1Progress").GetComponent<Image>();
        }

        if (GameObject.Find("Loc/Canvas/Video1Panel/SceneToLoad"))
        {
            Video1ToLoad = GameObject.Find("Loc/Canvas/Video1Panel/SceneToLoad").GetComponent<Text>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        delay = 3.0f;
        maximumFOV = 120.0f;
        minFOV = -65.0f; 
    }

    // Update is called once per frame
    void Update()
    {
        isSelected = false;

        Ray CameraRayCast = new Ray(camera.position, camera.rotation * Vector3.forward);

        RaycastHit hit;

        Debug.DrawRay(camera.position, camera.rotation * Vector3.forward * 200, Color.green);

        if (Physics.Raycast(CameraRayCast, out hit))
        {
            isSelected = true;

            if (hit.collider.tag == "ShowMainUI")
            {
                MainPanel.SetActive(true);  
            }

            else if (hit.collider.tag == "Exit" && timedClick)
            {
                Exit.SetActive(true);

                timer += Time.deltaTime;
                scaleFactor = timer / delay;
                ProgressBar.fillAmount = scaleFactor;

                if (timer >= delay)
                {
                    StartCoroutine(ExitFOV());
                }
            }

            else if (hit.collider.tag == "ShowVideo1")
            {
                VideoPanel1.SetActive(true);
                Video1Timer = 0.0f;
                Video1ProgressBar.fillAmount = 0.0f;
            }

            else if (hit.collider.transform.tag == "WatchVideo1")
            {
                Video1Timer += Time.deltaTime;
                Video1scaleFactor = Video1Timer / delay;
                Video1ProgressBar.fillAmount = Video1scaleFactor;


                if (Video1Timer >= delay)
                {
                    SceneManager.LoadScene(int.Parse(Video1ToLoad.text));
                }
            }

            else if (hit.collider.transform.tag == "DontWatch" && GameObject.Find("Loc/Canvas/Video1Panel"))
            {
                Video1Timer += Time.deltaTime;
                Video1scaleFactor = Video1Timer / delay;
                Video1ProgressBar.fillAmount = Video1scaleFactor;

                if (Video1Timer >= delay)
                {
                    VideoPanel1.SetActive(false);
                }
               
            }
        } 

        else
        {
            MainPanel.SetActive(false);
            Exit.SetActive(false);
            VideoPanel1.SetActive(false);
            timer = 0.0f;
            Video1Timer = 0.0f; 
            ProgressBar.fillAmount = 0.0f;
            Video1ProgressBar.fillAmount = 0.0f; 
        }
    }

    IEnumerator ExitFOV()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, maximumFOV, 0.05f);
        yield return new WaitForSeconds(0.4f);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, minFOV, 0.05f);
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
