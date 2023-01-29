using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class TextLog : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField]
    List<Message> messageList = new List<Message>();

    public GameObject logPannel, textObject;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void logMessage(string text, bool write)
    {
        if (!write)
            return;


        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, logPannel.transform);

        newMessage.textObject = newText.GetComponent<TMP_Text>();

        newMessage.textObject.text = newMessage.text;

        messageList.Add(newMessage);
    }

}

[System.Serializable]
public class Message
{
    public string text;
    public TMP_Text textObject;
}
