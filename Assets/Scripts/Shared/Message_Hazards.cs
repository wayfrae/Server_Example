using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Message_Hazards : GameMessage
{
    public string hazards;

    public Message_Hazards()
    {
        Code = OperationCode.Hazards;
    }
}
