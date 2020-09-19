// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aguacongas.Identity.Firestore
{
    static class Map
    {
        public static T FromDictionary<T>(Dictionary<string, object> dictionary) where T : new()
        {
            if (dictionary == null)
            {
                return default;
            }

            var user = new T();
            var type = user.GetType();
            foreach (var key in dictionary.Keys)
            {
                var property = type.GetProperty(key);
                var value = dictionary[key];
                var valueType = value?.GetType();
                if (value != null && valueType != property.PropertyType)
                {
                    if (valueType == typeof(Timestamp))
                    {
                        value = ConvertFromDate(property, value);
                    }
                    else
                    {
                        value = Convert.ChangeType(value, property.PropertyType);
                    }
                }
                property.SetValue(user, value);
            }
            return user;
        }
        public static Dictionary<string, object> ToDictionary<T>(T obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties()
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string));
            var dictionary = new Dictionary<string, object>(properties.Count());
            foreach (var property in properties)
            {
                dictionary.Add(property.Name, property.GetValue(obj));
            }
            return dictionary;
        }

        private static object ConvertFromDate(PropertyInfo property, object value)
        {
            if (property.PropertyType == typeof(DateTimeOffset) || property.PropertyType == typeof(DateTimeOffset?))
            {
                value = ((Timestamp)value).ToDateTimeOffset();
            }
            else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
            {
                value = ((Timestamp)value).ToDateTime();
            }

            return value;
        }

    }
}
