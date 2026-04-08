using Serilog.Core;
using Serilog.Events;
using System.Reflection;

namespace Playground.Logging
{
    public class SensitiveDataDestructuringPolicy : IDestructuringPolicy
    {
        private static readonly string[] SensitivePropertyNames = new[]
        {
            "password",
            "secret",
            "token",
            "apikey",
            "clientsecret",
            "connectionstring",
            "salt",
            "userid",
            "username"
        };

        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            if (value == null)
            {
                result = null;
                return false;
            }

            var type = value.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (properties.Any(p => SensitivePropertyNames.Any(s => p.Name.ToLowerInvariant().Contains(s))))
            {
                var maskedProperties = new List<LogEventProperty>();

                foreach (var property in properties)
                {
                    var propertyValue = property.GetValue(value);
                    var isSensitive = SensitivePropertyNames.Any(s => property.Name.ToLowerInvariant().Contains(s));

                    var maskedValue = isSensitive
                        ? new ScalarValue("***REDACTED***")
                        : propertyValueFactory.CreatePropertyValue(propertyValue, true);

                    maskedProperties.Add(new LogEventProperty(property.Name, maskedValue));
                }

                result = new StructureValue(maskedProperties);
                return true;
            }

            result = null;
            return false;
        }
    }
}
