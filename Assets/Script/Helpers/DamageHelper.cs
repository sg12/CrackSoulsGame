
public enum MsgType
{
    DAMAGED,
    DEAD,
}

public interface DamageReceiver
{
    void OnReceiveMessage(MsgType type, object sender, object msg);
}