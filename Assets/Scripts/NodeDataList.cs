using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeDataList 
{

    public List<NodeSaveData> nodeDataList;

    public NodeDataList()
    {
        nodeDataList = new List<NodeSaveData>();
    }

    public void add(NodeSaveData nsd)
    {
        nodeDataList.Add(nsd);
    }

    public NodeSaveData read(int i)
    {
        if (i >= nodeDataList.Count || i < 0)
        {
            return null;
        }

        return nodeDataList[i];
    }

}
