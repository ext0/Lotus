using log4net;
using LotusRoot.CComm.CData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LotusRoot.LComm.Data
{
    public class LCipher
    {
        private static readonly int RSA_KEY_SIZE = 4096;
        private static readonly int RSA_BUFFER_SIZE = 446;
        private static readonly int AES_KEY_SIZE = 16 * 8;

        private static readonly CspParameters _cspParams = new CspParameters { ProviderType = 1 };
        private RijndaelManaged _localAesProvider;
        private RijndaelManaged _remoteAesProvider;
        private RSACryptoServiceProvider _rsaProvider;
        private byte[] _privateKey;
        private LPublicKey _publicKey;
        private bool _canDecrypt;
        private bool _remoteAESloaded;

        public LCipher()
        {
            _localAesProvider = new RijndaelManaged();
            _localAesProvider.GenerateIV();
            _localAesProvider.KeySize = AES_KEY_SIZE;
            _localAesProvider.GenerateKey();
            _localAesProvider.Mode = CipherMode.CBC;
            _localAesProvider.Padding = PaddingMode.PKCS7;

            _rsaProvider = new RSACryptoServiceProvider(RSA_KEY_SIZE, _cspParams);

            String publicXml = _rsaProvider.ToXmlString(false);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(publicXml);

            String modulus = xml.GetElementsByTagName("Modulus")[0].InnerText;
            String exponent = xml.GetElementsByTagName("Exponent")[0].InnerText;

            _publicKey = new LPublicKey(modulus, exponent);
            _privateKey = Encoding.ASCII.GetBytes(_rsaProvider.ToXmlString(true));

            _canDecrypt = true;
        }

        public LCipher(LPublicKey key)
        {
            _localAesProvider = new RijndaelManaged();
            _localAesProvider.GenerateIV();
            _localAesProvider.KeySize = AES_KEY_SIZE;
            _localAesProvider.GenerateKey();
            _localAesProvider.Mode = CipherMode.CBC;
            _localAesProvider.Padding = PaddingMode.PKCS7;

            _rsaProvider = new RSACryptoServiceProvider(RSA_KEY_SIZE, _cspParams);

            RSAParameters RSAKeyInfo = new RSAParameters();

            RSAKeyInfo.Modulus = Convert.FromBase64String(key.Modulus);
            RSAKeyInfo.Exponent = Convert.FromBase64String(key.Exponent);

            _rsaProvider.ImportParameters(RSAKeyInfo);

            _publicKey = key;

            _canDecrypt = false;
        }

        public LPublicKey PublicKey
        {
            get
            {
                return _publicKey;
            }
        }

        public LAESInfo LocalAESInfo
        {
            get
            {
                return new LAESInfo(Convert.ToBase64String(_localAesProvider.IV), Convert.ToBase64String(_localAesProvider.Key));
            }
        }

        public byte[] LocalAESEncrypt(byte[] data)
        {
            return CryptoStreamProcess(data, _localAesProvider.CreateEncryptor(_localAesProvider.Key, _localAesProvider.IV));
        }

        public byte[] LocalAESDecrypt(byte[] data)
        {
            return CryptoStreamProcess(data, _localAesProvider.CreateDecryptor(_localAesProvider.Key, _localAesProvider.IV));
        }

        public byte[] RemoteAESEncrypt(byte[] data)
        {
            if (!_remoteAESloaded)
            {
                throw new Exception("Remote AES configuration has not been initialized! Did the handshake succeed?");
            }
            return CryptoStreamProcess(data, _remoteAesProvider.CreateEncryptor(_remoteAesProvider.Key, _remoteAesProvider.IV));
        }

        public byte[] RemoteAESDecrypt(byte[] data)
        {
            if (!_remoteAESloaded)
            {
                throw new Exception("Remote AES configuration has not been initialized! Did the handshake succeed?");
            }
            return CryptoStreamProcess(data, _remoteAesProvider.CreateDecryptor(_remoteAesProvider.Key, _remoteAesProvider.IV));
        }

        private byte[] CryptoStreamProcess(byte[] data, ICryptoTransform transform)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }

        }
        public void LoadRemoteAES(LAESInfo info)
        {
            if (_remoteAESloaded)
            {
                throw new Exception("Remote AES configuration already initialized!");
            }
            _remoteAesProvider = new RijndaelManaged();
            _remoteAesProvider.IV = Convert.FromBase64String(info.IV);
            _remoteAesProvider.Key = Convert.FromBase64String(info.Key);
            _remoteAesProvider.Mode = CipherMode.CBC;
            _remoteAesProvider.Padding = PaddingMode.PKCS7;
            _remoteAESloaded = true;
        }

        public byte[] PDecrypt(byte[] data)
        {
            if (!_canDecrypt)
            {
                throw new Exception("Cannot decrypt on an LCipher initialized with public parameters!");
            }
            byte[] block;
            using (MemoryStream stream = new MemoryStream())
            {
                int read = 0;
                while (read < data.Length)
                {
                    block = new byte[RSA_KEY_SIZE / 8];
                    int toRead = Math.Min(data.Length - read, block.Length);
                    Buffer.BlockCopy(data, read, block, 0, toRead);
                    read += toRead;
                    byte[] decrypted = _rsaProvider.Decrypt(block, false);
                    stream.Write(decrypted, 0, decrypted.Length);
                }
                return stream.ToArray();
            }
        }

        public byte[] PEncrypt(byte[] data)
        {
            byte[] block;
            using (MemoryStream stream = new MemoryStream())
            {
                int read = 0;
                while (read < data.Length)
                {
                    block = new byte[RSA_BUFFER_SIZE];
                    int toRead = Math.Min(data.Length - read, block.Length);
                    Buffer.BlockCopy(data, read, block, 0, toRead);
                    read += toRead;
                    byte[] decrypted = _rsaProvider.Encrypt(block, false);
                    stream.Write(decrypted, 0, decrypted.Length);
                }
                return stream.ToArray();
            }
        }
    }
}
