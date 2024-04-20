using GZip.Entity;
using System.Collections;

namespace GZip.Business
{
    public class HuffmanEncryption
    {
        private List<Token> tokenList;
        private List<Node> nodeList;
        private List<Node> nodeListForArray;
        private Dictionary<Token, string> codes;
        private BitArray encryptedContext;

        public HuffmanEncryption()
        {
            nodeList = new List<Node>();
            codes = new Dictionary<Token, string>();
            nodeListForArray = new List<Node>();
        }

        public HuffmanEncryption(List<Token> TokenList) : this()
        {
            tokenList = TokenList;
        }

        public HuffmanResult EncryptContext()
        {
            findFrequencies();
            nodeListForArray = nodeList.ToList();
            orderNodeList();
            buidTree();
            FindCodes(nodeList.First(), string.Empty);
            EncryptTokens();
            //var orderedCodes = new Dictionary<Token, string>();
            //foreach (var token in tokenList)
            //{
            //    orderedCodes.Add(token, codes[token]);
            //}
            return new HuffmanResult()
            {
                BitArray = encryptedContext,
                //Codes = orderedCodes
                Codes = codes
            };

        }

        private void findFrequencies()
        {
            foreach (var token in tokenList)
            {
                Node? node = nodeList.FirstOrDefault(f => f.Token.Offset == token.Offset && f.Token.TotalOfMatchedCharacters == token.TotalOfMatchedCharacters && f.Token.UnmatchedCharacter == token.UnmatchedCharacter);
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
                var codeKey = codes.Keys.FirstOrDefault(f => f.Offset == token.Offset && f.TotalOfMatchedCharacters == token.TotalOfMatchedCharacters && f.UnmatchedCharacter == token.UnmatchedCharacter);
                if (codeKey != null)
                {
                    var code = codes[codeKey];
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

        public string SerializeHuffmanCodes(Dictionary<Token, string> huffmanCodes)
        {
            return string.Join(";", huffmanCodes.Select(kvp => $"{SerializeToken(kvp.Key)}:{kvp.Value}"));
        }

        public Dictionary<Token, string> DeserializeHuffmanCodes(string serializedCodes)
        {
            return serializedCodes.Split(';')
                .Select(part => part.Split(':'))
                .ToDictionary(
                    split => DeserializeToken(split[0]),
                    split => split[1]
                );
        }

        public string SerializeToken(Token token)
        {
            return $"{token.Offset},{token.TotalOfMatchedCharacters},{token.UnmatchedCharacter}";
        }

        public Token DeserializeToken(string serializedToken)
        {
            var parts = serializedToken.Split(',');
            return new Token
            {
                Offset = Int16.Parse(parts[0]),
                TotalOfMatchedCharacters = Int16.Parse(parts[1]),
                UnmatchedCharacter = parts[2][0]
            };
        }

        public List<Token> DecodeHuffmanCode(string cryptedContext, Dictionary<Token, string> codes)
        {

            Dictionary<string, Token> reversedCodes = new Dictionary<string, Token>();
            foreach (var pair in codes)
            {
                reversedCodes[pair.Value] = pair.Key;
            }

            var tokenList = new List<Token>();
            string currentCode = string.Empty;
            foreach (char bit in cryptedContext)
            {
                currentCode += bit;

                if (reversedCodes.TryGetValue(currentCode, out Token token))
                {
                    tokenList.Add(token);
                    currentCode = string.Empty;
                }
            }

            return tokenList;
        }

    }
}
