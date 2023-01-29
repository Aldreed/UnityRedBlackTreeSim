using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpWindow : MonoBehaviour
{

    TMP_InputField inputFieldName;
    TMP_Text placeholder;
    [SerializeField] NodeTree tree;

    private void Awake()
    {
        inputFieldName = transform.Find("InputName").GetComponent<TMP_InputField>();
        placeholder = inputFieldName.transform.Find("Text Area").Find("Placeholder").GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void show()
    {
        this.gameObject.SetActive(true);
    }

    public void hide()
    {
        this.gameObject.SetActive(false);
    }

    public void save()
    {
        string s = inputFieldName.text;
        inputFieldName.text = "";
        if(s.Length == 0)
        {
            placeholder.text = "Enter file name";
            return;
        }

        s = s + ".rb";

        SaveLoadSystem.saveTree(tree, false, s);

        placeholder.text = "File saved";
        
    }

    public void load()
    {
        string s = inputFieldName.text;
        inputFieldName.text = "";
        if (s.Length == 0)
        {
            placeholder.text = "Enter file name";
        }

        s = s + ".rb";

        

        if (!SaveLoadSystem.loadTree(tree, 10, false, s))
        {
            placeholder.text = "File not found ";
            return;
        }

        if (tree.root != null) tree.root.GetComponent<NodeScript>().updateTreeStructure();

        placeholder.text = "File loaded";
        return;
    }

}
