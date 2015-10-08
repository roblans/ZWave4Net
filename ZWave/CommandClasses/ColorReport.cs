using System;
using System.Linq;
using System.Text;

namespace ZWave.CommandClasses
{
    public class ColorReport : NodeReport
    {
        public readonly byte WarmWhite;
        public readonly byte ColdWhite;
        public readonly byte Red;
        public readonly byte Green;
        public readonly byte Blue;

        internal ColorReport(Node node, byte[] payload) : base(node)
        {
            for (int i = 0; i < payload.Length; i += 2)
            {
                var pair = payload.Skip(i).Take(2).ToArray();
                if (pair.Length < 2)
                    break;

                switch (pair[0])
                {
                    case 0x00:
                        WarmWhite = pair[1];
                        break;
                    case 0x01:
                        ColdWhite = pair[1];
                        break;
                    case 0x02:
                        Red = pair[1];
                        break;
                    case 0x03:
                        Green = pair[1];
                        break;
                    case 0x04:
                        Blue = pair[1];
                        break;
                }
            }
        }

        public override string ToString()
        {
            return $"WarmWhite:{WarmWhite},ColdWhite:{ColdWhite},Red:{Red},Green:{Green},Blue:{Blue}";
        }
    }
}
