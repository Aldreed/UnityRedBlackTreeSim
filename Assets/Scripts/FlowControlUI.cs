using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FlowControlUI : MonoBehaviour
{

    [SerializeField] NodeTree tree = null;
    Button playButton;
    Button fastButton;
    Button pauseButton;
    Button backButton;
    Slider flowSlider;

    public enum Op
    {
        Insert,
        Delete
    }

    public Op curop;

    private void Awake()
    {
        playButton = transform.Find("PlayButton").GetComponent<Button>();
        fastButton = transform.Find("FastfowardButton").GetComponent<Button>();
        pauseButton = transform.Find("PauseButton").GetComponent<Button>();
        backButton = transform.Find("BackButton").GetComponent<Button>();
        flowSlider = transform.Find("Slider").GetComponent<Slider>();
    }

    private void setLiseners()
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => {
            if (curop == Op.Insert)
            {
                tree.once = true;
            }
            else
            {
                tree.delete = true;
            }
        });
        fastButton.onClick.RemoveAllListeners();
        fastButton.onClick.AddListener(() => {
            if (curop == Op.Insert)
            {
                tree.skip = true;
            }
            else
            {
                tree.skipDelete = true;
            }
        });
        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(() => {
            if (curop == Op.Insert)
            {
                tree.skip= false;
            }
            else
            {
                tree.skipDelete = false;
            }
        });


        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => {
            tree.skip = false;
            tree.skipDelete = false;
            tree.once = false;
            tree.delete = false;
            tree.stepBack = true;
        });

    }

    // Start is called before the first frame update
    void Start()
    {
        setLiseners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
