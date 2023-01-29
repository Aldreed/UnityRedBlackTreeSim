using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeSaveData
{

    public float[] position;

    public int value;
    public int color;
    public int nChildren;

    public int[] childValidVector;

    public int indexNumber;


    public NodeSaveData(NodeScript ns)
    {
        position = new float[3];

        position[0] = ns.transform.position.x;
        position[1] = ns.transform.position.y;
        position[2] = ns.transform.position.z;

        value = ns.val;

        childValidVector = new int[2];

        childValidVector[0] = 0;
        childValidVector[1] = 0;

        if (ns.leftChild != null)
            childValidVector[0] = 1;
        if (ns.rightChild != null)
            childValidVector[1] = 1;

        if (ns.MyColor == NodeScript.NodeColor.Red)
            color = 0;
        else
            color = 1;

        nChildren = ns.ChildNodeCount;

        indexNumber = -1;

    }

}
