using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "Data/PlayerStatus")]
public class PlayerStatus : ScriptableObject
{
        public int maxHp;

        public float moveSpeed;
        public float maxSpeed;
        public float breakSpeed;
        public float jumpSpeed;
        public float dashSpeed;

        public int jumpCount;
        public int dashCount;

        public float dashCoolTime;
        public float dashTime;
}
