using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject cellPrefab;
    
    [Header("Board Container")]
    [SerializeField] private GridLayoutGroup boardContainer;
     
    [Header("Grid Settings")]
    [SerializeField] private Vector2 cellSize = new Vector2(300, 300);
    [SerializeField] private Vector2 spacing = new Vector2(10, 10);
    
    private Cell[] cells;

    public void GenerateBoard(int side,Action<int> OnClickCallback)
    {
        int totalCells = side*side;
        cells = new Cell[totalCells];
        
        SetupGridLayout(side);
        
        for (int i = 0; i < totalCells; i++)
        {
            CreateCell(i,OnClickCallback);
        }
    }

    private void CreateCell(int index,Action<int> OnClickCallback)
    {
        GameObject cellObj = Instantiate(cellPrefab, boardContainer.transform);
        Cell cell = cellObj.GetComponent<Cell>();
        
        if (cell != null)
        {
            cell.Initialize(index); 
            cell.AttachEvent(OnClickCallback);
            cells[index] = cell;
        }
        else
        {
            Debug.LogError("Cell prefab doesn't have a Cell component!");
        }
    }

    public void PerformMove(int index,int turn)
    {
        Cell c =GetCellByIndex(index);
        if (c.value == -1)
        {
            c.PerformMove(turn);
        }
    }

    private void SetupGridLayout(int side)
    {
        GridLayoutGroup grid = boardContainer.GetComponent<GridLayoutGroup>();
        
        if (grid == null)
        {
            grid = boardContainer.gameObject.AddComponent<GridLayoutGroup>();
        }
        
        grid.cellSize = cellSize;
        grid.spacing = spacing;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = side;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
    }

    public Cell GetCellByIndex(int index)
    {
        if (cells != null && index >= 0 && index < cells.Length)
        {
            return cells[index];
        }
        return null;
    }    
}