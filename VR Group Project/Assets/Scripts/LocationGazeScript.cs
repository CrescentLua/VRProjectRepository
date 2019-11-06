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
    float Video2Timer;
    float Video2scaleFactor; 
    float scaleFactor;
    float maximumFOV;
    float minFOV; 

    Transform camera;

    public Image ProgressBar;
    public Image Video1ProgressBar;
    public Image Video2ProgressBar; 

    public GameObject MainPanel;
    public GameObject FirstPanel;
    public GameObject SecondPanel; 
    public GameObject Exit;

    public Text Video1ToLoad;
    public Text Video2ToLoad; 

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

        if(GameObject.Find("Loc/Canvas/FirstPanel")) 
        {
            FirstPanel = GameObject.Find("Loc/Canvas/FirstPanel");
        }

        if (GameObject.Find("Loc/Canvas/SecondPanel"))
        {
            SecondPanel = GameObject.Find("Loc/Canvas/SecondPanel");
        }

        if (GameObject.Find("Loc/Canvas/FirstPanel/SceneToLoad"))
        {
            Video1ToLoad = GameObject.Find("Loc/Canvas/FirstPanel/SceneToLoad").GetComponent<Text>();
        }

        if (GameObject.Find("Loc/Canvas/SecondPanel/SceneToLoad"))
        {
            Video2ToLoad = GameObject.Find("Loc/Canvas/SecondPanel/SceneToLoad").GetComponent<Text>();
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

                if (timer >= delay && SceneManager.GetActiveScene().buildIndex < 6)
                {
                    StartCoroutine(ExitFOV());
                }    

                else if (timer >= delay && SceneManager.GetActiveScene().buildIndex > 5)
                {
                    SceneManager.LoadScene("Introduction");
                }
            }

            else if (hit.collider.tag == "ShowVideo1")
            {
                FirstPanel.SetActive(true);
                Video1Timer = 0.0f;
                Video1ProgressBar.fillAmount = 0.0f;
            }

            else if (hit.collider.tag == "ShowVideo2")
            {
                SecondPanel.SetActive(true);
                Video2Timer = 0.0f;
                Video2ProgressBar.fillAmount = 0.0f;
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

            else if (hit.collider.transform.tag == "WatchVideo2")
            {
                Video2Timer += Time.deltaTime;
                Video2scaleFactor = Video2Timer / delay;
                Video2ProgressBar.fillAmount = Video2scaleFactor; 

                if (Video2Timer >= delay)
                {
                    SceneManager.LoadScene(int.Parse(Video2ToLoad.text));
                }
            }
        } 

        else
        {
            if (MainPanel)
            {
                MainPanel.SetActive(false);
            }

            Exit.SetActive(false);
            FirstPanel.SetActive(false);
            Video1Timer = 0.0f;
            Video1ProgressBar.fillAmount = 0.0f;
            timer = 0.0f;
            ProgressBar.fillAmount = 0.0f;
            SecondPanel.SetActive(false);        
            Video2Timer = 0.0f;
            Video2ProgressBar.fillAmount = 0.0f;            
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
