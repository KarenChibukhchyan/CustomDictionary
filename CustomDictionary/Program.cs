using System;
using static System.Console;

namespace CustomDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            // ------------------ creating custom dictionary ------------------
            WriteLine("Creating custom dictionary");
            _Dictionary dict1 = new _Dictionary();
            dict1.Add("key1", "value1");
            dict1.Add("key2", "value2");
            dict1.Add("key3", "value3");
            dict1.Add("key4", "value4");
            dict1.Add("key5", "value5");
            dict1.Add("key6", "value6");

            // ------------------ print out dictionary using custom ToString() ------------------
            WriteLine(dict1);
            WriteLine();

            // ------------------ getting element with key ------------------
            var key = "key3";
            WriteLine($"Getting element with key: {key}");
            WriteLine($"Value equals {dict1[key]}");

            try
            {
                var key2 = "key3456456456";
                WriteLine($"Getting element with key: {key2}");
                WriteLine($"Value equals {dict1[key2]}"); // error
            }
            catch (Exception e)
            {
                WriteLine(e);
            }

            // ------------------ creating new dictionary from existing ------------------
            _Dictionary dict2 = new _Dictionary(dict1, StringComparer.InvariantCulture);


            // ------------------ Count ------------------
            WriteLine($"Count of {nameof(dict2)}: {dict2.Count}");

            // ------------------ Keys ------------------
            foreach (var key1 in dict2.Keys)
            {
                Write(key1 + ",\t");
            }

            WriteLine();


            // ------------------ Values ------------------
            foreach (var value in dict2.Values)
            {
                Write(value + ",\t");
            }

            WriteLine();

            // ------------------ ContainsKey ------------------
            var key3 = "key1";
            WriteLine($"{nameof(dict2)} contains key {key3}: {dict2.ContainsKey(key3)}");

            key3 = "key11111";
            WriteLine($"{nameof(dict2)} contains key {key3}: {dict2.ContainsKey(key3)}");


            // ------------------ Clear ------------------
            dict2.Clear();
            WriteLine($"{nameof(dict2)} after clearing {dict2.ToString()}");
            WriteLine($"Count of {nameof(dict2)} after clearing {dict2.Count}");


            // ------------------ ContainsValue ------------------
            string value1 = "value1";
            WriteLine($"{nameof(dict2)} contains value {value1}: {dict2.ContainsValue(value1)}");
            WriteLine($"{nameof(dict1)} contains value {value1}: {dict1.ContainsValue(value1)}");


            // ------------------ CopyTo ------------------
            KeyValuePair[] keyValuePairs =
            {
                new KeyValuePair() {Key = "key101", Value = "value101"},
                new KeyValuePair() {Key = "key102", Value = "value102"},
                new KeyValuePair() {Key = "key103", Value = "value103"},
            };
            WriteLine($"{nameof(keyValuePairs)} before copying:");
            foreach (var element in keyValuePairs)
            {
                WriteLine(element.ToString());
            }

            WriteLine();
            WriteLine($"{nameof(keyValuePairs)} after copying:");

            dict1.CopyTo(ref keyValuePairs, index: 2);
            foreach (var element in keyValuePairs)
            {
                WriteLine(element.ToString());
            }

            WriteLine();
            // ------------------ Remove ------------------
            WriteLine($"{nameof(dict1)} before deletion");
            WriteLine(dict1);
            WriteLine();
            string key4 = "key4";
            dict1.Remove(key4);
            WriteLine($"{nameof(dict1)} after deletion key: {key4}");
            WriteLine(dict1);
        }
    }
}