using Encryptor;

var enc = new EncryptionService();

Console.WriteLine("Enter directory to encrypt/decrypt");
var dir = Console.ReadLine();
while (true)
{
    Console.WriteLine("Enter 'e' to encrypt or 'd' to decrypt");
    var input = Console.ReadLine();
    if (input == "e")
    {
        Console.WriteLine("Encrypting file...");
        var files = Directory.GetFiles(dir,"*.*", SearchOption.AllDirectories).ToList();
        files.ForEach(f => enc.FileEncrypt(f, "test"));
    }

    if (input == "d")
    {
        Console.WriteLine("Decrypting file...");
        var files = Directory.GetFiles(dir,"*.*", SearchOption.AllDirectories).ToList();
        files.ForEach(f => enc.FileDecrypt(f, "test"));
    }
    
    enc.DeleteFiles();
}



