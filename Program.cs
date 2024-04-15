
using GZip.Business;
using System.Collections;

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
        var huffmanResult = huffman.EncryptContext();

        var cryptedContext = huffmanResult.BitArray;
        var huffmanCodes = huffmanResult.Codes;

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

        int lastIndex = filePath.LastIndexOf('\\');
        string basePath = filePath.Substring(0, lastIndex + 1);
        string newPath = basePath + "compressedFile";
        //C:\Users\s22987\Desktop\test.txt
        //C:\Users\s22987\Desktop\compressedFile
        using FileStream fileStream = new(newPath, FileMode.Create);

        using BinaryWriter binaryWriter = new(fileStream);

        string serializedHuffmanCodes = huffman.SerializeHuffmanCodes(huffmanCodes);
        byte[] huffmanCodeBytes = System.Text.Encoding.UTF8.GetBytes(serializedHuffmanCodes);

        binaryWriter.Write(huffmanCodeBytes.Length);
        binaryWriter.Write(huffmanCodeBytes);

        binaryWriter.Write(cryptedContext.Count);

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
            byteStringValue += bit ? "1" : "0";

            bitIndex++;
            if (bitIndex == 8)
            {
                bytes[byteIndex] = Convert.ToByte(byteStringValue, 2);
                byteIndex++;
                bitIndex = 0;
                byteStringValue = string.Empty;
            }
        }
        if (byteStringValue.Length > 0)
        {
            byteStringValue = byteStringValue.PadLeft(8, '0');
            bytes[byteIndex] = Convert.ToByte(byteStringValue, 2);
        }
        fileStream.Write(bytes, 0, bytesNeeded);
        Console.WriteLine("File size after compression: " + bytes.Length + " byte");
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
        HuffmanEncryption huffman = new();
        LZ77Encryption lz77 = new();

        using FileStream fileStream = new FileStream(filePath, FileMode.Open);
        using BinaryReader binaryReader = new BinaryReader(fileStream);

        int huffmanCodeLength = binaryReader.ReadInt32();
        byte[] huffmanCodeBytes = binaryReader.ReadBytes(huffmanCodeLength);
        string serializedHuffmanCodes = System.Text.Encoding.UTF8.GetString(huffmanCodeBytes);
        var huffmanCodes = huffman.DeserializeHuffmanCodes(serializedHuffmanCodes);

        int actualBitLength = binaryReader.ReadInt32();
        byte[] fileBytes = binaryReader.ReadBytes((actualBitLength + 7) / 8);

        //int bitCount = 0;
        //string convertedText = string.Empty;
        //foreach (byte b in fileBytes)
        //{
        //    string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');
        //    if (bitCount + 8 > actualBitLength)
        //    {
        //        binaryString = binaryString.Substring(8 - (actualBitLength - bitCount));
        //    }
        //    convertedText += binaryString;
        //    bitCount += 8;
        //}

        List<bool> bitArray = new List<bool>();
        for (int i = 0; i < fileBytes.Length; i++)
        {
            for (int bit = 0; bit < 8; bit++)
            {
                if (i * 8 + bit >= actualBitLength) break; // Do not read past the actual number of bits
                bool bitValue = (fileBytes[i] & (1 << (7 - bit))) != 0;
                bitArray.Add(bitValue);
            }
        }

        //string decodedText = huffman.Decode(bitArray, huffmanCodes);

        //Console.WriteLine("Original Text: " + decodedText);
    }
    catch (IOException e)
    {
        Console.Write("The file could not be read: ");
        Console.WriteLine(e.Message);
    }
}