using System;
using System.Linq;
using System.Reflection;

using System.Collections.Generic;
using System.Text;

namespace ZWave
{
    public static class EnumConverter
    {
        public static int GetConfigurationValue(Enum @enum, object discriminator = null)
        {
#if PORTABLE
            // get the ConfigurationValueAttribute attributes on the enumeration member
            var field = @enum.GetType().GetRuntimeField(@enum.ToString());
#else
            // get the ConfigurationValueAttribute attributes on the enumeration member
            var field = @enum.GetType().GetMember(@enum.ToString()).First();
#endif

            // get the ConfigurationValueAttribute attributes on the enumeration member
            var attributes = field.GetCustomAttributes(typeof(ConfigurationValueAttribute), false).Cast<ConfigurationValueAttribute>();

            // discriminator passed?
            if (discriminator != null)
            {
                // yes, so match on discrimator value
                var match = attributes.SingleOrDefault(element => object.Equals(element.Discriminator, discriminator));
                if (match != null)
                    return match.Value;
            }

            // no match on discriminator but there are attributes  
            if (attributes.Any())
            {
                // use first attribute without a discriminator
                var match = attributes.SingleOrDefault(element => element.Discriminator == null);
                if (match != null)
                    return match.Value;
            }

            // no match, so use underlingvalue of enum
            return Convert.ToInt32(@enum);
        }

        public static T ParseConfigurationValue<T>(int value, object discriminator = null) where T : struct
        {
#if PORTABLE
            // get the ConfigurationValueAttributes for all enumeration members
            var attributes = typeof(T).GetRuntimeFields()
                .SelectMany(member => member.GetCustomAttributes(typeof(ConfigurationValueAttribute), false), (member, attribute) => new { Member = member, Attrib = (ConfigurationValueAttribute)attribute });
#else

            // get the ConfigurationValueAttributes for all enumeration members
            var attributes = typeof(T).GetMembers()
                .SelectMany(member => member.GetCustomAttributes(typeof(ConfigurationValueAttribute), false), (member, attribute) => new { Member = member, Attrib = (ConfigurationValueAttribute)attribute });
#endif

            // discriminator passed?
            if (discriminator != null)
            {
                // yes, so match on discrimator value
                var matches = attributes.SingleOrDefault(element => element.Attrib.Value == value && object.Equals(element.Attrib.Discriminator, discriminator));
                if (matches != null)
                    return (T)Enum.Parse(typeof(T), matches.Member.Name);
            }

            // discriminator not found (or NULL) but there are attributes  
            if (attributes.Any())
            {
                // use first attribute without a discriminator
                var matches = attributes.SingleOrDefault(element => element.Attrib.Value == value && element.Attrib.Discriminator == null);
                if (matches != null)
                    return (T)Enum.Parse(typeof(T), matches.Member.Name);
            }

            // no match, so use underlingvalue of enum
            return (T)Enum.ToObject(typeof(T), value);
        }
    }

}
