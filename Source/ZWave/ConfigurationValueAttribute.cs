using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ConfigurationValueAttribute : Attribute
    {
        public int Value { get; }
        public object Discriminator { get; }

        public ConfigurationValueAttribute(int value, object discriminator = null)
        {
            Value = value;
            Discriminator = discriminator;
        }

        public override string ToString()
        {
            return $"Value: {Value}, Discriminator: {Discriminator}";
        }
    }

}
