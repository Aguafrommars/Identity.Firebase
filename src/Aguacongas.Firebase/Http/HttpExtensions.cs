using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    public static class HttpExtensions
    {
        public static StringContent CreateJsonContent<T>(this HttpClient httpClient, T data, JsonSerializerSettings jsonSerializerSettings = null, bool requestEtag = false, string etag = null)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data, jsonSerializerSettings), Encoding.UTF8, "application/json");
            var headers = content.Headers;
            headers.SetIfMath(etag);
            headers.SetRequestEtag(requestEtag);

            return content;
        }

        public static void SetRequestEtag(this HttpHeaders headers, bool requestEtag)
        {
            if (requestEtag)
            {
                headers.Add("X-Firebase-ETag", new[] { "true" });
            }
        }

        public static void SetIfMath(this HttpHeaders headers, string etag)
        {
            if (!string.IsNullOrEmpty(etag))
            {
                headers.Add("if-match", new[] { etag });
            }
        }

        public static void SetIfMatch(this StringContent content, string etag = null)
        {

        }
        public static async Task<FirebaseResponse<T>> DeserializeResponseAsync<T>(this HttpResponseMessage response, JsonSerializerSettings jsonSerializerSettings = null)
        {
            await response.EnsureIsSuccess();
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var type = typeof(T);
            if (jsonResponse.StartsWith("{")
                && type.GetInterfaces().Any(i => i == (typeof(IEnumerable))))
            {
                var reader = new JsonTextReader(new StringReader(jsonResponse));
                var listType = typeof(List<>);
                var genericTypes = type.GetGenericArguments();
                var makeme = listType.MakeGenericType(genericTypes);
                var list = Activator.CreateInstance(makeme) as IList;
                var itemType = genericTypes[0];

                var builder = new StringBuilder('[');
                while(reader.Read())
                {
                    if (reader.Depth == 1)
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            builder.Append("{");
                            builder.AppendFormat("\"{0}\": ", reader.Value);
                            continue;
                        }
                        if (reader.TokenType == JsonToken.EndArray || reader.TokenType == JsonToken.EndArray)
                        {
                            builder.Append("}");
                            continue;
                        }
                    }
                    if (reader.Depth > 0)
                    {
                        switch(reader.TokenType)
                        {
                            case JsonToken.EndArray:
                                builder.Append("[");
                                break;
                            case JsonToken.EndObject:
                                builder.Append("}");
                                break;
                            case JsonToken.PropertyName:
                                builder.AppendFormat("\"{0}\": ", reader.Value);
                                break;
                            case JsonToken.StartArray:
                                builder.Append("[");
                                break;
                            case JsonToken.StartObject:
                                builder.Append("{");
                                break;
                            case JsonToken.String:
                                builder.AppendFormat("\"{0}\"", reader.Value);
                                break;
                            default:
                                builder.Append(reader.Value);
                                break;
                        }
                    }
                }

                builder.Append(']');
                jsonResponse = builder.ToString();
            }

            return new FirebaseResponse<T>
            {
                Data = JsonConvert.DeserializeObject<T>(jsonResponse, jsonSerializerSettings),
                Etag = response.Headers.ETag?.Tag
            };
        }

        public static async Task EnsureIsSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new FirebaseException(response.StatusCode, response.ReasonPhrase, await response.Content.ReadAsStringAsync(), response.Headers.ETag?.Tag);
            }
        }
    }
}
