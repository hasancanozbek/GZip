
using System.Collections;

namespace GZip.Entity
{
    public class HuffmanResult
    {
        public BitArray BitArray { get; set; }
        public Dictionary<Token, string> Codes { get; set; }
    }
}
