using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Colonisation.Common;

public static class Json
{
    public static IEnumerable<T> Load<T>(JsonSerializer jsonSerializer, string filename,
            Predicate<T>? filter = default)
        where T : class
    {
        using TextReader textReader = new StreamReader(filename);
        using JsonTextReader jsonReader = new(textReader);

        // This pattern, while more code than Deserialise<T[]>(), minimizes memory use on
        // multi GB input files.
        while (jsonReader.Read())
        {
            if (jsonReader.TokenType == JsonToken.StartObject)
            {
                T? current = jsonSerializer.Deserialize<T>(jsonReader);
                if (current != null
                    && ((filter != default && filter(current)) || filter == default))
                {
                    yield return current;
                }
            }
        }
    }
}
