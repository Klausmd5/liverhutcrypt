using System.Security.Cryptography;
using System.Text;

namespace Encryptor;

public class EncryptionService
{
    private const string fileExt = ".liverhut";
    
    private static byte[] GenerateSalt()
    {
        byte[] data = new byte[32];
        using (RNGCryptoServiceProvider rgnCryptoServiceProvider = new RNGCryptoServiceProvider())
        {
            rgnCryptoServiceProvider.GetBytes(data);
        }
        return data;
    }
    
    public void FileEncrypt(string inputFile, string password)
    {
        byte[] salt = GenerateSalt();
        byte[] passwords = Encoding.UTF8.GetBytes(password);
        RijndaelManaged AES = new RijndaelManaged();
        AES.KeySize = 256;//aes 256 bit encryption c#
        AES.BlockSize = 128;//aes 128 bit encryption c#
        AES.Padding = PaddingMode.PKCS7;
        var key = new Rfc2898DeriveBytes(passwords, salt, 50000);
        AES.Key = key.GetBytes(AES.KeySize / 8);
        AES.IV = key.GetBytes(AES.BlockSize / 8);
        AES.Mode = CipherMode.CFB;
        using (FileStream fsCrypt = new FileStream(inputFile + fileExt, FileMode.Create))
        {
            fsCrypt.Write(salt, 0, salt.Length);
            using (CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write))
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
        File.Delete(inputFile);
    }
    
    public void FileDecrypt(string outputFileName, string password)
    {
        byte[] passwords = Encoding.UTF8.GetBytes(password);
        byte[] salt = new byte[32];
        using (FileStream fsCrypt = new FileStream(outputFileName, FileMode.Open))
        {
            fsCrypt.Read(salt, 0, salt.Length);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;//aes 256 bit encryption c#
            AES.BlockSize = 128;//aes 128 bit encryption c#
            var key = new Rfc2898DeriveBytes(passwords, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;
            using (CryptoStream cryptoStream = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read))
            {
                var name = outputFileName.Substring(0, outputFileName.Length - fileExt.Length);
                using (FileStream fsOut = new FileStream(name, FileMode.Create))
                {
                    int read;
                    byte[] buffer = new byte[1048576];
                    while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fsOut.Write(buffer, 0, read);
                        Console.WriteLine("Decrypting file... {0}", name);
                    }
                }
            }
        }
        File.Delete(outputFileName);
    }
    
}