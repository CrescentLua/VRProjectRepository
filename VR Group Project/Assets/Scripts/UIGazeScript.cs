using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Valve.VR;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIGazeScript: MonoBehaviour
{
    //-- INITIALIZING VARIABLES --// 

    Button button;

    public bool isSelected; //Boolean variable to assure the button is being selected
    public bool timedClick = true; //Boolean variable to decide whether or not we want the button to have a timedclick
    public bool clicker = true; //Boolean variable that determines if the button is clickable  
    public bool highlight = true; //Boolean to check and switch between button highlights and a null selection  
    public bool isGazeBazed = true; //Boolean variable "isGazeBazed" decides whether or not we want the UI to be gaze based friendly or controller supported instead 
    bool SelectAudioPlayed = false; //Determines if audio has been played to avoid audio play loops (We call this a debounce when scripting in rblx.lua) 

    public string inputButton = "Fire1"; //Button string set for button clicking (We don't really use this so it might be removed in future updates) 

    public float delay; //Initialized delay float variable used to create a delay with timed selections 
    float timer; //float for the timer itself 
    float scaleFactor; //scaleFactor is the timer/delay to determine the size of fillAmount of the progressBar (Totally didn't steal the naming convention from last year)
    float volume; //float variable for audio volume 

    Transform camera; //Transform for the camera, this is what we set our raycast to  

    public Image ProgressBar; //Progress bar for timer used when the player looks at UI buttons 
    public TextMeshProUGUI TimerText; //Text for the timer 
    public TextMeshProUGUI VolumeText; //Text for volume

    public GameObject MainPanel; //Initializing the main panel (includes the start button, options, and quit) 
    public GameObject OptionsPanel; //Initializing the settings panel (includes volume changing, and back button) 

    AudioSource Intro; //Initializing introduction audio (robot that greets the user and gives them a heads up on what to do) 
    AudioSource Click; //Initializing click audio
    AudioSource Select; //Initializing select audio 

    //-- INITIALIZING VARIABLES --// 

    void Awake()
    {
        camera = Camera.main.transform; //This sets the camera to whatever camera in Unity that is currently active and being used 
        MainPanel = GameObject.Find("Menu/Canvas/Main/MainPanel");
        OptionsPanel = GameObject.Find("Menu/Canvas/Main/Settings");
        VolumeText = GameObject.Find("Menu/Canvas/Main/Settings/VolumeAmount").GetComponent<TextMeshProUGUI>();
        Intro = GameObject.Find("AudioClips/Intro").GetComponent<AudioSource>();
        Click = GameObject.Find("AudioClips/Click").GetComponent<AudioSource>();
        Select = GameObject.Find("AudioClips/Select").GetComponent<AudioSource>();
    }

    void Start()
    {
        //-- SETTING DEFAULT VALUES FOR VARIABLES --// 
        button = GetComponent<Button>();
        delay = 3.0f; 
        volume = 1.0f; 
        Intro.Play(); //Playing introduction audio
        //-- SETTING DEFAULT VALUES FOR VARIABLES --// 
    }

    void Update()
    {
        isSelected = false; //Automatically set isSelected boolean variable to false 

        Ray CameraRayCast = new Ray(camera.position, camera.rotation * Vector3.forward); //Creating our camera raycast and attaching it to the camera's position/rotation

        RaycastHit hit; //Creating a RaycastHit named "hit" to determine when the raycast collides with something (Great naming convention, I know) 

        Debug.DrawRay(camera.position, camera.rotation * Vector3.forward * 200, Color.green); //Created a visual reprensentation of the raycast to the camera's position & rotation facing 200 units forward and coloured green 

        //-- Conditional IF statement to determine when the raycast hits another object --//
        if (Physics.Raycast(CameraRayCast, out hit))
        {

            isSelected = true;

            if (timedClick && isGazeBazed == true) //If timed click and isGazedBazed then increase the timer, progress bar, and calculate the scaleFactor in order to do so
            {
                timer += Time.deltaTime;
                scaleFactor = timer / delay;
                TimerText.text = timer.ToString();
                ProgressBar.fillAmount = scaleFactor;

                button = hit.collider.transform.parent.GetComponent<Button>(); //Set the new button variable to whatever button the raycast might've hit along the way 
                button.Select(); //Select the button 

                if (SelectAudioPlayed == false) //Debounce created for playing audio
                {
                    Select.Play();
                    SelectAudioPlayed = true;
                }

                //If the raycast finds what we're looking for, then invoke a button click
                if (timer >= delay && hit.collider != null && hit.collider.transform.parent.tag.Equals("UIButton") && isGazeBazed == true) 
                {
                    button.onClick.Invoke();
                }
            }
        }

        else //RESET all the variables such as timer, progress bar, etc. 
        {
            EventSystem.current.SetSelectedGameObject(null);
            timer = 0.0f;
            TimerText.text = "0.00";
            scaleFactor = 0.0f;
            ProgressBar.fillAmount = 0.0f;
            SelectAudioPlayed = false;
        }
        //-- END of conditional IF statement to determine when the raycast hits another object --//
    }

    //-- PUBLIC METHODS ("VOIDS") WE USE WHEN THE PLAYER CLICKS A BUTTON --//
    public void StartGame()
    {
        if (timer >= delay) //If the timer exceeds the delay, then start the game and reset the timer and any other variable shown below
        {
            Click.Play();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("Button Pressed!");
            timer = 0.0f;
            TimerText.text = "0.00";
            ProgressBar.fillAmount = 0.0f;
            SelectAudioPlayed = false;
        }
    }

    public void OpenOptions()  
    {
        if (timer >= delay) //If the timer exceeds the delay, then open up options panel and close the main panel (resets variables as well) 
        {
            Click.Play();

            MainPanel.SetActive(false);
            OptionsPanel.SetActive(true);
            timer = 0.0f;
            TimerText.text = "0.00";
            ProgressBar.fillAmount = 0.0f;
            SelectAudioPlayed = false;
        }
    }

    public void CloseOptions()
    {
        if (timer >= delay) //If the timer exceeds the delay, then close the options panel and open the main panel (resets variables as well) 
        {
            Click.Play();

            OptionsPanel.SetActive(false);
            MainPanel.SetActive(true);
            timer = 0.0f;
            TimerText.text = "0.00";
            ProgressBar.fillAmount = 0.0f;
            SelectAudioPlayed = false;
        }
    }

    public void ExitGame()
    {
        if (timer >= delay) //If the timer exceeds the delay, then close the game (The game won't close if you're testing in the Unity Editor so we reset variables here too just to make things clean)
        {
            Click.Play();

            Application.Quit();
            timer = 0.0f;
            TimerText.text = "0.00";
            ProgressBar.fillAmount = 0.0f;
            SelectAudioPlayed = false;
        }
    }

    public void IncreaseVolume()
    {
        if (timer >= delay) //If the timer exceeds the delay, then INCREASE audio volume and set the volume text to match up with the current audio levels (resets other variables as well)
        {
            Click.Play();

            volume = volume + 0.5f;
            VolumeText.text = "(" + volume.ToString() + ")";

            Intro.volume = volume;
            Click.volume = volume;
            Select.volume = volume;


            timer = 0.0f;
            TimerText.text = "0.00";
            ProgressBar.fillAmount = 0.0f;
            SelectAudioPlayed = false;
        }
    }

    public void DecreaseVolume()
    {
        if (timer >= delay && volume > 0.0f) //If the timer exceeds the delay, then DECREASE audio volume and set the volume text to match up with the current audio levels (resets other variables as well)
        {
            Click.Play();

            volume = volume - 0.5f;
            VolumeText.text = "(" + volume.ToString() + ")";

            Intro.volume = volume;
            Click.volume = volume;
            Select.volume = volume;

            timer = 0.0f;
            TimerText.text = "0.00";
            ProgressBar.fillAmount = 0.0f;
            SelectAudioPlayed = false;
        }
    }
    //-- PUBLIC METHODS ("VOIDS") WE USE WHEN THE PLAYER CLICKS A BUTTON --//
}