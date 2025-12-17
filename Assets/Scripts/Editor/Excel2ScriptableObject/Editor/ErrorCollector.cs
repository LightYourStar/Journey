using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ErrorType
{
    //单元格数值错误
    E1001, //数组不能以逗号结尾
}

public class ErrorCollector
{
    private bool IsEnable = true;
    private ExcelToScriptableObjectSetting m_Excel;
    private int m_CellX;
    private int m_CellY;

    private static readonly List<string> m_ErrorList = new List<string>();
    private static readonly List<string> m_ErrorTotal = new List<string>();

    public void SetExcel(ExcelToScriptableObjectSetting excel)
    {
        m_Excel = excel;
        m_ErrorList.Clear();
    }

    public void SetXY(int x, int y)
    {
        m_CellX = x;
        m_CellY = y;
    }

    public void Reset()
    {
        m_ErrorList.Clear();
        m_ErrorTotal.Clear();
    }

    public void VerifyArrayContent(string cellContent)
    {
        if (cellContent.EndsWith(","))
        {
            RecordError(ErrorType.E1001, "数组字段不能以','结尾 :" + cellContent);
        }
    }

    private void RecordError(ErrorType errType, string content)
    {
        var _content = $"【{m_Excel.GetExcelName()}】 row = 【{m_CellX}】 column = 【{m_CellY}】 {errType}  " + content;
        m_ErrorList.Add(_content);
        m_ErrorTotal.Add(_content);
    }

    public bool HasError()
    {
        return IsEnable && m_ErrorList.Count > 0;
    }

    public void ReportErrors()
    {
        if (IsEnable && m_ErrorTotal.Count > 0)
        {
            foreach (var err in m_ErrorTotal)
            {
                Debug.LogError(err);
            }

            EditorUtility.DisplayDialog("Excel To ScriptableObject", "生成数据表有错误，查看控制台错误日志", "OK");
        }
    }
}