using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //ApplyModifiedProperties();

        var gameManager = target as GameManager;

        // Randomise board settings
        gameManager.randomiseBoard = GUILayout.Toggle(gameManager.randomiseBoard, "Randomise Board");

        // Resize number of spaces list
        int cur = gameManager.number.Count;
        int sz = gameManager.boardSpaceMaterials.Length;

        if (sz < cur)
        {
            gameManager.number.RemoveRange(sz, cur - sz);
        }
        else if (sz > cur)
        {
            if (sz > gameManager.number.Capacity)
            {
                gameManager.number.Capacity = sz;
            }
            gameManager.number.AddRange(Enumerable.Repeat(default(int), sz - cur));
        }

        if (gameManager.randomiseBoard)
        {
            //Debug.Log(gameManager.number.Capacity);
            
            for (int i = 0; i < gameManager.number.Capacity; i++)
            {
                if (i >= gameManager.number.Capacity)
                    Debug.Log("Problem");
                
                gameManager.number[i] = EditorGUILayout.IntField("Number of Board Space Type " + i + ":", gameManager.number[i]);
            }
        }
    }
}
