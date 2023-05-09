namespace ZWave.CommandClasses
{
    public class ColorComponent
    {
        public readonly ColorComponentType ID;
        public readonly byte Value;

        public ColorComponent(ColorComponentType id, byte value)
        {
            ID = id;
            Value = value;
        }

        public override string ToString()
        {
            return $"ID:{ID}, Value:{Value}";
        }

        public byte[] ToBytes()
        {
            return new[] { (byte)ID, Value };
        }
    }
}
