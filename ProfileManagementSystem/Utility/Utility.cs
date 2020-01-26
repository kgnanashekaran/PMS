using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProfileManagementSystem.Utility
{
    public static class Utility
    {      
        
      
        #region Password Encryption
        public static string Encrypt(string clearText)
        {
            try
            {
                string EncryptionKey = "1234hsghsgdhsgdhsd";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Password Decryption
        public static string Decrypt(string cipherText)
        {
            try
            {
                string EncryptionKey = "1234hsghsgdhsgdhsd";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch(Exception ex)
            {
                return "";
            }

        }
        #endregion
        public static void StoreError(string method,string errorMessage)
        {
            string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
            // SQLiteConnection conn = new SQLiteConnection(connectString);
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectString))
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.CommandText = @"insert into errorLog(method,date,errorMessage) values ('" + method + "','" + DateTime.Now.ToString() + "','" + errorMessage + "')";
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    //conn.Close();
                }
            }
            catch(Exception ex)
            {

            }

        }

        //public static string PasswordDecryption(string ConString)
        //{

        //    try

        //    {

        //        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConString);

        //        builder.Password = Utility.Decrypt(builder.Password);

        //        return builder.ConnectionString;

        //    }

        //    catch (Exception ex)

        //    {

        //        return ex.ToString();

        //    }

        //}

    }
}
