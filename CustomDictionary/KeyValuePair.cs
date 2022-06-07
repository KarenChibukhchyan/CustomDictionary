using System.Text;

namespace CustomDictionary
{
    public struct KeyValuePair
    {
        private string key;
        private string value;

        public KeyValuePair(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public string Key
        {
            get { return key; }
            init { this.key = value; }
        }

        public string Value
        {
            get { return value; }
            init { this.value = value; }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append('[');
            if (Key != null)
            {
                s.Append(Key.ToString());
            }

            s.Append(", ");
            if (Value != null)
            {
                s.Append(Value.ToString());
            }

            s.Append(']');
            return s.ToString();
        }
    }
}