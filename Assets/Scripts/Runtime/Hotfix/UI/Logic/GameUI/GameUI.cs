using JO.UIManager;
using System.Collections.Generic;
using UnityEngine;
namespace UIManager
{

    public enum MoveDire
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    }

    public partial class GameUI
    {

        private Vector3 upperLeftPos;

        private Vector3 lowerRightPos;


        private Dictionary<int, Vector3> gridPosDic = new Dictionary<int, Vector3>();

        private Dictionary<int, ItemCell> itemDic = new Dictionary<int, ItemCell>();

        private int curPlayIndex = -1;

        private int allGridNum = -1;


        private List<int> moveList = new List<int>();

        //临时写死
        private int line = 7;
        //临时写死
        private int column = 20;

        //临时写死
        private Dictionary<int, string> config = new Dictionary<int, string>();


        private string success = "我是刘江";

        private int startIndex = 0;

        private float CDTime = 0.2f;

        private float lastMoveTime = 0;

        public override void OnInit()
        {
            base.OnInit();

            IsNeedUpdate = true;

            allGridNum = line * column;

            config.Add(36, "我");
            config.Add(37, "是");
            config.Add(38, "江");
            config.Add(39, "刘");

            m_Item_RectTransform.gameObject.SetActive(false);
            RectTransform rectTransform = m_Grid_RectTransform;
            Vector3[] corners = new Vector3[4];

            rectTransform.GetWorldCorners(corners);

            upperLeftPos = corners[1];
            lowerRightPos = corners[3];

            gridPosDic.Clear();
            itemDic.Clear();

            float width = (lowerRightPos.x - upperLeftPos.x) / column ;
            float height = (upperLeftPos.y - lowerRightPos.y) / line;
            for (int i = 0; i < line; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int index = i * column + j;
                    Vector3 pos = new Vector3(upperLeftPos.x + (j + 0.5f) * width, upperLeftPos.y - (i + 0.5f) * height, 0);
                    gridPosDic.Add(index, pos);
                    var obj = GameObject.Instantiate(m_Item_RectTransform.gameObject, rectTransform, false);
                    obj.SetActive(true);
                    obj.transform.position = pos;
                    obj.name = index.ToString();
                    var itemCell = obj.GetComponent<ItemCell>();
                    if (itemCell != null)
                    {
                        itemCell.SetIndex(index);
                        itemCell.UpdateItemData(null);
                    }
                    itemDic.Add(index, itemCell);
                }
            }
        }

        public override void OnOpen(object param1 = null, object param2 = null, object param3 = null)
        {
            SetStartShow();
        }

        public override void OnClose()
        {
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if(curPlayIndex >= 0 && curPlayIndex < allGridNum)
            {
                if(gridPosDic.TryGetValue(curPlayIndex, out var pos))
                {
                    m_play_Image.transform.position = pos;
                }
            }

            float cdTime = Time.realtimeSinceStartup - lastMoveTime;
            if (cdTime < CDTime) return;
            moveList.Clear();
            bool isCanMove = false;
            MoveDire dire = MoveDire.Left;
            if (Input.GetKey(KeyCode.W))
            {
                isCanMove = GetMoveList(curPlayIndex, MoveDire.Up, ref moveList);
                dire = MoveDire.Up;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                isCanMove = GetMoveList(curPlayIndex, MoveDire.Down, ref moveList);
                dire = MoveDire.Down;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                isCanMove = GetMoveList(curPlayIndex, MoveDire.Left, ref moveList);
                dire = MoveDire.Left;
            }
            else if(Input.GetKey(KeyCode.D))
            {
                isCanMove = GetMoveList(curPlayIndex, MoveDire.Right, ref moveList);
                dire = MoveDire.Right;
            }

            if (isCanMove)
            {
                for (int i = moveList.Count - 1; i >= 0; i--)
                {
                    int curIndex = moveList[i];
                    int newIndex = GetNewIndex(curIndex, dire);
                    ItemCell curItem = itemDic[curIndex];

                    ItemCell newItem = itemDic[newIndex];

                    if (curItem != null && newItem != null)
                    {
                        newItem.UpdateItemData(curItem.itemData);
                        curItem.UpdateItemData(null);

                    }
                }
                moveList.Clear();
                curPlayIndex = GetNewIndex(curPlayIndex, dire);
                lastMoveTime = Time.realtimeSinceStartup;

                CheckFinish();
            }


        }

        private void SetStartShow()
        {
            foreach (var item in config)
            {
                int index = item.Key;
                string des = item.Value;
                ItemCell itemCell = itemDic[index];
                if (itemCell != null)
                {
                    ItemData itemData = new ItemData();
                    itemData.Str = des;
                    itemData.MoveType = (index == 38 || index == 39)? MoveType.CanMove : MoveType.None;
                    itemCell.UpdateItemData(itemData);
                }
            }
            curPlayIndex = startIndex;
        }


        private bool GetMoveList(int index,MoveDire dire, ref List<int> list)
        {
            index = GetNewIndex(index, dire);

            if (itemDic.TryGetValue(index, out var itemCell))
            {
                if (itemCell.IsHadData())
                {
                    if (itemCell.IsCanMove())
                    {
                        list.Add(index);
                        return GetMoveList(index, dire, ref list);
                    }
                    else
                    {
                        list.Clear();
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            }
            else
            {
                list.Clear();
                return false;
            }

        }


        private int GetNewIndex(int index, MoveDire dire)
        {
            switch (dire)
            {
                case MoveDire.Up:
                    index = index - column;
                    break;
                case MoveDire.Down:
                    index = index + column;
                    break;

                case MoveDire.Left:
                    index = index - 1;
                    break;

                case MoveDire.Right:
                    index = index + 1;
                    break;

            }
            return index;
        }


        private void CheckFinish()
        {

            string tempStr = string.Empty;
            for(int i = 0;i < allGridNum;i++)
            {
                var itemCell = itemDic[i];
                if (itemCell.IsHadData())
                {
                    tempStr += itemCell.itemData.Str;
                    if (tempStr.Equals(success))
                    {
                        UIMgr.OpenUI(UIConf.StartUI, "恭喜通关！！");
                    }
                }
                else
                {
                    tempStr = string.Empty;
                }


            }
        }

    }
}