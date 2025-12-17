using System;
using UnityEngine;
using UnityEditor;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ExcelComparer : EditorWindow
{
    [MenuItem("Tools/Excel Comparer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ExcelComparer>("Excel Comparer");
    }

    private string filePathA = "";
    private string filePathB = "";
    private int columnAIndex = 1;
    private int columnEIndexA = 5;
    private int columnEIndexB = 5;

    void OnGUI()
    {
        GUILayout.Label("Excel Comparer", EditorStyles.boldLabel);

        if (GUILayout.Button("选择旧表"))
        {
            filePathA = EditorUtility.OpenFilePanel("选择旧表", "", "xlsx");
        }
        GUILayout.Label($"旧表路径: {filePathA}");
        columnEIndexA = EditorGUILayout.IntField("目标列", columnEIndexA);

        if (GUILayout.Button("选择新表"))
        {
            filePathB = EditorUtility.OpenFilePanel("选择新表", "", "xlsx");
        }
        GUILayout.Label($"新表路径: {filePathB}");
        columnEIndexB = EditorGUILayout.IntField("目标列", columnEIndexB);

        if (GUILayout.Button("运行"))
        {
            CompareAndMarkExcelFiles(filePathA, filePathB, columnAIndex, columnEIndexA, columnEIndexB);
        }
    }

    void CompareAndMarkExcelFiles(string pathA, string pathB, int columnA, int columnEInA, int columnEInB)
    {
        // 存储 A 表中 A 列元素及其对应的 E 列元素
        Dictionary<string, string> aColumnValues = new Dictionary<string, string>();
        List<int> rowsToMarkBlue = new List<int>(); // 存储新增行的索引
        List<int> rowsToMarkRed = new List<int>();  // 存储改动行的索引

        try
        {
            using (ExcelPackage packageA = new ExcelPackage(new FileInfo(pathA)))
            {
                ExcelWorksheet sheetA = packageA.Workbook.Worksheets.FirstOrDefault();
                if (sheetA == null)
                {
                    Debug.LogError($"No worksheets found in file {pathA}");
                    return;
                }
                int rowCountA = sheetA.Dimension.Rows;
                int columnCountA = sheetA.Dimension.Columns;

                // 检查输入的列索引是否超出范围
                if (columnA >= columnCountA || columnEInA >= columnCountA)
                {
                    Debug.LogError($"输入的列索引超出表 A 的列范围");
                    return;
                }

                for (int i = 5; i <= rowCountA; i++)
                {
                    string aValue = sheetA.Cells[i, columnA].Value?.ToString();
                    string eValue = sheetA.Cells[i, columnEInA].Value?.ToString();
                    if (aValue!= null &&!aValue.StartsWith("#"))
                    {
                        aColumnValues[aValue] = eValue;
                    }
                }
            }

            using (ExcelPackage packageB = new ExcelPackage(new FileInfo(pathB)))
            {
                ExcelWorksheet sheetB = packageB.Workbook.Worksheets.FirstOrDefault();
                if (sheetB == null)
                {
                    Debug.LogError($"No worksheets found in file {pathB}");
                    return;
                }
                int rowCountB = sheetB.Dimension.Rows;
                int columnCountB = sheetB.Dimension.Columns;

                // 检查输入的列索引是否超出范围
                if (columnA >= columnCountB || columnEInB >= columnCountB)
                {
                    Debug.LogError($"输入的列索引超出表 B 的列范围");
                    return;
                }

                for (int i = 5; i <= rowCountB; i++)
                {
                    string bAValue = sheetB.Cells[i, columnA].Value?.ToString();
                    string bEValue = sheetB.Cells[i, columnEInB].Value?.ToString();
                    if (bAValue!= null &&!bAValue.StartsWith("#"))
                    {
                        if (aColumnValues.ContainsKey(bAValue) && aColumnValues[bAValue]!= bEValue)
                        {
                            rowsToMarkRed.Add(i); // 改动的行
                        }
                        else if (!aColumnValues.ContainsKey(bAValue))
                        {
                            rowsToMarkBlue.Add(i); // 新增的行
                        }
                    }
                }

                // 标记新增的行
                foreach (int rowIndex in rowsToMarkBlue)
                {
                    var cell = sheetB.Cells[rowIndex, columnA];
                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                }

                // 标记改动的行
                foreach (int rowIndex in rowsToMarkRed)
                {
                    var cell = sheetB.Cells[rowIndex, columnA];
                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                }

                packageB.Save();
            }

            int blueCount = rowsToMarkBlue.Count;
            int redCount = rowsToMarkRed.Count;
            EditorUtility.DisplayDialog("操作完成", $"运行成功，共有 {blueCount} 行新增(蓝色) 和 {redCount} 行改动(红色)", "确定");
        }
        catch (Exception e)
        {
            Debug.LogError($"运行时出错: {e.Message}");
        }
    }
}