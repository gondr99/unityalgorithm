using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tree : MonoBehaviour
{
    public GameObject nodePrefab;

    private TreeNode<int> _root = null;

    [SerializeField]
    private InputField _txtInput;

    [SerializeField]
    private int _treeDepth = 0;

    public void AddNode(int number = 0)
    {
        try
        {
            if(number == 0)
                number = int.Parse(_txtInput.text);


            TreeNode<int> newNode = new TreeNode<int>();
            newNode.Data = number;
            
            Vector3 pos = transform.position;
            if (_root == null)
            {
                _root = newNode;
                newNode.Parent = null;
                _treeDepth++;
            }
            else
            {
                pos = Insert(newNode);
                //������� ���� newNode�� ���� ��������
                if(pos == Vector3.zero)
                {
                    throw new Exception("�ߺ����Դϴ�.");
                }
            }
            GameObject nodeObj = Instantiate(nodePrefab, transform);
            newNode.Prefab = nodeObj;
            newNode.SetPosition(pos);

            AdjustPosition(); 
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
   

    private void Start()
    {
        AddNode(50);
        AddNode(75);
        AddNode(25);
        AddNode(10);
        AddNode(35);
        AddNode(65);
        AddNode(90);
        AddNode(100);
        AddNode(110);
        AddNode(60);
        AddNode(73);
        AddNode(30);
        AddNode(38);
        AddNode(5);
        AddNode(20);
        AddNode(2);
        AddNode(8);
        AddNode(15);
        AddNode(23);
        AddNode(27);
        AddNode(34);
        AddNode(36);
        AddNode(40);
        AddNode(55);
        AddNode(64);
        AddNode(70);
        AddNode(74);
        AddNode(95);
        AddNode(85);
        AddNode(88);
        AddNode(78);
        AdjustPosition();
    }

    //Ŭ���� ������ �����ϴ� ���� 
    public void DeleteNode(int number = 0)
    {
        try
        {
            if (number == 0)
                number = int.Parse(_txtInput.text);
            TreeNode<int> node = Find(number);
            if(node == null)
            {
                throw new Exception("�������� �ʴ� ���Դϴ�.");
            }
            //�ڽ��� ���� ���
            if(node.Left == null && node.Right == null)
            {
                //��� �θ𿡰Լ� �ڽ��� ��������
                bool isLeft = IsLeftChild(node);
                if (isLeft) node.Parent.Left = null;
                else node.Parent.Right = null;
                node.Delete();
            }else if(node.Left != null && node.Right == null)
            {
                //�����ڽĸ� �ִ� ��� �ڽ��� ���ʿ��� ���� ������ ���ڸ� ã�Ƽ� �����;� ��.
                TreeNode<int> mostRightChild = node.Left;
                while(mostRightChild.Right != null)
                {
                    mostRightChild = mostRightChild.Right;
                }
                node.Data = mostRightChild.Data;  //������ ���� ��

                bool isLeft = IsLeftChild(mostRightChild); //�θ��� �����ڽ����� üũ
                if(isLeft)
                {
                    mostRightChild.Parent.Left = mostRightChild.Left; //������ ����
                }
                else
                {
                    mostRightChild.Parent.Right = mostRightChild.Left; //������ ����
                }
                if(mostRightChild.Left != null)
                    mostRightChild.Left.Parent = mostRightChild.Parent;
                mostRightChild.Delete();
            }else if(node.Right != null)
            {
                //������ �ڽĸ� �ִ� ��� ���� ���� ���ڸ� ã�Ƽ� �����;� ��.
                TreeNode<int> mostLeftChild = node.Right;
                while (mostLeftChild.Left != null)
                {
                    mostLeftChild = mostLeftChild.Left;
                }
                node.Data = mostLeftChild.Data;
                bool isLeft = IsLeftChild(mostLeftChild); //�θ��� �������� üũ
                if (isLeft)
                {
                    mostLeftChild.Parent.Left = mostLeftChild.Right;
                }else
                {
                    mostLeftChild.Parent.Right = mostLeftChild.Right;
                }
                if (mostLeftChild.Right != null)
                    mostLeftChild.Right.Parent = mostLeftChild.Parent;
                mostLeftChild.Delete();
            }
            //else if(node.Left != null && node.Right != null)
            //{
            //    //�Ѵ� �ִ� ���� ���� �ϳ��� �����ؼ� �����ϸ� ��. ���� ���� �ڵ带 �״�� �������� �ȴ�.
            //    TreeNode<int> mostLeftChild = node.Right;
            //    while (mostLeftChild.Left != null)
            //    {
            //        mostLeftChild = mostLeftChild.Left;
            //    }
            //    Debug.Log(mostLeftChild.Data);
            //    node.Data = mostLeftChild.Data;
            //    bool isLeft = IsLeftChild(mostLeftChild); //�θ��� �������� üũ
            //    if (isLeft)
            //    {
            //        mostLeftChild.Parent.Left = mostLeftChild.Right;
            //    }
            //    else
            //    {
            //        mostLeftChild.Parent.Right = mostLeftChild.Right;
            //    }
            //    if(mostLeftChild.Right != null)
            //        mostLeftChild.Right.Parent = mostLeftChild.Parent;
            //    mostLeftChild.Delete();
            //}

            AdjustPosition(); //��ġ ����
        }catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.Log(e.StackTrace);
        }
    }

    private TreeNode<int> Find(int number)
    {
        TreeNode<int> node = _root;
        if (node == null) return null;
        while(node.Data != number)
        {
            if (node.Data < number)
            {
                if (node.Right == null) return null;
                node = node.Right;
            }
            else if (node.Data > number)
            {
                if (node.Left == null) return null;
                node = node.Left;
            }
        }
        return node;
    }

    
    //���� �ڽ��� ��� true��ȯ
    private bool IsLeftChild(TreeNode<int> node)
    {
        return node.Parent != null && node.Parent.Left == node;
    }


    private Vector3 Insert(TreeNode<int> node)
    {
        TreeNode<int> pointer = _root;
        int depth = 1;
        while(true)
        {
            if(node.Data < pointer.Data) //�ߺ� ��� ���ҽÿ��� ��ȣ�����ϰ� ��������
            {//�������� 
                if (pointer.Left == null )
                {
                    node.Parent = pointer;
                    pointer.Left = node;
                    if (depth + 1 > _treeDepth)
                    {
                        _treeDepth = depth + 1;
                    }
                    return pointer.GetPosition() + new Vector3(-2f, -2f, 0);
                }else
                {
                    pointer = pointer.Left;
                }
            }else if(node.Data > pointer.Data)
            {
                //����������
                if (pointer.Right == null)
                {
                    node.Parent = pointer;
                    pointer.Right = node;
                    if (depth + 1 > _treeDepth)
                    {
                        _treeDepth = depth + 1;
                    }
                    return pointer.GetPosition() + new Vector3(2f, -2f, 0);
                }
                else
                {
                    pointer = pointer.Right;
                }
            }else
            {
                //�ߺ����� �Է� ����.
                return Vector3.zero;
            }
            depth++;
        }
    }

    //Ʈ���� ������ ���� �������� �����ϵ��� ���� => �̰͵� ������
    private void AdjustPosition()
    {        
        //���⼭�� Ʈ���� �ʺ� �켱���� Ž���� �س����� �ϱ� ������ �ʺ�켱 Ž���� ����Ѵ�.
        Queue <TreeNode<int>> bfs = new Queue<TreeNode<int>>();

        bfs.Enqueue(_root); //��Ʈ �ְ� ����
        int depth = 1;
        while(bfs.Count > 0)
        {
            Queue<TreeNode<int>> nextNodes = new Queue<TreeNode<int>>();
            while(bfs.Count > 0)
            {
                TreeNode<int> node = bfs.Dequeue();

                SetNodePosition(node, depth);
                if (node.Left != null)
                    nextNodes.Enqueue(node.Left);
                if (node.Right != null)
                    nextNodes.Enqueue(node.Right);
            }
            depth++;
            bfs = nextNodes;
        }
    }

    private void SetNodePosition(TreeNode<int> node, int depth)
    {
        
        if (depth == 1)
        {
            node.SetPosition(new Vector3(0, 0, 0)); //�������� ����
        }
        else
        {
            Vector3 parentPos = node.Parent.GetPosition();
            bool isLeft = node.Parent.Left != null && node.Parent.Left == node;


            float distance = Mathf.Pow(2, _treeDepth - depth + 1) / 2;

            if (isLeft) //�����϶�
            {
                node.SetPosition(parentPos + new Vector3(-distance, -2f, 0));
            }else //�������� ��
            {
                node.SetPosition(parentPos + new Vector3(distance, -2f, 0));
            }
        }
        SetNodeLine(node);
    }

    private void SetNodeLine(TreeNode<int> node)
    {
        node.Line.SetPosition(0, node.GetPosition());
        if (node.Parent != null)
            node.Line.SetPosition(1, node.Parent.GetPosition());
        else
            node.Line.SetPosition(1, node.GetPosition());
    }
}

