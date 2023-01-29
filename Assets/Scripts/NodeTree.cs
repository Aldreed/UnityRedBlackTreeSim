using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeTree : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject NodePrefab;

    public int initalNodes = 7;

    public List<int> nodeValues;

    public List<int> deleteValues;

    public GameObject root = null;

    public bool once = false;

    public bool skip = false;

    public bool delete = false;

    public bool skipDelete = false;

    public int ValueToDelete = 0;

    public int ValueToAdd = 0;

    public GameObject InputUI;

    public TextLog textLog;

    public bool writeToLog = true;

    public Slider flowSlider;

    public TMPro.TMP_Text TextArray;

    public float TurnTime = 1f;

    private float TimePassed = 0;


    public enum ActionStateInsert
    {
        AwaitingAction,
        InsertSearch,
        InsertFixup,
        InsertSecondCall,
        None
    }

    public enum ActionStateDelete
    {
        AwaitingAction,
        DeleteSearch,
        DeleteOne,
        DeleteChildTemp,
        Case1,
        Case2,
        Case3,
        Case4,
        Case5,
        Case6,
        None
    }

    void Awake()
    {
        TextArray.text = "";

    }

    void Start()
    {

        nodeValues.Clear();
        deleteValues.Clear();

    }
    int i = 0;
    int j = 0;
    int stepNumber = 0;
    int snapShotIndex = 0;


    ActionStateInsert globalActionStateInsert = ActionStateInsert.AwaitingAction;

    ActionStateDelete globalActionStateDelete = ActionStateDelete.AwaitingAction;

    int item;
    GameObject curNode;
    Transform parent;
    Transform gParent;
    NodeScript.NodeDirection dir;
    bool tempChild = false;
    NodeScript tempScript = null;

    [SerializeField] bool testing = false;

    public Stack<ActionData> actionStack = new Stack<ActionData>();

    public bool stepBack = false;
    public bool stepSkip = false;


    private void endInsertAction()
    {

        logMessage("Ending insert action");
        
        PushToActionStack(curNode, 0, 0, i, globalActionStateInsert, 0, nodeValues, ActionData.Action.endinsert);

        i++;
        curNode = null;
        globalActionStateInsert = ActionStateInsert.AwaitingAction;

        if (i == nodeValues.Count)
        {

            InputUI.GetComponent<InputSingleScript>().activateInput();
            //nodeValues.Clear();
            nodeValues = new List<int>();
            i = 0;
            once = false;
            skip = false;
            if (root != null) root.GetComponent<NodeScript>().updateTreeStructure();
            else
                updateVisualArray(deleteValues, -1);
            //updateVisualArray(nodeValues, -1);
            //curNode.GetComponent<NodeScript>().disableSelect();

            //updateVisualArray(nodeValues, -1);
        }

    }

    private void endInsertAction_inverse(ActionData ad)
    {
        i = ad.oldValue;
        globalActionStateInsert = ad.curActionStateInsert;
        nodeValues = ad.valueList;
        curNode = ad.node;
        once = false;
        skip = false;
        InputUI.GetComponent<InputSingleScript>().deactivateInput();
    }

    private void changeCurNode(GameObject g)
    {
        PushToActionStack(curNode, 0, 0, 0, 0, 0, null, ActionData.Action.changeCurNode);
        
        curNode = g;
    }

    private void changeCurNode_inverse(ActionData ad)
    {
        curNode = ad.node;
    }

    private void endDeleteAction()
    {
        j++;
        curNode = null;
        globalActionStateDelete = ActionStateDelete.AwaitingAction;

        logMessage("Ending delete action");

        if (j == deleteValues.Count)
        {
            j = 0;
            //deleteValues.Clear();
            deleteValues = new List<int>();
            InputUI.GetComponent<InputSingleScript>().activateInput();
            delete = false;
            skipDelete = false;
            if(root!=null)root.GetComponent<NodeScript>().updateTreeStructure();
            else
                updateVisualArray(deleteValues, -1);
        }
        
    }

    bool nwItem = false;
    // Update is called once per frame
    void Update()
    {

        TimePassed += Time.deltaTime;

        if ((once || (skip && TimePassed >= TurnTime)) && i < nodeValues.Count && testing) {

            chumpBlockOnStack();
            stepNumber++;
            GameObject temp;
            switch (globalActionStateInsert)
            {
                case ActionStateInsert.AwaitingAction:
                    logMessage("Inserting node: " + nodeValues[i]);
                    if (root==null)
                    {
                        
                        root = rootSet(nodeValues[i]);
                        endInsertAction();
                    }
                    else
                    {
                        //globalActionStateInsert = ActionStateInsert.InsertSearch;
                        globalActionStateInsertChange(ActionStateInsert.InsertSearch);
                        //item = nodeValues[i];
                        updateItem(nodeValues[i]);
                        //curNode = root;
                        changeCurNode(root);



                        if (curNode.GetComponent<NodeScript>().findNextForInsert_Step(item, out temp)) {
                            changeCurNode(temp);
                            //globalActionStateInsert = ActionStateInsert.InsertFixup;
                            globalActionStateInsertChange(ActionStateInsert.InsertFixup);
                            root.GetComponent<NodeScript>().updateTreeStructure();
                            logMessage("Starting fixup");
                        }
                        else if (temp == null)
                        {
                            changeCurNode(temp);
                            updateVisualArray(nodeValues, i + 1);
                            endInsertAction();
                            break;
                        }
                        else
                        {
                            changeCurNode(temp);
                            //globalActionStateInsert = ActionStateInsert.InsertSearch;
                            globalActionStateInsertChange(ActionStateInsert.InsertSearch);
                        };
                        curNode.GetComponent<NodeScript>().enableSelect();

                    }
                    updateVisualArray(nodeValues, i+1);
                    break;
                case ActionStateInsert.InsertSearch:
                    curNode.GetComponent<NodeScript>().disableSelect();
                    if (curNode.GetComponent<NodeScript>().findNextForInsert_Step(item, out temp))
                    {
                        changeCurNode(temp);
                        //globalActionStateInsert = ActionStateInsert.InsertFixup;
                        globalActionStateInsertChange(ActionStateInsert.InsertFixup);
                        root.GetComponent<NodeScript>().updateTreeStructure();
                    }
                    else if (temp == null)
                    {
                        changeCurNode(temp);
                        endInsertAction();

                        break;
                    }
                    else
                    {
                        changeCurNode(temp);
                        //globalActionStateInsert = ActionStateInsert.InsertSearch;
                        globalActionStateInsertChange(ActionStateInsert.InsertSearch);
                    };
                    curNode.GetComponent<NodeScript>().enableSelect();
                    break;
                case ActionStateInsert.InsertFixup:
                    Transform TempTransform;
                    NodeScript.NodeDirection tempDir;
                    curNode.GetComponent<NodeScript>().disableSelect();
                    if (curNode.GetComponent<NodeScript>().insertFixup_step(this.gameObject, curNode.transform, out TempTransform, out parent, out gParent, out tempDir))
                    {
                        endInsertAction();
                        //if(curNodecurNode.GetComponent<NodeScript>()) curNode.GetComponent<NodeScript>().disableSelect();
                    }
                    else if(parent!=null)
                    {
                        //globalActionStateInsert = ActionStateInsert.InsertSecondCall;
                        globalActionStateInsertChange(ActionStateInsert.InsertSecondCall);
                        //curNode = temp.gameObject;
                        changeCurNode(TempTransform.gameObject);
                        setDir(tempDir);
                        curNode.GetComponent<NodeScript>().enableSelect();

                    }
                    else
                    {
                        //globalActionStateInsert = ActionStateInsert.InsertFixup;
                        globalActionStateInsertChange(ActionStateInsert.InsertFixup);
                        //curNode = temp.gameObject;
                        changeCurNode(TempTransform.gameObject);
                        if (this.gameObject != curNode)curNode.GetComponent<NodeScript>().enableSelect();

                    }
                    break;
                case ActionStateInsert.InsertSecondCall:
                    curNode.GetComponent<NodeScript>().disableSelect();
                    curNode.GetComponent<NodeScript>().insertFixup_CaseX(this.gameObject, curNode.transform, parent, gParent, out TempTransform, dir);
                    //globalActionStateInsert = ActionStateInsert.InsertFixup;
                    globalActionStateInsertChange(ActionStateInsert.InsertFixup);
                    //parent = null;
                    setParent(null);
                    //gParent = null;
                    setGParent(null);
                    //curNode = temp.gameObject;
                    changeCurNode(TempTransform.gameObject);
                    curNode.GetComponent<NodeScript>().enableSelect();
                    break;
                case ActionStateInsert.None:
                    endInsertAction();
                    break;
                default:
                    break;
            }

            once = false;
            TimePassed = 0;

        }


        if (((skipDelete && TimePassed >= TurnTime) || delete) && j < deleteValues.Count && testing)
        {
            stepNumber++;
            bool CasesDone = false;
            switch (globalActionStateDelete)
            {
                case ActionStateDelete.AwaitingAction:
                    logMessage("Deleting node: " + deleteValues[j]);
                    if (root != null)
                    {
                        item = deleteValues[j];
                        curNode = root;

                        if (curNode.GetComponent<NodeScript>().delete_step(curNode, item, root, out curNode, out nwItem))
                        {
                            globalActionStateDelete = ActionStateDelete.DeleteOne;
                        }
                        else if (curNode == null) {
                            logMessage("No node found");
                            updateVisualArray(deleteValues, j+1);
                            endDeleteAction();
                            break;
                        }
                        else {
                            globalActionStateDelete = ActionStateDelete.DeleteSearch;
                            if (nwItem) { item = curNode.GetComponent<NodeScript>().val; nwItem = false; }
                        };
                        updateVisualArray(deleteValues, j + 1);
                        curNode.GetComponent<NodeScript>().enableSelect();
                    }
                    else
                    {
                        Debug.Log("Can't delete: Tree Empty");
                        logMessage("Can't delete: Tree Empty");
                        endDeleteAction();
                    }
                    break;
                case ActionStateDelete.DeleteSearch:
                    curNode.GetComponent<NodeScript>().disableSelect();
                    if (curNode.GetComponent<NodeScript>().delete_step(curNode, item, this.gameObject, out curNode,out nwItem))
                    {
                        globalActionStateDelete = ActionStateDelete.DeleteOne;
                    }
                    else if (curNode == null)
                    {
                        logMessage("No node found");
                        endDeleteAction();
                        break;
                    }
                    else
                    {
                        globalActionStateDelete = ActionStateDelete.DeleteSearch;
                        if (nwItem) { item = curNode.GetComponent<NodeScript>().val; nwItem = false; }
                    };
                    curNode.GetComponent<NodeScript>().enableSelect();
                    break;
                case ActionStateDelete.DeleteOne:
                    if (curNode.GetComponent<NodeScript>().deleteOne_step(curNode, this.gameObject, out tempScript,out tempChild))
                    {
                        if(root!=null)root.GetComponent<NodeScript>().updateTreeStructure();
                        endDeleteAction();
                    }
                    else
                    {
                        logMessage("Checking Delete Cases");
                        globalActionStateDelete = ActionStateDelete.Case1;
                    }
                    break;
                case ActionStateDelete.Case1:
                    if(tempScript.deleteCase1_step(tempScript, this.gameObject))
                    {
                        CasesDone = true;
                        break;
                    }
                    else if (tempScript.deleteCase2_step(tempScript, this.gameObject)){
                        globalActionStateDelete = ActionStateDelete.Case3;
                        break;
                    }
                    goto case ActionStateDelete.Case3;
                case ActionStateDelete.Case3:
                    GameObject parentInTree;
                    if (tempScript.deleteCase3_step(tempScript, this.gameObject, out parentInTree))
                    {
                        globalActionStateDelete = ActionStateDelete.Case1;
                        if (tempChild)
                        {
                            curNode.GetComponent<NodeScript>().cleanupDelete(curNode, tempScript, tempChild);
                            tempScript = null;
                            tempChild = false;
                        }
                        curNode = parentInTree;
                        tempScript = curNode.GetComponent<NodeScript>();
                        break;
                    }
                    else if (tempScript.deleteCase4_step(tempScript, this.gameObject))
                    {
                        CasesDone = true;
                        break;
                    }
                    else if (tempScript.deleteCase5_step(tempScript, this.gameObject))
                    {
                        globalActionStateDelete = ActionStateDelete.Case6;
                        break;
                    }
                    goto case ActionStateDelete.Case6;
                case ActionStateDelete.Case6:
                    if (tempScript.deleteCase6_step(tempScript, this.gameObject))
                    {
                        CasesDone = true;
                        break;
                    }
                    break;
                default:
                    break;
            }

            if (CasesDone)
            {
                
                if (tempChild) curNode.GetComponent<NodeScript>().cleanupDelete(curNode, tempScript, tempChild);
                endDeleteAction();
                tempScript = null;
                tempChild = false;
            }
            delete = false;
            TimePassed = 0;
        }




            if (!testing)
        {

            if ((once || (skip && TimePassed >= TurnTime)) && i < nodeValues.Count)
            {

                int item = nodeValues[i++];

                if (transform.childCount == 0)
                {
                    root = rootSet(item);
                }
                else
                {
                    root.GetComponent<NodeScript>().findNextForInsert(item);
                }


                if (nodeValues.Count != 0)
                {
                    root.GetComponent<NodeScript>().updateTreeStructure();
                }
                updateVisualArray(nodeValues, i);
                TimePassed = 0;
                //if (i == nodeValues.Count && (once || skip))
                //{
                //    InputUI.GetComponent<InputSingleScript>().activateInput();
                //    nodeValues.Clear();
                //    i = 0;
                //    once = false;
                //    skip = false;
                //    updateVisualArray(nodeValues, -1);
                //}
                once = false;
            }
            else if (i == nodeValues.Count && (once || skipDelete))
            {
                InputUI.GetComponent<InputSingleScript>().activateInput();
                nodeValues.Clear();
                i = 0;
                once = false;
                skip = false;
                updateVisualArray(nodeValues, -1);
            }

            if (((skipDelete && TimePassed >= TurnTime) || delete) && j < deleteValues.Count)
            {
                int item = deleteValues[j++];
                deleteNode(item);
                if (root != null)
                    root.GetComponent<NodeScript>().updateTreeStructure();
                delete = false;
                updateVisualArray(deleteValues, j);
                TimePassed = 0;
            }
            else if (j >= deleteValues.Count && (delete || skipDelete))
            {
                j = 0;
                deleteValues.Clear();
                InputUI.GetComponent<InputSingleScript>().activateInput();
                delete = false;
                skipDelete = false;
                updateVisualArray(deleteValues, -1);
            }

            //if (delete)
            //{
            //    deleteNode(ValueToDelete);
            //    if(root!=null)
            //    root.GetComponent<NodeScript>().updateTreeStructure();
            //    delete = false;
            //}

        }


        if (stepBack && !skip && !once && !delete && !skipDelete) {
            logMessage("Steping back");
            writeToLog = false;
            loadStepInsert();
            stepBack = false;
            skip = false;
            skipDelete = false;
            once = false;
            delete = false;
            TimePassed = 0;
            writeToLog = true;
        }

        if (false && !skip && !once && !delete && !skipDelete)
        {
            if(actionStack.Count == 0)
            {
                Debug.Log("Stack Empty");
                return;
            }

            ActionData ad = actionStack.Pop();

            while(ad.currentAction != ActionData.Action.chumpBlock)
            {
                switch (ad.currentAction)
                {
                    case ActionData.Action.changeCurNode:
                        changeCurNode_inverse(ad);
                        break;
                    case ActionData.Action.rootSet:
                        rootSet_inverse(ad);
                        break;
                    case ActionData.Action.changeColor:
                        ad.node.GetComponent<NodeScript>().changeColor_inverse(ad);
                        break;
                    case ActionData.Action.rotate:
                        if(ad.dir == NodeScript.NodeDirection.Left)
                            ad.node.GetComponent<NodeScript>().rotateLeft_inverse(this.gameObject);
                        else
                            ad.node.GetComponent<NodeScript>().rotateRight_inverse(this.gameObject);
                        break;
                    case ActionData.Action.insert:
                        ad.node.GetComponent<NodeScript>().insert_inverse(ad);
                        break;
                    case ActionData.Action.endinsert:
                        endInsertAction_inverse(ad);
                        break;
                    case ActionData.Action.updateChildCountMinusOne:
                        ad.node.GetComponent<NodeScript>().updateChildCount_inverse(ad);
                        break;
                    case ActionData.Action.updateValue:
                        ad.node.GetComponent<NodeScript>().updateValue_inverse(ad);
                        break;
                    case ActionData.Action.updateVisualArray:
                        updateVisualArray_inverse(ad);
                        break;
                    case ActionData.Action.enableSelect:
                        ad.node.GetComponent<NodeScript>().enableSelect_inverse();
                        break;
                    case ActionData.Action.disableSelect:
                        ad.node.GetComponent<NodeScript>().disableSelect_inverse();
                        break;
                    case ActionData.Action.itemUpdate:
                        updateItem_inverse(ad);
                        break;
                    case ActionData.Action.findNextForInsert:
                        ad.node.GetComponent<NodeScript>().findNextForInsert_inverse();
                        break;
                    case ActionData.Action.globalActionStateInsertChange:
                        globalActionStateInsertChange_inverse(ad);
                        break;
                    case ActionData.Action.globalActionStateDeleteChange:
                        globalActionStateDeletetChange_inverse(ad);
                        break;
                    case ActionData.Action.setParent:
                        setParent_inverse(ad);
                        break;
                    case ActionData.Action.setGParent:
                        setGParent_inverse(ad);
                        break;
                    case ActionData.Action.chumpBlock:
                        break;
                    case ActionData.Action.insertArray:
                        insertArray_inverse();
                        break;
                    case ActionData.Action.saveDir:
                        setDir_inverse(ad);
                        break;
                    case ActionData.Action.deleteArrayNI:
                        break;
                    default:
                        break;
                }

                if (actionStack.Count != 0)
                {
                    ad = actionStack.Pop();
                }
                else
                {
                    Debug.LogError("Stack Empty");
                    return;
                }
            }

            stepBack = false;

        }


    }


    private void saveStepInsert()
    {

        SaveLoadSystem.saveTree(this, true, "tree");
    }

    private void loadStepInsert()
    {

        if (nodeValues.Count == 0 && deleteValues.Count == 0) return;

        clearTree();
        SaveLoadSystem.loadTree(this, 10, true,"tree");
        if (root != null) root.GetComponent<NodeScript>().updateTreeStructure();

        int tempSteps = --stepNumber;
        if (tempSteps == -1)
        {
            deleteValues.Clear();
            nodeValues.Clear();
            i = 0;
            j = 0;
            InputUI.GetComponent<InputSingleScript>().activateInput();
            updateVisualArray(nodeValues, -1);
            return;
        }

        stepNumber = 0;
        stepBack = false;
        globalActionStateInsert = ActionStateInsert.AwaitingAction;
        globalActionStateDelete = ActionStateDelete.AwaitingAction;

        if (nodeValues.Count > 0)
        {
            i = 0;
            while (stepNumber<tempSteps)
            {
                once = true;
                Update();
            }
            if (tempSteps == 0)
            {
                updateVisualArray(nodeValues, i);
            }
        }
        else
        {
            j = 0;
            while (stepNumber != tempSteps)
            {
                delete = true;
                Update();
            }
            if (tempSteps == 0)
            {
                updateVisualArray(deleteValues, j);
            }
        }
        if (root != null) root.GetComponent<NodeScript>().updateTreeStructure();
    }

    private GameObject rootSet(int i)
    {
        logMessage("Setting root");

        GameObject o = Instantiate(NodePrefab, new Vector3(0, 1, 0), Quaternion.identity, this.transform);
        o.transform.localScale = new Vector3(10, 10, 10);
        NodeScript ns = o.GetComponent<NodeScript>();
        ns.treeRoot = this.gameObject; 
        ns.NodePrefab = NodePrefab;
        ns.updateValue(i);
        ns.changeColor(o, NodeScript.NodeColor.Black);
        o.name = "Node " + NodeScript.nCount++;

        PushToActionStack(o, 0, 0, 0, 0, 0, null, ActionData.Action.rootSet);
        
        
        return o;
    }

    private void rootSet_inverse(ActionData ad)
    {
        
        Destroy(ad.node);

        root = null;
    }


    private void deleteNode(int value)
    {
        if (root != null)
        {
            root.GetComponent<NodeScript>().delete(root, value, this.gameObject);
        }
        else
        {
            Debug.Log("Can't delete. Tree empty");
        }
    }

    public void insertArray(List<int> inputsList)
    {
        chumpBlockOnStack();
        nodeValues = new List<int>();
        foreach (int item in inputsList)
        {
            nodeValues.Add(item);
        }
        updateVisualArray(nodeValues, 0);
        PushToActionStack(null, 0, 0, 0, 0, 0, null, ActionData.Action.insertArray);
        stepNumber = 0;
        saveStepInsert();
    }

    public void insertArray_inverse()
    {
        nodeValues = new List<int>();
        updateVisualArray(null, -1);
        InputUI.GetComponent<InputSingleScript>().activateInput();
    }

    public void deleteArray(List<int> inputsList)
    {
        deleteValues = inputsList;
        updateVisualArray(deleteValues, 0);
        stepNumber = 0;
        saveStepInsert();
    }

    public void updateVisualArray(List<int> inputList,int index)
    {
        PushToActionStack(null, 0, 0, index, 0, 0, inputList, ActionData.Action.updateVisualArray);
        

        if(index == -1)
        {
            TextArray.text = ""; 
            return;
        }

        if(index >= inputList.Count)
        {
            TextArray.text = "";
            return;
        }
        int i = 0;
        TextArray.text = "Next:";
        while (index < inputList.Count && i < 4)
        {

            TextArray.text = TextArray.text + " " + inputList[index];
            i++;
            index++;
        }

        if (i==4) { TextArray.text = TextArray.text + " ..."; }
        
    }

    public void updateVisualArray_inverse(ActionData ad)
    {
        updateVisualArray(ad.valueList, ad.oldValue - 1);
    }

    public void clearTree()
    {
        if (root != null)
        {
            LineRenderer[] objects = GameObject.FindObjectsOfType<LineRenderer>();
            foreach (LineRenderer item in objects)
            {
                Destroy(item.gameObject);
            }
            root.transform.parent = root.transform.parent.parent;
            Destroy(root);

            updateVisualArray(deleteValues,-1);
            if (!stepBack)
            {
                deleteValues.Clear();
                actionStack.Clear();
                nodeValues.Clear();
            }
            delete = false;
            skipDelete = false;
            root = null;
        }
    }

    public void saveTree()
    {
        if(nodeValues.Count != 0 || deleteValues.Count != 0)
        {
            Debug.LogError("Can't save: Operation in Execution");
            return;
        }

        SaveLoadSystem.saveTree(this,false,"tree");
    
    }

    public void loadTree()
    {
        if (nodeValues.Count != 0 || deleteValues.Count != 0)
        {
            Debug.LogError("Can't load: Operation in Execution");
            return;
        }

        clearTree();

        SaveLoadSystem.loadTree(this,10,false, "tree");

        if (root != null) root.GetComponent<NodeScript>().updateTreeStructure();

    }

    private void updateItem(int i)
    {
        PushToActionStack(null, 0, 0, item, 0, 0, null, ActionData.Action.itemUpdate);
        item = i;
    }

    private void updateItem_inverse(ActionData ad)
    {
        item = ad.oldValue;
    }

    private void globalActionStateInsertChange(ActionStateInsert asi)
    {
        PushToActionStack(null, 0, 0, 0, globalActionStateInsert, 0, null, ActionData.Action.globalActionStateInsertChange);
        globalActionStateInsert = asi;

    }

    private void globalActionStateInsertChange_inverse(ActionData ad)
    {
        globalActionStateInsert = ad.curActionStateInsert;
    }

    private void globalActionStateDeleteChange(ActionStateDelete asd)
    {
        PushToActionStack(null, 0, 0, 0, 0, globalActionStateDelete, null, ActionData.Action.globalActionStateInsertChange);
        globalActionStateDelete = asd;

    }

    private void globalActionStateDeletetChange_inverse(ActionData ad)
    {
        globalActionStateDelete = ad.curActionStateDelete;
    }

    private void setParent(Transform newParent)
    {

        PushToActionStack(parent.gameObject, 0, 0, 0, 0, 0, null, ActionData.Action.setParent);

        parent = newParent;
        
    }


    private void setGParent(Transform newGParent)
    {

        PushToActionStack(gParent.gameObject, 0, 0, 0, 0, 0, null, ActionData.Action.setGParent);

        gParent = newGParent;

    }

    private void setParent_inverse(ActionData ad)
    {
        parent = ad.node.transform;
    }

    private void setGParent_inverse(ActionData ad)
    {
        gParent = ad.node.transform;
    }

    private void setDir(NodeScript.NodeDirection nDir)
    {
        PushToActionStack(null, 0, dir, 0, 0, 0, null, ActionData.Action.saveDir);
        dir = nDir;
    }


    private void setDir_inverse(ActionData ad)
    {
        dir = ad.dir;
    }

    private void chumpBlockOnStack()
    {
        PushToActionStack(null, 0, 0, 0, 0, 0, null, ActionData.Action.chumpBlock);
    }

    public void PushToActionStack(GameObject node, NodeScript.NodeColor color, NodeScript.NodeDirection dir, int oldValue ,ActionStateInsert asi,ActionStateDelete asd, List<int> l,ActionData.Action action)
    {
        //When the inverse actions are taking place we don't want to push anything on the actionStack
        if (stepBack) return;

        return;

        //ActionData a = new ActionData(node, color, dir, oldValue, asi, asd, l,action);
        //actionStack.Push(a);
    }


    public void setStepBack(bool n)
    {
        stepBack = n;
    }


    public void updateTurnTime()
    {
        TurnTime = flowSlider.value;
    }

    public void logMessage(string s)
    {
        textLog.logMessage(s, writeToLog);
        textLog.logMessage("     ", writeToLog);
    }

}
