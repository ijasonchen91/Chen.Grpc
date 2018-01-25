using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chen.Grpc.Extensions
{
    public static class MetadataExtension
    {
        //public const string BinarySuffix = "-bin";

        /// <summary>
        /// Get metdata entry. If does not exists, return null.
        /// </summary>
        public static Metadata.Entry Get(this Metadata metadata, string key, bool ignoreCase = true)
        {
            for (int i = 0; i < metadata.Count; i++)
            {
                var entry = metadata[i];
                if (ignoreCase)
                {
                    if (entry.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return entry;
                    }
                }
                else
                {
                    if (entry.Key == key)
                    {
                        return entry;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get metdata string value. If does not exists, return null.
        /// </summary>
        public static string GetValue(this Metadata metadata, string key, bool ignoreCase = true)
        {
            for (int i = 0; i < metadata.Count; i++)
            {
                var entry = metadata[i];
                if (ignoreCase)
                {
                    if (entry.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return entry.Value;
                    }
                }
                else
                {
                    if (entry.Key == key)
                    {
                        return entry.Value;
                    }
                }
            }
            return null;
        }

    }
}
