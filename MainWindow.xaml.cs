using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Security.Cryptography;

namespace AES_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        byte[] enc;
        byte[] i;
        byte[] k;


        public MainWindow()
        {
            InitializeComponent();
        }

        static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }

        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }


        private void btEncrypt_Click(object sender, RoutedEventArgs e)
        {
            string raw = tbTextToEncrypt.Text;
            //byte[] key = Encoding.UTF8.GetBytes(tbKeyPrefix.Text + DateTime.Now.ToString("dd.MM.yyyy.HH"));
            //date separator ")"
            string d = DateTime.Now.Day.ToString();
            string m = DateTime.Now.Month.ToString();
//            string h = DateTime.Now.Hour.ToString();
            d = (d.Length == 1 ? "0" + d : d);
            m = (m.Length == 1 ? "0" + m : m);
//            h = (h.Length == 1 ? "0" + h : h);
            byte[] key = Encoding.UTF8.GetBytes(tbKeyPrefix.Text + d + Convert.ToChar(41) + m + Convert.ToChar(41) + DateTime.Now.Year + Convert.ToChar(41) /*+ h*/);

            try
            {
                // Create Aes that generates a new key and initialization vector (IV).    
                // Same key must be used in encryption and decryption    
                using (AesManaged aes = new AesManaged())
                {
                    // Encrypt string    
                    byte[] encrypted = Encrypt(raw, key, aes.IV);
                    byte[] bytesToWrite = aes.IV.Concat(encrypted).ToArray();
                    File.WriteAllBytes(tbFileName.Text, bytesToWrite);

                    //----для теста
                    enc = encrypted;
                    i = aes.IV;
                    k = key;
                    //----
                }

                MessageBox.Show("Ready");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }

        }

        private void btDecrypt_Click(object sender, RoutedEventArgs e)
        {
            
            byte[] readBytes = File.ReadAllBytes(tbFileName.Text);

            string d = DateTime.Now.Day.ToString();

            d = "25";

            string m = DateTime.Now.Month.ToString();
//            string h = DateTime.Now.Hour.ToString();
            d = (d.Length == 1 ? "0" + d : d);
            m = (m.Length == 1 ? "0" + m : m);
//            h = (h.Length == 1 ? "0" + h : h);
            byte[] key = Encoding.UTF8.GetBytes(tbKeyPrefix.Text + d + Convert.ToChar(41) + m + Convert.ToChar(41) + DateTime.Now.Year + Convert.ToChar(41)/* + h*/);
            
            byte[] iv = new byte[16];
            byte[] enctrypted = new byte[readBytes.Length - iv.Length];
            Array.Copy(readBytes, iv, 16);
            Array.Copy(readBytes, iv.Length, enctrypted, 0, enctrypted.Length);

            string raw = Decrypt(enctrypted, key, iv);
            
            //string raw = Decrypt(enc, k, i);

            MessageBox.Show(raw);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            char x = ')';
            int a = (int)x;
        }
    }
}
