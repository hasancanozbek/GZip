namespace GZip.Entity
{
    public class Token
    {
        public Int16 Offset { get; set; }
        public Int16 TotalOfMatchedCharacters { get; set; }
        public char UnmatchedCharacter { get; set; }
    }
}