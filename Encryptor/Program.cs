using Encryptor;

var enc = new EncryptionService();

var password = "orderMeALiverhut";

while (true)
{
    Console.Write("Enter directory to encrypt/decrypt: ");
    var dir = Console.ReadLine();
    Console.Write("Enter 'e' to encrypt or 'd' to decrypt: ");
    var input = Console.ReadLine();
    if (input == "e")
    {
        GetPassword();
        Console.WriteLine("Encrypting file...");
        var files = Directory.GetFiles(dir,"*.*", SearchOption.AllDirectories).ToList();
        files.ForEach(f => enc.FileEncrypt(f, password));
        Console.WriteLine("Files Encrypted");
    }

    if (input == "d")
    {
        GetPassword();
        Console.WriteLine("Decrypting file...");
        var files = Directory.GetFiles(dir,"*.*", SearchOption.AllDirectories).ToList();
        files.ForEach(f => enc.FileDecrypt(f, password));
        Console.WriteLine("Files Decrypted");
    }
    
    enc.DeleteFiles();
}

void GetPassword() {
    Console.Write("Enter Password: ");
    var pw = Console.ReadLine();
    if (!string.IsNullOrEmpty(pw))
    {
        password = pw;
    }
}

