[System.Serializable]
public class Message_CreateAccount : GameMessage
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }


    public Message_CreateAccount()
    {
        Code = OperationCode.CreateAccount;
    }
}
