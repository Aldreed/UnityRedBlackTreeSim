using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionData
{


    public enum Action
    {
        changeCurNode,
        rootSet,
        changeColor,
        rotate,
        insert,
        endinsert,
        updateChildCountMinusOne,
        updateValue,
        updateVisualArray,
        enableSelect,
        disableSelect,
        itemUpdate,
        findNextForInsert,
        globalActionStateInsertChange,
        globalActionStateDeleteChange,
        chumpBlock,
        setParent,
        setGParent,
        firstAction,
        insertArray,
        saveDir,
        deleteArrayNI,
    }
    
    public GameObject node;
    public NodeScript.NodeColor color;
    public NodeScript.NodeDirection dir;
    public Action currentAction;
    public int oldValue;
    public NodeTree.ActionStateInsert curActionStateInsert;
    public NodeTree.ActionStateDelete curActionStateDelete;
    public List<int> valueList;

    public ActionData(GameObject n, NodeScript.NodeColor nc, NodeScript.NodeDirection nd,int i,NodeTree.ActionStateInsert asi, NodeTree.ActionStateDelete asd, List<int> list,Action a)
    {
        node = n;
        color = nc;
        dir = nd;
        currentAction = a;
        oldValue = i;

        curActionStateInsert = asi;
        curActionStateDelete = asd;

        valueList = list;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
