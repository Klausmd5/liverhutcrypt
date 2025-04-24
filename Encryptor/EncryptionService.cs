using System.Security.Cryptography;
using System.Text;

namespace Encryptor;

public class EncryptionService
{
    private const string FileExt = ".liverhut";
    private readonly List<string> _filesToDelete = new();

    private static byte[] GenerateSalt()
    {
        byte[] data = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);
        }
        return data;
    }

    public void DeleteFiles()
    {
        _filesToDelete.ForEach(x=> File.Delete(x));
        _filesToDelete.Clear();
    }
    
    public void FileEncrypt(string inputFile, string password)
    {
        try
        {
            byte[] salt = GenerateSalt();
            byte[] passwords = Encoding.UTF8.GetBytes(password);
            Aes aes = Aes.Create();
            aes.KeySize = 256; //aes 256 bit encryption c#
            aes.BlockSize = 128; //aes 128 bit encryption c#
            aes.Padding = PaddingMode.PKCS7;
            var key = new Rfc2898DeriveBytes(passwords, salt, 50000, HashAlgorithmName.SHA3_512);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CFB;
            using (FileStream fsCrypt = new FileStream(inputFile + FileExt, FileMode.Create, FileAccess.ReadWrite))
            {
                fsCrypt.Write(salt, 0, salt.Length);
                using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        byte[] buffer = new byte[1048576];
                        int read;
                        while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cs.Write(buffer, 0, read);
                        }
                    }
                }
            }

            Console.WriteLine("Encrypting file... {0}", inputFile);
            File.SetAttributes(inputFile + FileExt, FileAttributes.Encrypted);
            _filesToDelete.Add(inputFile);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message);
        }
    }

    public void FileDecrypt(string outputFileName, string password)
    {
        try
        {
            byte[] passwords = Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];
        
            var name = outputFileName.Substring(0, outputFileName.Length - FileExt.Length);
            using (FileStream fsCrypt = new FileStream(outputFileName, FileMode.Open))
            {
                fsCrypt.ReadExactly(salt, 0, salt.Length);
                Aes aes = Aes.Create();
                aes.KeySize = 256;//aes 256 bit encryption c#
                aes.BlockSize = 128;//aes 128 bit encryption c#
                var key = new Rfc2898DeriveBytes(passwords, salt, 50000, HashAlgorithmName.SHA3_512);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CFB;
            
                using (CryptoStream cryptoStream = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (FileStream fsOut = new FileStream(name, FileMode.Create))
                    {
                        int read;
                        byte[] buffer = new byte[1048576];
                        while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOut.Write(buffer, 0, read);
                        }
                    }
                }
            }
            Console.WriteLine("Decrypting file... {0}", name);
            _filesToDelete.Add(outputFileName);
        } catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message);
        }
    }
    
}