using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    [SerializeField] Texture2D CursorImg; //ToDo: �迭�� �����صּ� ���� ���� Ŀ�� ��ȯ
    void Start()
    {
        Cursor.SetCursor(CursorImg, Vector2.zero, CursorMode.ForceSoftware);
    }

    
}
