using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{


    public enum NodeColor
    {
        Red,
        Black
    }

    public enum NodeDirection
    {
        Left,
        Right
    }

    public NodeColor MyColor = NodeColor.Red;

    public GameObject NodePrefab;

    public GameObject leftChild;
    public GameObject rightChild;

    public int val = 0;

    public LineRenderer line;
    public LineRenderer lineToLeft = null;
    public LineRenderer lineToRight = null;

    public int ChildNodeCount;

    public float mag = 5f;

    public GameObject treeRoot;

    public static int nCount = 0;

    private GameObject SelectField;

    private void Awake()
    {
        SelectField = this.transform.Find("Select").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //here or in LateUpdate we Can update the start and end positions of lineRenederers everyFrame to follow the actual nodes if we deside to implement animations
    }

    //Preforms a search for a place to insert the node with the value i
    //If the node can insert the value it returns true and the out param is set to the inserted node
    //If the value can't be inserted retruns false and the out param points to the next node to search
    //If the value is already in the tree retruns false and the out param is null
    public bool findNextForInsert_Step(int i, out GameObject node) {

        Vector3 myPos = this.transform.position;
        Vector3 leftChildPos = myPos + new Vector3(-1, -1) * mag;
        Vector3 rightChildPos = myPos + new Vector3(1, -1) * mag;


        GameObject ret = null;

        pushToActionStack(this.gameObject, 0, 0, 0, ActionData.Action.findNextForInsert);

        //Prereq for insertFixup
        ChildNodeCount++;
        if (i > val)
        {
            if (rightChild == null)
            {
                ret = Instantiate(NodePrefab, rightChildPos, Quaternion.identity, this.transform);
                NodeScript ns = ret.GetComponent<NodeScript>();
                ns.NodePrefab = NodePrefab;
                ns.treeRoot = this.treeRoot;
                ns.updateValue(i);
                rightChild = ret;
                ret.name = "Node " + nCount++;


                updateLine(NodeDirection.Right, rightChild);
                node = rightChild;


                pushToActionStack(ret, 0, 0, 0,ActionData.Action.insert);

                logMessage("Insert as right child");

                return true;
                //insertFixup(treeRoot, rightChild.transform);

            }
            else
            {
                node = rightChild;
                logMessage("Moving to right child");
                return false;

                //updateTreeStructure
            }

        }
        else if (i < val)
        {

            if (leftChild == null)
            {
                ret = Instantiate(NodePrefab, leftChildPos, Quaternion.identity, this.transform);
                NodeScript ns = ret.GetComponent<NodeScript>();
                ns.NodePrefab = NodePrefab;
                ns.treeRoot = this.treeRoot;
                ns.updateValue(i);
                leftChild = ret;
                ret.name = "Node " + nCount++;

                updateLine(NodeDirection.Left, leftChild);
                node = leftChild;

                pushToActionStack(ret, 0, 0, 0,ActionData.Action.insert);

                logMessage("Insert as left child");

                return true;
                //insertFixup(treeRoot, leftChild.transform);

            }
            else
            {
                node = leftChild;
                logMessage("Moving to left child");
                return false;

                //updateTreeStructure
            }


        }
        else
        {
            logMessage("Value to be inserted: " + i + " is already in the Tree");
            updateChildCount(this, -1);
        }

        node = null;
        return false;
    }

    public void insert_inverse(ActionData ad)
    {
        if (isLeftChild(this.gameObject))
            this.transform.parent.GetComponent<NodeScript>().leftChild = null;
        else
            this.transform.parent.GetComponent<NodeScript>().rightChild = null;

        treeRoot.GetComponent<NodeTree>().root.GetComponent<NodeScript>().updateTreeStructure();

        Destroy(this.gameObject);

    }


    public void findNextForInsert_inverse()
    {
        ChildNodeCount--;
        this.updateTreeStructure();
    }

    public GameObject findNextForInsert(int i)
    {
        Vector3 myPos = this.transform.position;
        Vector3 leftChildPos = myPos + new Vector3(-1, -1) * mag;
        Vector3 rightChildPos = myPos + new Vector3(1, -1) * mag;


        GameObject ret = null;

        //Prereq for insertFixup
        ChildNodeCount++;
        if (i > val)
        {
            if (rightChild == null)
            {
                ret = Instantiate(NodePrefab, rightChildPos, Quaternion.identity, this.transform);
                NodeScript ns = ret.GetComponent<NodeScript>();
                ns.NodePrefab = NodePrefab;
                ns.updateValue(i);
                ns.treeRoot = this.treeRoot;
                rightChild = ret;
                ret.name = "Node " + nCount++;


                updateLine(NodeDirection.Right, rightChild);
                insertFixup(treeRoot, rightChild.transform);

            }
            else
            {
                rightChild.GetComponent<NodeScript>().findNextForInsert(i);

                //updateTreeStructure
            }

        }
        else if (i < val)
        {
            
            if (leftChild == null)
            {
                ret = Instantiate(NodePrefab, leftChildPos, Quaternion.identity, this.transform);
                NodeScript ns = ret.GetComponent<NodeScript>();
                ns.NodePrefab = NodePrefab;
                ns.updateValue(i);
                ns.treeRoot = this.treeRoot;
                leftChild = ret;
                ret.name = "Node " + nCount++;

                updateLine(NodeDirection.Left, leftChild);
                insertFixup(treeRoot, leftChild.transform);

            }
            else
            {
                leftChild.GetComponent<NodeScript>().findNextForInsert(i);

                //updateTreeStructure
            }


        }
        else
        {
            Debug.Log("Value to be inserted: " + i + " is already in the Tree");
            updateChildCount(this, -1);

        }



        return ret;
    }

    private void updateLine(NodeDirection dir, GameObject childNode)
    {
        if (dir == NodeDirection.Left)
        {
            if (lineToLeft != null)
            {
                lineToLeft.SetPosition(0, this.transform.position + new Vector3(0, 0, 1));
                lineToLeft.SetPosition(1, childNode.transform.position + new Vector3(0, 0, 1));
            }
            else
            {

                LineRenderer CreatedLine = Instantiate(line);
                CreatedLine.SetPosition(0, this.transform.position + new Vector3(0,0,1));
                CreatedLine.SetPosition(1, childNode.transform.position + new Vector3(0, 0, 1));
                


                lineToLeft = CreatedLine;
            }
        }
        else
        {
            if (lineToRight != null)
            {
                lineToRight.SetPosition(0, this.transform.position + new Vector3(0, 0, 1));
                lineToRight.SetPosition(1, childNode.transform.position + new Vector3(0, 0, 1));
            }
            else
            {

                LineRenderer CreatedLine = Instantiate(line);
                CreatedLine.SetPosition(0, this.transform.position + new Vector3(0, 0, 1));
                CreatedLine.SetPosition(1, childNode.transform.position + new Vector3(0, 0, 1));


                lineToRight = CreatedLine;
            }
        }

    }

    public float NodeBranchDistanceModif = 0.2f;

    public void changeColor(GameObject g,NodeColor n) {

        logMessage("Recoloring node: " + g.GetComponent<NodeScript>().val + " to " + ((n == NodeColor.Red) ? "Red" : "Black"));

        pushToActionStack(g, g.GetComponent<NodeScript>().MyColor, 0, 0,ActionData.Action.changeColor);

        g.GetComponent<NodeScript>().MyColor = n;
        g.GetComponent<SpriteRenderer>().color = n == NodeColor.Red ? Color.red : Color.black;


    }

    public void changeColor_inverse(ActionData ad)
    {
        changeColor(ad.node, ad.color);
    }

    public void updateTreeStructure()
    {
        
        if (leftChild != null)
        {
            //NodeBranchDistanceModif je staticka vrednost koja se koristi prilikom racunanja pozicije radi boljeg izgleda stabla
            leftChild.transform.position = new Vector3(leftChild.transform.parent.position.x + 1  + ChildNodeCount * -NodeBranchDistanceModif, leftChild.transform.position.y);
            updateLine(NodeDirection.Left, leftChild);
            leftChild.GetComponent<NodeScript>().updateTreeStructure();

        }
        else if (lineToLeft != null)
        {
            //Brisanje linije ako vise nije potrebna
            Destroy(lineToLeft);
        }

        if (rightChild != null)
        {
            rightChild.transform.position = new Vector3(rightChild.transform.parent.position.x - 1  + ChildNodeCount * NodeBranchDistanceModif, rightChild.transform.position.y);
            updateLine(NodeDirection.Right, rightChild);
            rightChild.GetComponent<NodeScript>().updateTreeStructure();
        }
        else if (lineToRight != null)
        {
            Destroy(lineToRight);
        }


    }


    public void switchLeft2RightChild()
    {

        Destroy(rightChild);
        rightChild = leftChild;
        leftChild.transform.position = this.transform.position + new Vector3(1, -1) * mag;
        leftChild.transform.position += new Vector3(ChildNodeCount * NodeBranchDistanceModif, 0);
        leftChild = null;
    }

    public void switchRight2LeftChild()
    {
        rightChild.transform.parent = leftChild.transform;
        rightChild.transform.position = new Vector3(1, -1) * mag + leftChild.transform.position;

        LineRenderer t = rightChild.transform.parent.parent.GetComponent<NodeScript>().lineToRight;
        Destroy(t.gameObject);
        rightChild.transform.parent.GetComponent<NodeScript>().lineToRight = null;

        leftChild.GetComponent<NodeScript>().ChildNodeCount += rightChild.GetComponent<NodeScript>().ChildNodeCount;
        leftChild.GetComponent<NodeScript>().rightChild = rightChild;

        rightChild = null;


    }


    public void updateValue(int n)
    {
        pushToActionStack(this.gameObject, 0, 0, val, ActionData.Action.updateValue);

        val = n;
        Transform child = this.transform.GetChild(0);
        child.GetComponent<TMPro.TextMeshPro>().text = n.ToString();
    }

    public void updateValue_inverse(ActionData ad)
    {
        updateValue(ad.oldValue);
    }

    //Needs to be called with the parent gParent and dir from the insertFixup_step Function
    //next - next node for witch insertFixup_step needs to be called
    public bool insertFixup_CaseX(GameObject tree, Transform z, Transform parent, Transform gParent,out Transform next,NodeDirection dir)
    {

            switch (dir)
            {
                case NodeDirection.Left:

                    rotateLeft(tree, gParent);
                    changeColor(parent.gameObject, NodeColor.Black);
                    changeColor(gParent.gameObject, NodeColor.Red);

                
                next = parent;
                return true;
                case NodeDirection.Right:
                    rotateRight(tree, gParent);


                    changeColor(parent.gameObject, NodeColor.Black);
                    changeColor(gParent.gameObject, NodeColor.Red);
   
                next = parent;
                return true;
            }

        next = null;
        return false;
    }


    //fixup step - returns false if it needs to be called again; true if we can move on 
    //z - current node 
    //next - next node to call this step
    //parentRet - if not null then insertFixup_CaseX needs to be called before the next call of this function with this as one of its args
    //gParentRet - same as above
    //dir - same as above 
    public bool insertFixup_step(GameObject tree, Transform z, out Transform next, out Transform parentRet, out Transform gParentRet,out NodeDirection dir)
    {

        


        Transform parent = null;
        Transform gParent = null;

        logMessage("Performing fixup");

        //Setting the out params in case we don't reach the case X 
        parentRet = null;
        gParentRet = null;
        dir = NodeDirection.Left;

        if (
            z.parent.GetComponent<NodeScript>() != null &&
            z.parent.parent.GetComponent<NodeScript>() != null &&
            z.parent.GetComponent<NodeScript>().MyColor == NodeColor.Red &&
            z.GetComponent<NodeScript>().MyColor != NodeColor.Black)
        {
            parent = z.parent;
            gParent = parent.parent;
            NodeScript y = null;
            if (gParent.GetComponent<NodeScript>().leftChild != null &&
                parent == gParent.GetComponent<NodeScript>().leftChild.transform)
            {
                GameObject uncleGO = gParent.GetComponent<NodeScript>().rightChild;
                if (uncleGO != null)
                {
                    y = uncleGO.GetComponent<NodeScript>();
                }

                if (y != null && y.MyColor == NodeColor.Red)
                {
                    changeColor(parent.gameObject, NodeColor.Black);
                    changeColor(y.gameObject, NodeColor.Black);
                    changeColor(gParent.gameObject, NodeColor.Red);

                    next = gParent;
                    return false;
                }
                else
                {
                    if (parent.GetComponent<NodeScript>().rightChild != null && z == parent.GetComponent<NodeScript>().rightChild.transform)
                    {

                        rotateLeft(tree, parent);
                        z = parent;
                        parent = z.parent;

                        parentRet = parent;
                        gParentRet = gParent;
                        next = z;

                        dir = NodeDirection.Right;
                        return false;


                    }
                    else
                    {

                        //rotateRight(tree, gParent);

                        
                        //changeColor(parent.gameObject, NodeColor.Black);
                        //changeColor(gParent.gameObject, NodeColor.Red);

                        insertFixup_CaseX(tree, z, parent, gParent,out next, NodeDirection.Right);


                    }
                    //z = parent;
                    next = parent;
                    return false;
                }
            }
            else
            {
                GameObject uncleGO = gParent.GetComponent<NodeScript>().leftChild;
                if (uncleGO != null)
                {
                    y = uncleGO.GetComponent<NodeScript>();
                }
                if (y != null && y.MyColor == NodeColor.Red)
                {
                    changeColor(parent.gameObject, NodeColor.Black);
                    changeColor(y.gameObject, NodeColor.Black);
                    changeColor(gParent.gameObject, NodeColor.Red);
                    //parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                    //y.MyColor = NodeColor.Black;
                    //gParent.GetComponent<NodeScript>().MyColor = NodeColor.Red;
                    //z = gParent;
                    next = gParent;
                    return false;
                }
                else
                {
                    if (parent.GetComponent<NodeScript>().leftChild != null && z == parent.GetComponent<NodeScript>().leftChild.transform)
                    {
                        //z = z.parent;
                        //right-rotate(t,z) ? right 
                        rotateRight(tree, parent);
                        z = parent;
                        parent = z.parent;

                        parentRet = parent;
                        gParentRet = gParent;
                        next = z;
                        dir = NodeDirection.Left;
                        return false;


                    }
                    else
                    {
                        //rotateLeft(tree, gParent);

                        //Maybe recoloring can be placed in front of the if
                        //changeColor(parent.gameObject, NodeColor.Black);
                        //changeColor(gParent.gameObject, NodeColor.Red);
                        //parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                        //gParent.GetComponent<NodeScript>().MyColor = NodeColor.Red;

                        insertFixup_CaseX(tree, z, parent, gParent, out next, NodeDirection.Left);

                    }

                    //z = parent;
                    next = parent;
                    return false;

                }
            }
        }
        else
        {
        //tree.transform.GetChild(0).GetComponent<NodeScript>().MyColor = NodeColor.Black;
        changeColor(tree.transform.GetChild(0).gameObject, NodeColor.Black);
        tree.GetComponent<NodeTree>().root = tree.transform.GetChild(0).gameObject;
            next = null;
            return true;

        }
    }

    public void insertFixup(GameObject tree, Transform z) {

        Transform parent = null;
        Transform gParent = null;

        while (
            z.parent.GetComponent<NodeScript>() != null &&
            z.parent.parent.GetComponent<NodeScript>() != null &&
            z.parent.GetComponent<NodeScript>().MyColor == NodeColor.Red &&
            z.GetComponent<NodeScript>().MyColor != NodeColor.Black) {
            parent = z.parent;
            gParent = parent.parent;
            NodeScript y = null;
            if (gParent.GetComponent<NodeScript>().leftChild != null &&
                parent == gParent.GetComponent<NodeScript>().leftChild.transform)
            {
                GameObject uncleGO = gParent.GetComponent<NodeScript>().rightChild;
                if (uncleGO != null)
                {
                    y = uncleGO.GetComponent<NodeScript>();
                }

                if (y != null && y.MyColor == NodeColor.Red)
                {
                    changeColor(parent.gameObject, NodeColor.Black);
                    changeColor(y.gameObject, NodeColor.Black);
                    changeColor(gParent.gameObject, NodeColor.Red);
                    //parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                    //y.MyColor = NodeColor.Black;
                    //gParent.GetComponent<NodeScript>().MyColor = NodeColor.Red;
                    z = gParent;
                }
                else
                {
                    if (parent.GetComponent<NodeScript>().rightChild != null && z == parent.GetComponent<NodeScript>().rightChild.transform)
                    {
                        //Ovo treba na kraju da se radi ali mi ovako lakse
                        //z.parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                        //z.parent.parent.GetComponent<NodeScript>().MyColor = NodeColor.Red;

                        rotateLeft(tree, parent);
                        z = parent;
                        parent = z.parent;

                        //isto ko else ali posto se desila rotatcija zbog referenci moramo ovako da implementiramo 
                        //mozda da uvedem pokzivace
                        rotateRight(tree, gParent);

                        //Mozda moze i pre svega da se uradi
                        changeColor(parent.gameObject, NodeColor.Black);
                        changeColor(gParent.gameObject, NodeColor.Red);
                        //parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                        //gParent.GetComponent<NodeScript>().rightChild.GetComponent<NodeScript>().MyColor = NodeColor.Red;

                    }
                    else
                    {

                        rotateRight(tree, gParent);

                        //Maybe recoloring can be placed in front of the if
                        changeColor(parent.gameObject, NodeColor.Black);
                        changeColor(gParent.gameObject, NodeColor.Red);
                        //parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                        //gParent.GetComponent<NodeScript>().MyColor = NodeColor.Red;


                    }
                    z = parent;
                }
            }
            else {
                GameObject uncleGO = gParent.GetComponent<NodeScript>().leftChild;
                if (uncleGO != null)
                {
                    y = uncleGO.GetComponent<NodeScript>();
                }
                if (y != null && y.MyColor == NodeColor.Red)
                {
                    changeColor(parent.gameObject, NodeColor.Black);
                    changeColor(y.gameObject, NodeColor.Black);
                    changeColor(gParent.gameObject, NodeColor.Red);
                    //parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                    //y.MyColor = NodeColor.Black;
                    //gParent.GetComponent<NodeScript>().MyColor = NodeColor.Red;
                    z = gParent;
                }
                else
                {
                    if (parent.GetComponent<NodeScript>().leftChild != null && z == parent.GetComponent<NodeScript>().leftChild.transform)
                    {
                        //z = z.parent;
                        //right-rotate(t,z) ? right 
                        rotateRight(tree, parent);
                        z = parent;
                        parent = z.parent;

                        rotateLeft(tree, gParent);
                        changeColor(parent.gameObject, NodeColor.Black);
                        changeColor(gParent.gameObject, NodeColor.Red);
                        //parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                        //gParent.GetComponent<NodeScript>().MyColor = NodeColor.Red;
                    }
                    else
                    {
                        rotateLeft(tree, gParent);

                        //Maybe recoloring can be placed in front of the if
                        changeColor(parent.gameObject, NodeColor.Black);
                        changeColor(gParent.gameObject, NodeColor.Red);
                        //parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                        //gParent.GetComponent<NodeScript>().MyColor = NodeColor.Red;
                    }

                    z = parent;

                }
            }
        }


        //tree.transform.GetChild(0).GetComponent<NodeScript>().MyColor = NodeColor.Black;
        changeColor(tree.transform.GetChild(0).gameObject, NodeColor.Black);
        tree.GetComponent<NodeTree>().root = tree.transform.GetChild(0).gameObject;


    }



    private void rotateLeft(GameObject tree, Transform node) {

        logMessage("Rotating left node: " + node.gameObject.GetComponent<NodeScript>().val);

        pushToActionStack(node.gameObject, 0, NodeDirection.Left,0,ActionData.Action.rotate);

        //TODO see if wee need to add logic if ptR is null
        Transform ptR = node.GetComponent<NodeScript>().rightChild.transform;
        NodeScript ptRnodeScript = ptR.GetComponent<NodeScript>();

        NodeScript nodeNodeScript = node.GetComponent<NodeScript>();

        nodeNodeScript.rightChild = ptRnodeScript.leftChild;


        if (nodeNodeScript.rightChild) {
            nodeNodeScript.rightChild.transform.parent = node;
            nodeNodeScript.rightChild.transform.position = new Vector3(1, -1) * mag + ptR.parent.transform.position;
        }

        ptR.parent = node.parent;

        if (!node.parent.GetComponent<NodeScript>()) {
            ptR.position = node.parent.position + new Vector3(0, 1, 0);
            //root = ptR;
            tree.GetComponent<NodeTree>().root = ptR.gameObject; // untested todo
        }
        else if (node.parent.GetComponent<NodeScript>().leftChild != null && node == node.parent.GetComponent<NodeScript>().leftChild.transform)
        {
            node.parent.GetComponent<NodeScript>().leftChild = ptR.gameObject;
            if (ptR.gameObject)
            {
                ptR.position = new Vector3(-1, -1) * mag + node.parent.transform.position; //TODO 
            }
        }
        else
        {
            node.parent.GetComponent<NodeScript>().rightChild = ptR.gameObject;
            if (ptR.gameObject)
            {
                ptR.position = new Vector3(1, -1) * mag + node.parent.transform.position; //TODO
            }
        }

        nodeNodeScript.ChildNodeCount -= ptRnodeScript.ChildNodeCount + 1;
        int rightChildrenCount;
        if (nodeNodeScript.rightChild) {
            rightChildrenCount = nodeNodeScript.rightChild.GetComponent<NodeScript>().ChildNodeCount + 1;
        }
        else
        {
            rightChildrenCount = 0;
        }

        nodeNodeScript.ChildNodeCount += rightChildrenCount;
        ptRnodeScript.ChildNodeCount -= rightChildrenCount;
        ptRnodeScript.ChildNodeCount += nodeNodeScript.ChildNodeCount + 1;


        ptRnodeScript.leftChild = node.gameObject;
        node.parent = ptR;
        if (node.gameObject)
        {
            node.position = new Vector3(-1, -1) * mag + node.parent.transform.position; //TODO
        }

        NodeScript parentScript = ptR.transform.parent.GetComponent<NodeScript>();
        if (parentScript)
        {
            parentScript.updateTreeStructure();
        }
        else
        {
            ptRnodeScript.updateTreeStructure();
        }

    }

    private void rotateRight(GameObject tree, Transform node) {

        logMessage("Rotating right node: " + node.gameObject.GetComponent<NodeScript>().val);

        pushToActionStack(node.gameObject, 0, NodeDirection.Right, 0,ActionData.Action.rotate);

        //TODO see if wee need to add logic if ptL is null
        Transform ptL = node.GetComponent<NodeScript>().leftChild.transform;
        NodeScript ptLnodeScript = ptL.GetComponent<NodeScript>();

        NodeScript nodeNodeScript = node.GetComponent<NodeScript>();

        nodeNodeScript.leftChild = ptLnodeScript.rightChild;

        if (nodeNodeScript.leftChild) {
            nodeNodeScript.leftChild.transform.parent = node;
            nodeNodeScript.leftChild.transform.position = new Vector3(-1, -1) * mag + ptL.parent.transform.position;
        }

        ptL.parent = node.parent;

        if (!node.parent.GetComponent<NodeScript>()) {
            ptL.transform.position = node.parent.position + new Vector3(0, 1, 0);
            //root = ptL;
            tree.GetComponent<NodeTree>().root = ptL.gameObject;//untested todo
        }
        else if (node.parent.GetComponent<NodeScript>().leftChild != null && node == node.parent.GetComponent<NodeScript>().leftChild.transform)
        {
            node.parent.GetComponent<NodeScript>().leftChild = ptL.gameObject;
            if (ptL.gameObject)
            {
                ptL.position = new Vector3(-1, -1) * mag + node.parent.transform.position; //TODO 
            }
        }
        else
        {
            node.parent.GetComponent<NodeScript>().rightChild = ptL.gameObject;
            if (ptL.gameObject)
            {
                ptL.position = new Vector3(1, -1) * mag + node.parent.transform.position; //TODO 
            }
        }

        nodeNodeScript.ChildNodeCount -= ptLnodeScript.ChildNodeCount + 1;
        int leftChildrenCount;
        if (nodeNodeScript.leftChild)
        {
            leftChildrenCount = nodeNodeScript.leftChild.GetComponent<NodeScript>().ChildNodeCount + 1;
        }
        else
        {
            leftChildrenCount = 0;
        }

        nodeNodeScript.ChildNodeCount += leftChildrenCount;
        ptLnodeScript.ChildNodeCount -= leftChildrenCount;
        ptLnodeScript.ChildNodeCount += nodeNodeScript.ChildNodeCount + 1;


        ptLnodeScript.rightChild = node.gameObject;
        node.parent = ptL;
        if (node.gameObject)
        {
            node.position = new Vector3(1, -1) * mag + node.parent.transform.position; //TODO
        }

        NodeScript parentScript = ptL.transform.parent.GetComponent<NodeScript>();
        if (parentScript)
        {
            parentScript.updateTreeStructure();
        }
        else
        {
            ptLnodeScript.updateTreeStructure();
        }
    }


    public void rotateLeft_inverse(GameObject tree)
    {
        rotateRight(tree, this.transform.parent);
    }

    public void rotateRight_inverse(GameObject tree)
    {
        rotateLeft(tree, this.transform.parent);
    }


    private GameObject findSibling(GameObject root) {
        NodeScript t = root.transform.parent.GetComponent<NodeScript>();
        if (t == null) {
            return null;
        }
        else if (t.leftChild == root)
        {
            return t.rightChild;
        }
        else
        {
            return t.leftChild;
        }
    }

    private bool isLeftChild(GameObject root) {
        NodeScript parent = root.transform.parent.GetComponent<NodeScript>();
        if (parent == null)
        {
            return false;
        }
        else if (parent.leftChild == root) {
            return true;
        }
        else
        {
            return false;
        }
    }

    public GameObject findInorderSuc(GameObject root) {

        if (root == null || root.GetComponent<NodeScript>() == null) {
            Debug.LogError("Invalid inorder root " + root);
            return null;
        }
        GameObject prev = null;
        while (root != null) {
            prev = root;
            root = root.GetComponent<NodeScript>().leftChild;
        }

        return prev != null ? prev : root;
    }

    //Delete step
    //if returned true need to check if next == root if so then procede to the next step of delete action if not then go to next elem in the deleteArray
    //if retrurend false then next points to the next node for witch to call delete_step
    public bool delete_step(GameObject root,int data, GameObject tree, out GameObject next, out bool newItem)
    {
        next = null;
        newItem = false;
        if (root == null)
        {
            Debug.Log("Null root: Invalid Node or reached Bottom");
            return true;
        }
        NodeScript rootScript = root.GetComponent<NodeScript>();
        if (rootScript == null)
        {
            Debug.Log("Null node: Invalid Node or reached Bottom");
            return true;
        }

        if (rootScript.val == data)
        {
            logMessage("Found node");
            if (rootScript.leftChild == null || rootScript.rightChild == null)
            {
                //deleteOne(root, tree);
                next = root;
                return true;
            }
            else
            {
                logMessage("Finding inorder successor");
                GameObject nextInOrder = findInorderSuc(rootScript.rightChild);
                rootScript.updateValue(nextInOrder.GetComponent<NodeScript>().val);
                //delete(nextInOrder, rootScript.val, tree);
                newItem = true;
                next = nextInOrder;
                logMessage("New node to delete: " + nextInOrder.GetComponent<NodeScript>().val);
                return false;
            }
        }
        else if (rootScript.val < data)
        {
            //delete(rootScript.rightChild, data, tree);
            logMessage("Moving to right child"); 
            next = rootScript.rightChild;
            return false;
        }
        else
        {
            //delete(rootScript.leftChild, data, tree);
            next = rootScript.leftChild;
            logMessage("Moving to left child");
            return false;
        }

    }
    public void delete(GameObject root, int data, GameObject tree)
    {
        if (root == null) {
            Debug.Log("Null root: Invalid Node or reached Bottom");
            return;
        }
        NodeScript rootScript = root.GetComponent<NodeScript>();
        if (rootScript == null)
        {
            Debug.Log("Null rootScript: Invalid Node or reached Bottom");
            return;
        }

        if (rootScript.val == data)
        {
            if (rootScript.leftChild == null || rootScript.rightChild == null)
            {
                deleteOne(root, tree);
            }
            else
            {
                GameObject next = findInorderSuc(rootScript.rightChild);
                rootScript.updateValue(next.GetComponent<NodeScript>().val);
                delete(next, rootScript.val, tree);
            }
        }
        else if (rootScript.val < data)
        {
            delete(rootScript.rightChild, data, tree);
        }
        else
        {
            delete(rootScript.leftChild, data, tree);
        }
    }

    private void replaceNode(GameObject node, NodeScript child, GameObject tree)
    {
        logMessage("Replacing node with child");
        NodeScript nodeSc = node.GetComponent<NodeScript>();
        //not null child
        if (node == null || tree == null) {
            Debug.LogError("Replace node error");
            return;
        }

        if(child != null)
        {
            child.transform.parent = node.transform.parent;
        }

        if (node.transform.parent.GetComponent<NodeScript>() == null) { 
            if(child != null)
            {
                child.transform.position = tree.transform.position + new Vector3(0, 1, 0);
                tree.GetComponent<NodeTree>().root = child.gameObject;
                //need to set root todo
            }
            else
            {
                tree.GetComponent<NodeTree>().root = null;
            }
        }
        else
        {
            //PMANI CHILD I PROVERI DIR CHULD
            //Setting the null pointer in the parents right/leftChild field will be done in the delete function at the end
            if (isLeftChild(node))
            {
                if (child != null)
                {
                    node.transform.parent.GetComponent<NodeScript>().leftChild = child.gameObject;
                    child.transform.position = new Vector3(-1, -1) * mag + child.transform.parent.position;
                }

            }
            else
            {
                if (child != null)
                {
                    node.transform.parent.GetComponent<NodeScript>().rightChild = child.gameObject;
                    child.transform.position = new Vector3(1, -1) * mag + child.transform.parent.position;
                }
            }

            NodeScript parentScript = node.transform.parent.GetComponent<NodeScript>(); 
            if (parentScript)
            {
                parentScript.updateTreeStructure();
            }

        }
    }


    private bool check_color_eq(GameObject node, NodeColor color)
    {
        if (node == null && color == NodeColor.Red)
        {
            return false;
        }
        else if (node == null && color == NodeColor.Black)
        {
            return true;
        }
        else if (node.GetComponent<NodeScript>().MyColor == color)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void updateChildCount(NodeScript node, int val)
    {

        pushToActionStack(node.gameObject, 0, 0, val,ActionData.Action.updateChildCountMinusOne);
        while(node!= null)
        {
            node.ChildNodeCount += val;
            node = node.transform.parent.GetComponent<NodeScript>();
        }
    }

    public void updateChildCount_inverse(ActionData ad)
    {
        updateChildCount(this, -ad.oldValue);
        treeRoot.GetComponent<NodeTree>().root.GetComponent<NodeScript>().updateTreeStructure();
    }

    //Deleting One node step
    // returns true if the node was deleted
    // returns false if we need to check delete cases
    // childNext - NodeScript for witch to call cases
    // tempChild - indicator if we used a tempChild Node
    public bool deleteOne_step(GameObject delNode,GameObject tree, out NodeScript childNext,out bool tempChild)
    {
        tempChild = false;
        NodeScript delNodeScript = delNode.gameObject.GetComponent<NodeScript>();
        NodeScript child = null;

        bool childTemp = false;
        childNext = null;
        
        if (delNodeScript.rightChild != null)
        {
            child = delNodeScript.rightChild.GetComponent<NodeScript>();
        }
        else if (delNodeScript.leftChild != null)
        {
            child = delNodeScript.leftChild.GetComponent<NodeScript>();
        }
        else
        {
            //create temp child to represent null node
            childTemp = true;

            tempChild = true;
        }
        //needs to be called after the above ifs 
        //since it relies on the accuracy of the child refrance to set the positions / update the root node
        //nothing to replace if the child is null since we will be using a temp node
        if (!childTemp) replaceNode(delNode, child, tree);

        if (delNodeScript.MyColor == NodeColor.Black)
        {
            if (childTemp)
            {
                logMessage("Adding temp node X");
                child = delNodeScript;
                delNodeScript.val = 0;
                child.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "X";
                changeColor(delNode, NodeColor.Black);
                //delNodeScript.MyColor = NodeColor.Black;
            }
            else
            {
                cleanupDelete(delNode, delNodeScript, childTemp);
            }

            if (check_color_eq(child.gameObject, NodeColor.Red))
            {
                //child.MyColor = NodeColor.Black;
                changeColor(child.gameObject, NodeColor.Black);
                return true;
            }
            else
            {
                //deleteCase1(child, tree);
                childNext = child;
                return false;
            }
        }
        else
        {
            cleanupDelete(delNode, delNodeScript, childTemp);
            return true;
        }

        


        //TODO Delete This updateTreeStructure when adding Steps


        //delete node here special case is root node todo

    }


    public void cleanupDelete(GameObject delNode, NodeScript delNodeScript,bool childTemp)
    {
        if (childTemp)
            logMessage("Deleting temp node X");
        else
            logMessage("Deleting node: " + delNodeScript.val);

        //If a temp node was used do cleanup
        NodeScript parent = delNode.transform.parent.GetComponent<NodeScript>();
        if (parent && childTemp)
        {
            if (isLeftChild(delNode))
            {
                parent.leftChild = null;
            }
            else
            {
                parent.rightChild = null;
            }
            //If the node to be deleted is the rootNode of the tree the referance will already be set to null in replace node
        }


        if (delNodeScript.lineToLeft != null)
            Destroy(delNodeScript.lineToLeft.gameObject);
        if (delNodeScript.lineToRight != null)
            Destroy(delNodeScript.lineToRight.gameObject);

 
        Destroy(delNode);
        if (parent)
        {
            //Need to update childCount from the bottom up if I want to implement skipping for errors/not found
            updateChildCount(parent, -1);
            parent.updateTreeStructure();
        }
    }

    public void cleanupDelete_inverse(ActionData ad)
    {

        Vector3 myPos = ad.node.transform.position;
        Vector3 leftChildPos = myPos + new Vector3(-1, -1) * mag;
        Vector3 rightChildPos = myPos + new Vector3(1, -1) * mag;



        GameObject ret = Instantiate(NodePrefab, rightChildPos, Quaternion.identity, this.transform);
        NodeScript ns = ret.GetComponent<NodeScript>();
        ns.NodePrefab = NodePrefab;
        ns.updateValue(ad.oldValue);
        ns.treeRoot = this.treeRoot;
        if (ad.dir == NodeDirection.Right)
            rightChild = ret;
        else
            leftChild = ret;

        ret.name = "Node " + nCount++;
        if(ad.curActionStateDelete == NodeTree.ActionStateDelete.DeleteChildTemp) this.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "X";

        updateLine(ad.dir == NodeDirection.Right ? NodeDirection.Right : NodeDirection.Left, ret);
        updateTreeStructure();
    }


    private void deleteOne(GameObject delNode, GameObject tree)
    {
        NodeScript delNodeScript = delNode.gameObject.GetComponent<NodeScript>();
        NodeScript child = null;
        bool childTemp = false;

        if(delNodeScript.rightChild != null)
        {
            child = delNodeScript.rightChild.GetComponent<NodeScript>();
        }
        else if(delNodeScript.leftChild != null)
        {
            child = delNodeScript.leftChild.GetComponent<NodeScript>();
        }
        else
        {
            //create temp child to represent null node
            childTemp = true;
        }
        //needs to be called after the above ifs 
        //since it relies on the accuracy of the child refrance to set the positions / update the root node
        //nothing to replace if the child is null since we will be using a temp node
        if(!childTemp) replaceNode(delNode, child, tree);

        if(delNodeScript.MyColor == NodeColor.Black)
        {
            if (childTemp)
            {
                child = delNodeScript;
                delNodeScript.val = 0;
                child.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "X";
                changeColor(delNode, NodeColor.Black);
                //delNodeScript.MyColor = NodeColor.Black;
            }

            if (check_color_eq(child.gameObject,NodeColor.Red))
            {
                //child.MyColor = NodeColor.Black;
                changeColor(child.gameObject, NodeColor.Black);
            }
            else
            {
                deleteCase1(child, tree);
            }
        }

        //If a temp node was used do cleanup
        NodeScript parent = delNode.transform.parent.GetComponent<NodeScript>();
        if (parent && childTemp)
        {
            if (isLeftChild(delNode))
            {
                parent.leftChild = null;
            }
            else
            {
                parent.rightChild = null;
            }
            //If the node to be deleted is the rootNode of the tree the referance will already be set to null in replace node
        }


        if(delNodeScript.lineToLeft != null)
            Destroy(delNodeScript.lineToLeft.gameObject);
        if (delNodeScript.lineToRight != null)
            Destroy(delNodeScript.lineToRight.gameObject);

        NodeScript parentScript = delNode.transform.parent.GetComponent<NodeScript>();
        Destroy(delNode);
        if (parentScript)
        {
            //Need to update childCount from the bottom up if I want to implement skipping for errors/not found
            updateChildCount(parentScript, -1);
            parentScript.updateTreeStructure();
        }


        //TODO Delete This updateTreeStructure when adding Steps
        

        //delete node here special case is root node todo

    }


    public bool deleteCase1_step(NodeScript child,GameObject tree)
    {
        if (child.transform.parent.GetComponent<NodeScript>() == null)
        {
            logMessage("Case 1 detected");
            tree.GetComponent<NodeTree>().root = child.gameObject;
            return true;
        }
        return false;

    }

    public bool deleteCase2_step(NodeScript child, GameObject tree)
    {
        GameObject sibling = findSibling(child.gameObject);
        Transform parent = sibling.transform.parent;

        if (check_color_eq(sibling, NodeColor.Red))//Can sibling be null ? no since the count of black nodes is not in line with RED BLACK tree def
        {
            logMessage("Case 2 detected");
            if (isLeftChild(sibling))
            {
                rotateRight(tree, parent);

            }
            else
            {
                rotateLeft(tree, parent);
            }
            //recolor
            //parent.GetComponent<NodeScript>().MyColor = NodeColor.Red;
            //sibling.GetComponent<NodeScript>().MyColor = NodeColor.Black;
            changeColor(parent.gameObject, NodeColor.Red);
            changeColor(sibling, NodeColor.Black);

            //check tree root.. done in rotations
            return true;
        }

        return false;
    }


    public bool deleteCase3_step(NodeScript child, GameObject tree, out GameObject next)
    {
        GameObject sibling = findSibling(child.gameObject);
        NodeScript siblingScript = sibling.GetComponent<NodeScript>();

        if (check_color_eq(child.transform.parent.gameObject, NodeColor.Black) &&
            check_color_eq(sibling, NodeColor.Black) &&
            check_color_eq(siblingScript.leftChild, NodeColor.Black) &&
            check_color_eq(siblingScript.rightChild, NodeColor.Black))
        {
            logMessage("Case 3 detected");
            //siblingScript.MyColor = NodeColor.Red;
            changeColor(sibling.gameObject, NodeColor.Red);
            //deleteCase1(child.transform.parent.GetComponent<NodeScript>(), tree);
            next = child.transform.parent.gameObject;
            return true;
        }
        else
        {
            //deleteCase4(child, tree);
            next = null;
            return false;
        }
    }

    public bool deleteCase4_step(NodeScript child, GameObject tree)
    {
        GameObject sibling = findSibling(child.gameObject);
        NodeScript siblingScript = sibling.GetComponent<NodeScript>();

        if (check_color_eq(child.transform.parent.gameObject, NodeColor.Red) &&
            check_color_eq(sibling, NodeColor.Black) &&
            check_color_eq(siblingScript.leftChild, NodeColor.Black) &&
            check_color_eq(siblingScript.rightChild, NodeColor.Black))
        {
            logMessage("Case 4 detected");
            //siblingScript.MyColor = NodeColor.Red;
            //child.transform.parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
            changeColor(sibling, NodeColor.Red);
            changeColor(child.transform.parent.gameObject, NodeColor.Black);
            return true;

        }
        else
        {

            //deleteCase5(child, tree);
            return false;
        }
    }

    public bool deleteCase5_step(NodeScript child, GameObject tree)
    {
        GameObject sibling = findSibling(child.gameObject);
        NodeScript siblingScript = sibling.GetComponent<NodeScript>();

        if (check_color_eq(sibling, NodeColor.Black))
        {
            logMessage("Case 5 detected");
            GameObject nephew = null;
            bool recolor = false;
            if (isLeftChild(child.gameObject) &&
                check_color_eq(siblingScript.rightChild, NodeColor.Black) &&
                check_color_eq(siblingScript.leftChild, NodeColor.Red))
            {
                rotateRight(tree, sibling.transform);
                nephew = siblingScript.leftChild;
                recolor = true;
            }
            else if (!isLeftChild(child.gameObject) &&
                check_color_eq(siblingScript.leftChild, NodeColor.Black) &&
                check_color_eq(siblingScript.rightChild, NodeColor.Red))
            {
                rotateLeft(tree, sibling.transform);
                nephew = siblingScript.rightChild;
                recolor = true;
            }

            if (recolor)
            {
                //siblingScript.MyColor = NodeColor.Red;
                changeColor(sibling, NodeColor.Red);
                if (nephew != null)
                {
                    //nephew.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                    changeColor(nephew, NodeColor.Black);
                }
                return true;
            }

        }
        //deleteCase6(child, tree);
        return false;
    }

    public bool deleteCase6_step(NodeScript child, GameObject tree)
    {
        logMessage("Case 6 detected");

        GameObject sibling = findSibling(child.gameObject);
        NodeScript siblingScript = sibling.GetComponent<NodeScript>();

        GameObject parent = sibling.transform.parent.gameObject;
        NodeScript parentScript = parent.GetComponent<NodeScript>();

        //siblingScript.MyColor = parentScript.MyColor;
        //parentScript.MyColor = NodeColor.Black;
        changeColor(sibling.gameObject, parentScript.MyColor);
        changeColor(parent, NodeColor.Black);

        if (isLeftChild(child.gameObject))
        {
            //siblingScript.rightChild.gameObject.GetComponent<NodeScript>().MyColor = NodeColor.Black;
            changeColor(siblingScript.rightChild, NodeColor.Black);
            rotateLeft(tree, parent.transform);

        }
        else
        {
            //siblingScript.leftChild.gameObject.GetComponent<NodeScript>().MyColor = NodeColor.Black;
            changeColor(siblingScript.leftChild, NodeColor.Black);
            rotateRight(tree, parent.transform);
        }

        return true;
    }

    private void deleteCase1(NodeScript child,GameObject tree)
    {

        if(child.transform.parent.GetComponent<NodeScript>() == null)
        {
            tree.GetComponent<NodeTree>().root = child.gameObject;
            return;
        }
        deleteCase2(child, tree);
    }

    private void deleteCase2(NodeScript child, GameObject tree)
    {
        GameObject sibling = findSibling(child.gameObject);
        Transform parent = sibling.transform.parent;

        if(check_color_eq(sibling,NodeColor.Red))//Can sibling be null ? no since the count of black nodes is not in line with RED BLACK tree def
        {
            if (isLeftChild(sibling))
            {
                rotateRight(tree, parent);
                
            }
            else
            {
                rotateLeft(tree, parent);
            }
            //recolor
            //parent.GetComponent<NodeScript>().MyColor = NodeColor.Red;
            //sibling.GetComponent<NodeScript>().MyColor = NodeColor.Black;
            changeColor(parent.gameObject, NodeColor.Red);
            changeColor(sibling, NodeColor.Black);

            //check tree root.. done in rotations
        }

        deleteCase3(child, tree);

    }

    private void deleteCase3(NodeScript child, GameObject tree)
    {
        GameObject sibling = findSibling(child.gameObject);
        NodeScript siblingScript = sibling.GetComponent<NodeScript>();//sibling can be null

        if(check_color_eq(child.transform.parent.gameObject,NodeColor.Black) &&
            check_color_eq(sibling,NodeColor.Black) &&
            check_color_eq(siblingScript.leftChild,NodeColor.Black)&&
            check_color_eq(siblingScript.rightChild,NodeColor.Black)){
            //siblingScript.MyColor = NodeColor.Red;
            changeColor(sibling.gameObject, NodeColor.Red);
            deleteCase1(child.transform.parent.GetComponent<NodeScript>(), tree);
        }
        else
        {
            deleteCase4(child, tree);
        }
    }
    private void deleteCase4(NodeScript child, GameObject tree)
    {
        GameObject sibling = findSibling(child.gameObject);
        NodeScript siblingScript = sibling.GetComponent<NodeScript>();

        if (check_color_eq(child.transform.parent.gameObject,NodeColor.Red) &&
            check_color_eq(sibling,NodeColor.Black) &&
            check_color_eq(siblingScript.leftChild,NodeColor.Black) &&
            check_color_eq(siblingScript.rightChild,NodeColor.Black)){
            //siblingScript.MyColor = NodeColor.Red;
            //child.transform.parent.GetComponent<NodeScript>().MyColor = NodeColor.Black;
            changeColor(sibling, NodeColor.Red);
            changeColor(child.transform.parent.gameObject, NodeColor.Black);
        
        } else {

            deleteCase5(child, tree);
        }
    }

    private void deleteCase5(NodeScript child, GameObject tree)
    {
        GameObject sibling = findSibling(child.gameObject);
        NodeScript siblingScript = sibling.GetComponent<NodeScript>();

        if (check_color_eq(sibling, NodeColor.Black))
        {
            GameObject nephew = null;
            bool recolor = false;
            if(isLeftChild(child.gameObject) && 
                check_color_eq(siblingScript.rightChild,NodeColor.Black) &&
                check_color_eq(siblingScript.leftChild, NodeColor.Red))
            {
                rotateRight(tree, sibling.transform);
                nephew = siblingScript.leftChild;
                recolor = true;
            }
            else if(!isLeftChild(child.gameObject)&&
                check_color_eq(siblingScript.leftChild,NodeColor.Black) &&
                check_color_eq(siblingScript.rightChild, NodeColor.Red)){
                rotateLeft(tree, sibling.transform);
                nephew = siblingScript.rightChild;
                recolor = true;
            }

            if (recolor)
            {
                //siblingScript.MyColor = NodeColor.Red;
                changeColor(sibling, NodeColor.Red);
                if (nephew != null)
                {
                    //nephew.GetComponent<NodeScript>().MyColor = NodeColor.Black;
                    changeColor(nephew, NodeColor.Black);
                }
            }
            
        }
        deleteCase6(child, tree);

    }

    private void deleteCase6(NodeScript child, GameObject tree)
    {
        GameObject sibling = findSibling(child.gameObject);
        NodeScript siblingScript = sibling.GetComponent<NodeScript>();

        GameObject parent = sibling.transform.parent.gameObject;
        NodeScript parentScript = parent.GetComponent<NodeScript>();

        //siblingScript.MyColor = parentScript.MyColor;
        //parentScript.MyColor = NodeColor.Black;
        changeColor(sibling.gameObject, parentScript.MyColor);
        changeColor(parent, NodeColor.Black);

        if (isLeftChild(child.gameObject))
        {
            //siblingScript.rightChild.gameObject.GetComponent<NodeScript>().MyColor = NodeColor.Black;
            changeColor(siblingScript.rightChild, NodeColor.Black);
            rotateLeft(tree, parent.transform);
                       
        }
        else
        {
            //siblingScript.leftChild.gameObject.GetComponent<NodeScript>().MyColor = NodeColor.Black;
            changeColor(siblingScript.leftChild, NodeColor.Black);
            rotateRight(tree, parent.transform);
        }

        
    }

    public void enableSelect()
    {
        pushToActionStack(this.gameObject, 0, 0, 0, ActionData.Action.enableSelect);

        SelectField.SetActive(true);
    }

    public void enableSelect_inverse()
    {
        SelectField.SetActive(false);
    }

    public void disableSelect()
    {
        pushToActionStack(this.gameObject, 0, 0, 0, ActionData.Action.disableSelect);
        SelectField.SetActive(false);
    }

    public void disableSelect_inverse()
    {
        SelectField.SetActive(true);
    }


    private void pushToActionStack(GameObject node, NodeColor color, NodeDirection dir,int oldValue, ActionData.Action action)
    {
        treeRoot.GetComponent<NodeTree>().PushToActionStack(node,color,dir,oldValue,0,0,null,action);
    }


    private void logMessage(string s) {
        treeRoot.GetComponent<NodeTree>().logMessage(s);
    }

    //todo test the 'untested' segments in rotations 
    //check nulls in delete + rotation stack 
    //check if updateTreeStructure needs called in replaceNode
    //delete unnesseary TODOS
    //check step by step viability via coroutines
    //finish delete stack
    //IMPORTANT WHEN CALLING rpllce and when deleting dont forget to fic lines
    //make visualiser for colors
}
