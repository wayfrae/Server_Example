[System.Serializable]
public abstract class GameMessage
{
    public OperationCode Code { set; get; }
}

public enum OperationCode : byte { None = 0, CreateAccount = 1, Spawn, Move, Shoot, Hazards }


