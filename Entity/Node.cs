namespace GZip.Entity
{
    public class Node
    {
        public int Frequency { get; set; }
        public Token? Token { get; set; }

        public Node? LeftNode { get; set; }
        public Node? RightNode { get; set; }

    }
}
