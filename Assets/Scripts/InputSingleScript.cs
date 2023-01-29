using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class InputSingleScript : MonoBehaviour
{
    TMP_InputField inputFieldInsert;
    TMP_Text placeholderInsert;
    Button buttonInsert;
    [SerializeField] NodeTree tree = null;

    TMP_InputField inputFieldDelete;
    Button buttonDelete;
    TMP_Text placeholderDelete;

    Button buttonClearTree;
    Button buttonSave;
    Button buttonLoad;


    public FlowControlUI flowControl;

    public enum Operation
    {
        Insert,
        Delete
    }

    private void Awake()
    {
        inputFieldInsert = transform.Find("InputFieldInsert").GetComponent<TMP_InputField>();
        buttonInsert = transform.Find("ButtonInsert").GetComponent<Button>();
        placeholderInsert = inputFieldInsert.transform.Find("Text Area").Find("Placeholder").GetComponent<TMP_Text>();

        inputFieldDelete = transform.Find("InputFieldDelete").GetComponent<TMP_InputField>();
        buttonDelete = transform.Find("ButtonDelete").GetComponent<Button>();
        placeholderDelete = inputFieldInsert.transform.Find("Text Area").Find("Placeholder").GetComponent<TMP_Text>();

        buttonClearTree = transform.Find("ButtonClearTree").GetComponent<Button>();
        buttonSave = transform.Find("ButtonSave").GetComponent<Button>();
        buttonLoad = transform.Find("ButtonLoad").GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {

        test();
    }


    public void deactivateInput()
    {
        buttonInsert.interactable = false;
        inputFieldInsert.interactable = false;

        buttonDelete.interactable = false;
        inputFieldDelete.interactable = false;

        buttonClearTree.interactable = false;
        buttonSave.interactable = false;
        buttonLoad.interactable = false;
    }

    public void activateInput()
    {
        buttonInsert.interactable = true;
        inputFieldInsert.interactable = true;

        buttonDelete.interactable = true;
        inputFieldDelete.interactable = true;

        buttonClearTree.interactable = true;
        buttonSave.interactable = true;
        buttonLoad.interactable = true;

    }

    public void sendInput(List<int> inList, Operation op)
    {
        if (op == Operation.Insert) { 
            tree.insertArray(inList);
            flowControl.curop = FlowControlUI.Op.Insert;
        } else {
            flowControl.curop = FlowControlUI.Op.Delete;
            tree.deleteArray(inList);
        }

    }

    public void test()
    {

        buttonInsert.onClick.RemoveAllListeners();

        buttonInsert.onClick.AddListener(() =>
        {
            int temp;
            string[] inputs;
            //Debug.Log(inputField.text);
            inputs = inputFieldInsert.text.Split(' ');
            List<int> inputsList = new List<int>();
            foreach (string item in inputs)
            {
                if (System.Int32.TryParse(item, out temp))
                {

                    inputFieldInsert.text = "";
                    placeholderInsert.text = "Enter value";
                    inputsList.Add(temp);
                    Debug.Log(temp);
                }
                else
                {
                    inputFieldInsert.text = "";
                    placeholderInsert.text = "Invalid input";
                };
            }

            if (inputsList.Count > 0)
            {
                sendInput(inputsList, Operation.Insert);
                deactivateInput();
            }


        });


        inputFieldInsert.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("-1234567890 ", addedChar, charIndex);
        };

        buttonDelete.onClick.RemoveAllListeners();

        buttonDelete.onClick.AddListener(() => {
            int temp;
            string[] inputs;
            //Debug.Log(inputField.text);
            inputs = inputFieldDelete.text.Split(' ');
            List<int> inputsList = new List<int>();
            foreach (string item in inputs)
            {
                if (System.Int32.TryParse(item, out temp))
                {

                    inputFieldDelete.text = "";
                    placeholderDelete.text = "Enter value";
                    inputsList.Add(temp);
                    Debug.Log(temp);
                }
                else
                {
                    inputFieldDelete.text = "";
                    placeholderDelete.text = "Invalid input";
                };
            }

            if (inputsList.Count > 0)
            {
                sendInput(inputsList, Operation.Delete);
                deactivateInput();
            }

        });

        inputFieldDelete.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("-1234567890 ", addedChar, charIndex);
        };

    }

    private char ValidateChar(string ValidChar, char addedChar, int charIndex)
    {

        if (charIndex == 0 && addedChar == ' ')
        {
            return '\0';
        }
        else if (ValidChar.IndexOf(addedChar) != -1)
        {
            return addedChar;
        }
        else
        {
            return '\0';
        }

    }

    public void takeScreenShot()
    {

        string dirName = "Screenshots";
        string fileName = "screenShot.png";

        DirectoryInfo info = Directory.CreateDirectory(dirName);
        string fullName = Path.Combine(info.FullName, fileName);

        GameObject o= GameObject.Find("Canvas");

        o.SetActive(false);
        ScreenCapture.CaptureScreenshot(fullName);
        o.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
