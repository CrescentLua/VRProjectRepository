using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoBackAfterVideo : MonoBehaviour
{
    public VideoPlayer VideoPlayer; // Drag & Drop the GameObject holding the VideoPlayer component
    public Text SceneToLoad;

    void Awake()
    {
        if (GameObject.Find("VideoSphere/Scene"))
        {
            SceneToLoad = GameObject.Find("VideoSphere/Scene").GetComponent<Text>();
        }
    }

    void Start()
    {
        VideoPlayer.loopPointReached += LoadScene;


    }

    void LoadScene(VideoPlayer vp)
    {
        SceneManager.LoadScene(int.Parse(SceneToLoad.text));
    }
}


