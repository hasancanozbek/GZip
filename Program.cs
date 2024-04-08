
using GZip.Business;

Console.Write("Please enter file path: ");
var filePath = Console.ReadLine();
try
{
    using (var sr = new StreamReader(filePath))
    {
        var fileContext = sr.ReadToEnd();
        LZ77Encryption lz77 = new LZ77Encryption(fileContext);
        var tokens = lz77.EncryptContext();
        HuffmanEncryption huffman = new HuffmanEncryption(tokens);
        var cryptedContext = huffman.EncryptContext();

        Console.WriteLine("File compressed.");
        Console.WriteLine("Original Text : " + fileContext.ToString());

        var cryptedContextString = string.Empty;
        for (int i = 0; i < cryptedContext.Count; i++)
        {
            cryptedContextString += cryptedContext[i] ? "1" : "0";
        }

        Console.WriteLine("Crypted text : " + cryptedContextString);
        Console.WriteLine("--------------------------------------------------------------------");
        Console.WriteLine("File size before compression: " + fileContext.Length + " byte");
        Console.WriteLine("File size after compression: " + cryptedContext.Length / 8 + " byte");
    }
}
catch (IOException e)
{
    Console.Write("The file could not be read: ");
    Console.WriteLine(e.Message);
}