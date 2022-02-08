using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private Vector3Int _currentPos;
    private Vector3Int _destination;

    private List<Vector3Int> _routePath = new List<Vector3Int>();

    private Camera _mainCam;

    private bool _isMove;
    private int _idx;
    private Vector3 _nextPos;
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private bool _cornerCheck = true;


    private void Start()
    {
        _currentPos = TilemapInfo.Instance.GetStartCellPos();
        transform.position = TilemapInfo.Instance.GetWorldPos(_currentPos);
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mPos = Input.mousePosition;
            mPos.z = 0;
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(mPos);
            Vector3Int cellPos = TilemapInfo.Instance.GetCellPos(worldPos);

            Debug.Log(cellPos);
            
            if (TilemapInfo.Instance.CanMove(cellPos))
            {
                _destination = cellPos;
                if (CalcRoute())
                {
                    //PrintRoute();
                    _idx = 0;
                    SetNextTarget();
                    _isMove = true;
                }
            }
        }

        if(_isMove)
        {
            Vector3 dir = _nextPos - transform.position;
            if ( dir.magnitude <= 0.05f)
            {
                SetNextTarget();
            }

            transform.position += dir.normalized * Time.deltaTime * _speed;
        }
    }

    private void SetNextTarget()
    {
        if(_idx >= _routePath.Count)
        {
            _isMove = false;
            return;
        }
        _currentPos = _routePath[_idx];
        _nextPos = TilemapInfo.Instance.GetWorldPos(_currentPos);
        _idx++;
    }

    public void PrintRoute()
    {
        for(int i = 0; i < _routePath.Count; i++)
        {
            Debug.Log(_routePath[i]);
        }
    }

    #region A��Ÿ
    //F = G + H
    // G�� ������� �̵��� �� ���, ���⼭���� ������������ ���� ���(��ֹ� ���ٰ� �����ϰ�)
    private PriorityQueue<Node> _openList;
    private List<Node> _closeList;

    
    private bool CalcRoute()
    {
        _openList = new PriorityQueue<Node>();
        _closeList = new List<Node>();

        //�� ó�� ���������� openList�� �ִ´�.
        _openList.Push(new Node { pos = _currentPos, _parent = null, G = 0, F = CaclH(_currentPos) });

        bool result = false;
        int cnt = 0;
        while(_openList.Count > 0)
        {
            Node n = _openList.Pop();
            FindOpenList(n);
            _closeList.Add(n);
            Debug.Log(n.pos);
            if(n.pos == _destination)
            {
                result = true;
                break;
            }

            cnt++;
            if (cnt >= 1000)
            {
                Debug.Log("While�� 1000ȸ!!");
                break;
            }
                
        }

        
        if(result)
        {
            _routePath.Clear();
            Node last = _closeList[_closeList.Count - 1];
            //_routePath.Add(_destination);
            while(last._parent != null)
            {
                _routePath.Add(last.pos);
                last = last._parent;
            }
            _routePath.Reverse();
        }

        return result;
    }

    private void FindOpenList(Node currentNode)
    {
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                Vector3Int next = currentNode.pos + new Vector3Int(j, i, 0);
                
                //�ش� ������ �̹� �湮�ߴ�.
                Node n = _closeList.Find(x => x.pos == next);
                if (n != null) continue;

                //�ڳʸ��� �ش� �ڳʿ� ��ֹ��� �ִٸ� ����
                if (_cornerCheck &&
                    (!TilemapInfo.Instance.CanMove(new Vector3Int(next.x, currentNode.pos.y, 0))
                    || !TilemapInfo.Instance.CanMove(new Vector3Int(currentNode.pos.x, next.y, 0)))
                    )
                {
                    continue;
                }

                if (TilemapInfo.Instance.CanMove(next))
                {
                    //������忡�� ������� ���� ��� + ���������� �Դ� ����� ���ؼ� G�� ������ְ�
                    float g = (currentNode.pos - next).magnitude + currentNode.G;

                    Node nextOpenNode = new Node { pos = next, _parent = currentNode, G = g, F = g + CaclH(next) };
                    Node exists = _openList.Contains(nextOpenNode);
                    
                    //Debug.Log(nextOpenNode.pos + ", " + nextOpenNode.F);

                    if (exists != null)
                    {
                        //�̹� openList�� �����Ѵٸ� ���� �� ª������ ����ؼ� �־��ش�. �ƴϸ� �ƹ��ϵ� �����൵ �ȴ�.
                        if(nextOpenNode.G < exists.G)
                        {
                            exists.G = nextOpenNode.G;
                            exists.F = nextOpenNode.F;
                            exists._parent = nextOpenNode._parent;
                        }
                    }else
                    {
                        //�������� ������ �־��ְ�
                        _openList.Push(nextOpenNode);
                    }
                    
                }
            }
        }
    }

    //��ġ�κ��� F���� ���ϴ� �Լ�
    private float CaclH(Vector3Int pos)
    {
        Vector3Int distance = _destination - pos;
        return distance.magnitude;
    }

    #endregion
}
