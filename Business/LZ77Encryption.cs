using GZip.Entity;

namespace GZip.Business
{
    public class LZ77Encryption
    {
        private string fileContext = string.Empty;
        private int fileContextIndex;
        private int searchBufferSize;
        private int lookAheadBufferSize;
        private char[] slidingWindow;
        private List<Token> tokens;

        public LZ77Encryption()
        {
            fileContextIndex = 0;
            searchBufferSize = 7;
            lookAheadBufferSize = 6;
            slidingWindow = new char[searchBufferSize + lookAheadBufferSize];
            tokens = new List<Token>();
        }
        public LZ77Encryption(string FileContext) : this()
        {
            fileContext = FileContext;
        }

        public List<Token> EncryptContext()
        {
            try
            {
                setContextToWindow();
                while (IsLookAheadBufferNotNull())
                {
                    var token = searchForMatching();
                    tokens.Add(token);
                    slideWindow(token.TotalOfMatchedCharacters == 0 ? 1 : token.TotalOfMatchedCharacters + 1);
                }

                //Print the tokens
                //foreach (var token in tokens)
                //{
                //    Console.WriteLine("(" + token.Offset + "," + token.TotalOfMatchedCharacters + "," + token.UnmatchedCharacter + ")");
                //}
            }
            catch (IOException e)
            {
                Console.Write("The file could not be read: ");
                Console.WriteLine(e.Message);
            }
            return tokens;
        }

        private void setContextToWindow()
        {
            for (int i = searchBufferSize; i < slidingWindow.Length; i++)
            {
                slidingWindow[i] = fileContext[fileContextIndex];
                fileContextIndex++;
            }
        }

        private void slideWindow(int slidingStep)
        {
            // Slide the window to left by specified step
            var tempArray = slidingWindow.ToArray();
            for (int i = slidingWindow.Length - 1; i - slidingStep >= 0; i--)
            {
                slidingWindow[i - slidingStep] = tempArray[i];
            }

            // Fill the look ahead buffer
            for (int i = slidingWindow.Length - slidingStep; i < slidingWindow.Length; i++)
            {
                if (fileContextIndex < fileContext.Length)
                {
                    slidingWindow[i] = fileContext[fileContextIndex];
                    fileContextIndex++;
                }
                else
                {
                    slidingWindow[i] = '\0';
                }
            }
        }

        private Token searchForMatching()
        {
            short matchedCharacters = 0;
            short tempMatchedCharacters = 0;
            Token? token = default;
            for (int index = searchBufferSize - 1; index >= 0; index--)
            {
                if (slidingWindow[searchBufferSize].Equals(slidingWindow[index]))
                {
                    tempMatchedCharacters = 1;
                    for (int i = index + 1; i < searchBufferSize; i++)
                    {
                        if (tempMatchedCharacters != lookAheadBufferSize)
                        {
                            if (slidingWindow[i].Equals(slidingWindow[searchBufferSize + tempMatchedCharacters]))
                            {
                                tempMatchedCharacters++;
                            }
                            else
                            {
                                i = searchBufferSize;
                            }
                        }                    
                    }
                    if (tempMatchedCharacters > matchedCharacters)
                    {
                        matchedCharacters = tempMatchedCharacters;

                        char unmatchedCharacter;
                        if (matchedCharacters == lookAheadBufferSize)
                        {
                            unmatchedCharacter = slidingWindow[searchBufferSize + lookAheadBufferSize - 1];
                            matchedCharacters = (short)(lookAheadBufferSize - 1);
                        }
                        else
                        {
                            unmatchedCharacter = slidingWindow[searchBufferSize + matchedCharacters];
                        }

                        token = new Token()
                        {
                            Offset = (short)(searchBufferSize - index),
                            TotalOfMatchedCharacters = matchedCharacters,
                            UnmatchedCharacter = unmatchedCharacter
                        };
                    }
                }
            }

            if (token == null)
            {
                token = new Token()
                {
                    Offset = 0,
                    TotalOfMatchedCharacters = 0,
                    UnmatchedCharacter = slidingWindow[searchBufferSize]
                };
            }
            return token;
        }


        private bool IsLookAheadBufferNotNull()
        {
            if ((slidingWindow[searchBufferSize - 1] != '\0' && slidingWindow[searchBufferSize] != '\0') ||
                 (slidingWindow[searchBufferSize - 1] == '\0' && slidingWindow[searchBufferSize] != '\0'))
            {
                return true;
            }
            return false;
        }

        public string DecodeToken(List<Token> decodedTokenList)
        {
            var decodedText = string.Empty;
            foreach (var token in decodedTokenList)
            {
                if (token.Offset == 0)
                {
                    decodedText += token.UnmatchedCharacter;
                }
                else if (decodedText.Length > token.Offset)
                {
                    var index = decodedText.Length - token.Offset;
                    for (var i = index; i < index + token.TotalOfMatchedCharacters; i++)
                    {
                        decodedText += decodedText[i];
                    }
                    decodedText += token.UnmatchedCharacter.ToString();
                    index = 0;
                }
            }
            return decodedText;
        }
    }
}
