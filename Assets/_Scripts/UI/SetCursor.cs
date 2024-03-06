using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    [SerializeField] Texture2D CursorImg; //ToDo: 배열로 저장해둬서 씬에 따라 커서 전환
    void Start()
    {
        Cursor.SetCursor(CursorImg, Vector2.zero, CursorMode.ForceSoftware);
    }

    
}
