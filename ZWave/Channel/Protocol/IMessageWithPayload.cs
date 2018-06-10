namespace ZWave.Channel.Protocol
{
    interface IMessageWithPayload
    {
        byte[] Payload { get; }
    }
}
