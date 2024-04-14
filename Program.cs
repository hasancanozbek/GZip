
using GZip.Business;

Console.Write("Please enter file path: ");
var filePath = Console.ReadLine();
Console.Write("Please enter c to compress, enter d to decompress: ");
var operation = Console.ReadLine();

if (operation.Equals("c"))
{
    try
    {
        using var sr = new StreamReader(filePath);
        var fileContext = sr.ReadToEnd();
        LZ77Encryption lz77 = new(fileContext);
        var tokens = lz77.EncryptContext();
        HuffmanEncryption huffman = new(tokens);
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

        int lastIndex = filePath.LastIndexOf('\\');
        string basePath = filePath.Substring(0, lastIndex + 1);
        string newPath = basePath + "compressedFile";
        //C:\Users\hasan_xo6cgp9\Desktop\test.txt
        //C:\Users\hasan_xo6cgp9\Desktop\compressedFile
        using FileStream fileStream = new(newPath, FileMode.Create);
        int bytesNeeded = cryptedContext.Count / 8;
        if (cryptedContext.Count % 8 > 0)
        {
            bytesNeeded++;
        }
        byte[] bytes = new byte[bytesNeeded];
        int byteIndex = 0;
        int bitIndex = 0;
        int lastBitIndex = cryptedContext.Count % 8;
        int lastByteIndex = cryptedContext.Count / 8;
        var byteStringValue = string.Empty;
        foreach (bool bit in cryptedContext)
        {
            if (bit)
            {
                byteStringValue += "1";
            }
            else if (!bit)
            {
                byteStringValue += "0";
            }

            bitIndex++;
            if (bitIndex == 8)
            {
                bytes[byteIndex] = Convert.ToByte(byteStringValue, 2);
                byteIndex++;
                bitIndex = 0;
                byteStringValue = string.Empty;
            }
            if (bitIndex == lastBitIndex && byteIndex == lastByteIndex && byteStringValue.Length < 8)
            {
                byteStringValue = byteStringValue.PadLeft(8, '0');
                bytes[byteIndex] = Convert.ToByte(byteStringValue, 2);
            }
        }
        fileStream.Write(bytes, 0, bytesNeeded);
    }

    catch (IOException e)
    {
        Console.Write("The file could not be read: ");
        Console.WriteLine(e.Message);
    }
}
else if (operation.Equals("d"))
{
    try
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        foreach (byte b in fileBytes)
        {
            // Byte'ı binary string'e çevirme
            string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');

            // Ekrana yazdırma
            Console.WriteLine(binaryString);
        }
    }
    catch (IOException e)
    {
        Console.Write("The file could not be read: ");
        Console.WriteLine(e.Message); ;
    }
}