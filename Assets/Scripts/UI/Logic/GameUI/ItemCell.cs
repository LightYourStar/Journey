using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum MoveType
{
    None = 0, //不可移动
    CanMove = 1, //可以移动
}

public class ItemData
{
    public string Str;

    public int Index;

    public MoveType MoveType;
}



public class ItemCell : MonoBehaviour
{


    public Text text;

    public Image image;

    public ItemData itemData;

    private int index = -1; 

    // Start is called before the first frame update
    void Start()
    {
        text.gameObject.SetActive(false);
        image.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(itemData == null)
        {
            text.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            return;
        }

        if(!string.IsNullOrEmpty(itemData.Str))
        {
            text.gameObject.SetActive(true);
            text.text = itemData.Str;
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }


    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void UpdateItemData(ItemData data)
    {
        itemData = data;
    
        if(itemData != null)
        {
            itemData.Index = index;
        }
    }
    

    public bool IsHadData()
    {
        return itemData != null;
    }

    public bool IsCanMove()
    {
        if (itemData == null) return false;
        return itemData.MoveType == MoveType.CanMove;
    }

}
