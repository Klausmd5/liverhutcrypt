using Encryptor;

var enc = new EncryptionService();

while (true)
{
    var input = Console.ReadLine();
    if (input == "e")
    {
        Console.WriteLine("Encrypting file...");
        var files = Directory.GetFiles("encrypt").ToList();
        files.ForEach(f => enc.FileEncrypt(f, "test"));

    }

    if (input == "d")
    {
        Console.WriteLine("Decrypting file...");
        var files = Directory.GetFiles("encrypt").ToList();
        files.ForEach(f => enc.FileDecrypt(f, "test"));
    }
}



