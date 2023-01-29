using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadSystem
{
    private static NodeDataList nodeDataList;

    private enum ChildDirection
    {
        Right,
        Left
    };

    public static void saveTree(NodeTree n,bool snapshot,string filename)
    {
         nodeDataList = new NodeDataList();

        GameObject root = n.root;
        if(root == null)
        {
            saveNode(null);
        }
        else
        {
            saveNode(root);
            visitNode(root);
        }

        //save logic

        BinaryFormatter formatter = new BinaryFormatter();

        string dirName = "Saves";
        string fileName = filename;

        if (snapshot)
        {
            dirName = "Snapshots";
        }

        DirectoryInfo info = Directory.CreateDirectory(dirName);

        //string path = Application.persistentDataPath + "/tree.rb";
        string path = Path.Combine(info.FullName, fileName); 
        Debug.Log(path);
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, nodeDataList);
        stream.Close();
         
    }

    private static NodeSaveData saveNode(GameObject n)
    {

        if (n == null)
        {
            nodeDataList.add(null);
            return null;
        }

        NodeScript ns = n.GetComponent<NodeScript>();
        if (ns == null)
            Debug.LogError("Can't save node: No Node found");

        NodeSaveData nsd = new NodeSaveData(ns);
        nodeDataList.add(nsd);
        return nsd;

    }

    private static int visitNode(GameObject n)
    {
        if (n == null)
            return 0;

        NodeScript ns = n.GetComponent<NodeScript>();
        GameObject leftChild = ns.leftChild;
        GameObject rightChild = ns.rightChild;

        int leftChildren;
        int rightChildren;

        NodeSaveData leftChildData = saveNode(ns.leftChild);
        NodeSaveData rightChildData = saveNode(ns.rightChild);

        leftChildren = visitNode(leftChild) + 1;
        rightChildren = visitNode(rightChild) + 1;

        if (leftChildData !=null)
            leftChildData.indexNumber = leftChildren - 1;
        
        if(rightChildData != null)
            rightChildData.indexNumber = rightChildren - 1;

        return leftChildren + rightChildren;

    }


    public static bool loadTree(NodeTree tree, float scale,bool snapshot,string filename)
    {
        //load logic

        //string path = Application.persistentDataPath + "/tree.rb";
        string dirName = "Saves";
        string fileName = filename;

        if (snapshot)
        {
            dirName = "Snapshots";
        }

        DirectoryInfo info = Directory.CreateDirectory(dirName);
        string path = Path.Combine(info.FullName, fileName);


        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            nodeDataList = (NodeDataList)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            Debug.Log("File doesn't exist");
            return false;
        }
        tree.clearTree();
        NodeSaveData rootData = nodeDataList.read(0);

        if (rootData == null)
        {
            tree.root = null;
            return true;
        }

        Vector3 position;
        position.x = rootData.position[0];
        position.y = rootData.position[1];
        position.z = rootData.position[2];

        GameObject o = Object.Instantiate(tree.NodePrefab, position, Quaternion.identity, tree.transform);
        NodeScript ns = o.GetComponent<NodeScript>();

        o.transform.localScale = new Vector3(scale, scale, scale);

        tree.writeToLog = false;

        ns.treeRoot = tree.gameObject;
        ns.NodePrefab = tree.NodePrefab;
        ns.updateValue(rootData.value);
        ns.changeColor(o, rootData.color == 0 ? NodeScript.NodeColor.Red : NodeScript.NodeColor.Black);
        ns.ChildNodeCount = rootData.nChildren;
        o.name = "Node " + NodeScript.nCount++;

        tree.root = o;

        if (rootData.childValidVector[0] == 1)
            ns.leftChild = loadNode(ns,1,ChildDirection.Left);
        else
            ns.leftChild = null;

        if (rootData.childValidVector[1] == 1)
            ns.rightChild = loadNode(ns,2,ChildDirection.Right);
        else
            ns.rightChild = null;

        tree.writeToLog = true;

        return true;
    }


    private static GameObject loadNode(NodeScript parent, int index,ChildDirection cd)
    {
        NodeSaveData nsd = nodeDataList.read(index);

        Vector3 position;
        position.x = nsd.position[0];
        position.y = nsd.position[1];
        position.z = nsd.position[2];

        GameObject o = Object.Instantiate(parent.NodePrefab, position, Quaternion.identity, parent.transform);
        NodeScript ns = o.GetComponent<NodeScript>();

        ns.treeRoot = parent.treeRoot;
        ns.NodePrefab = parent.NodePrefab;
        ns.changeColor(o, nsd.color == 0 ? NodeScript.NodeColor.Red : NodeScript.NodeColor.Black);
        ns.updateValue(nsd.value);
        ns.ChildNodeCount = nsd.nChildren;
        o.name = "Node " + NodeScript.nCount++;

        if(cd == ChildDirection.Left)
        {
            if (nsd.childValidVector[0] == 1)
                ns.leftChild = loadNode(ns, index + 2,ChildDirection.Left);
            else
                ns.leftChild = null;

            if (nsd.childValidVector[1] == 1)
                ns.rightChild = loadNode(ns, index + 3,ChildDirection.Right);
            else
                ns.rightChild = null;
        
        }
        else
        {
            int ChildIndex = getSiblingChildrenCount(index);
            if (nsd.childValidVector[0] == 1)
                ns.leftChild = loadNode(ns, index + ChildIndex + 1, ChildDirection.Left);
            else
                ns.leftChild = null;

            if (nsd.childValidVector[1] == 1)
                ns.rightChild = loadNode(ns, index + ChildIndex + 2, ChildDirection.Right);
            else
                ns.rightChild = null;

        }

        return o;

    }

    private static int getSiblingChildrenCount(int index)
    {
        NodeSaveData sibling = nodeDataList.read(index - 1);
        if (sibling == null)
            return 0;
        else
            return sibling.indexNumber;

    }

}
