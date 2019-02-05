using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Message_Spawn : GameMessage
{
    public string X { get; set; }
    public string Y { get; set; }


    public Message_Spawn()
    {
        Code = OperationCode.Spawn;
    }
}
