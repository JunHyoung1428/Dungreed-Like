using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    [SerializeField] Texture2D CursorImg; //ToDo: 배열로 저장해둬서 씬에 따라 커서 전환
    [SerializeField] Texture2D CursorImg2;
    void Start()
    {
        Cursor.SetCursor(CursorImg, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void LateUpdate()
    {
        if(Manager.UI.isOpenInventory)
        {
            Cursor.SetCursor(CursorImg2, Vector2.zero, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(CursorImg, Vector2.zero, CursorMode.ForceSoftware);
        }
    }

}
