using GZip.Entity;
using System.Collections;
using System.Reflection.Metadata.Ecma335;

namespace GZip.Business
{
    public class HuffmanEncryption
    {
        private List<Token> tokenList;
        private List<Node> nodeList;
        private List<Node> nodeListForArray;
        private Dictionary<Token, string> codes;
        private BitArray encryptedContext;

        public HuffmanEncryption(List<Token> TokenList)
        {
            tokenList = TokenList;
            nodeList = new List<Node>();
            codes = new Dictionary<Token, string>();
        }

        public BitArray EncryptContext()
        {
            findFrequencies();
            nodeListForArray = nodeList.ToList();
            orderNodeList();
            buidTree();
            FindCodes(nodeList.First(), string.Empty);
            EncryptTokens();
            return encryptedContext;
        }

        private void findFrequencies()
        {
            foreach (var token in tokenList)
            {
                Node node = nodeList.Find(f => f.Token == token);
                if (node != null)
                {
                    node.Frequency++;
                }
                else
                {
                    Node nodeToAdd = new()
                    {
                        Frequency = 1,
                        Token = token
                    };
                    nodeList.Add(nodeToAdd);
                }
            }
        }

        private void orderNodeList()
        {
            nodeList = nodeList.OrderBy(f => f.Frequency).ToList();
        }

        public void buidTree()
        {
            while (nodeList.Count > 1)
            {
                var leftNode = nodeList.First();
                nodeList.Remove(leftNode);
                var rightNode = nodeList.First();
                nodeList.Remove(rightNode);

                var sum = leftNode.Frequency + rightNode.Frequency;
                Node parentNode = new()
                {
                    Frequency = sum,
                    LeftNode = leftNode,
                    RightNode = rightNode
                };

                nodeList.Add(parentNode);
                orderNodeList();
            }
        }

        private void FindCodes(Node node, string code)
        {
            if (node == null) return;

            if (node.Token != null)
            {
                codes.Add(node.Token, code);
            }

            FindCodes(node.LeftNode, code + "0");
            FindCodes(node.RightNode, code + "1");
        }

        private void EncryptTokens()
        {
            var size = calculateBitArraySize();
            
            encryptedContext = new BitArray(size);

            int index = 0;
            foreach (var token in tokenList)
            {
                if (codes.ContainsKey(token))
                {
                    var code = codes[token];
                    for (int i = 0; i < code.Length; i++)
                    {
                        var currentBit = code.ElementAt(i);
                        if (currentBit == '1')
                        {
                            encryptedContext[index] = true;
                        }
                        index++;
                    }
                }
            }
        }

        private int calculateBitArraySize()
        {
            var size = 0;
            var leafNodes = nodeListForArray.ToList();
            foreach (var node in leafNodes)
            {
                var codeSize = codes[node.Token].Length;
                size += codeSize * node.Frequency;
            }
            return size;
        }

    }
}
