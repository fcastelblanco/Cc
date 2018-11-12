using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cc.Common.Isolucion
{
    public class Encryptor
    {   
        private string _oldKey;
        private string _oldVector;
        
        public Cripto Decrypt(string txtTextoEncrypado, string txtLlave, string txtVectorIni)
        {
            var theCripto = new Cripto
            {
                LlaveEncripcion = txtLlave,
                ViEncripcion = txtVectorIni,
                CadenaEncriptada = txtTextoEncrypado
            };

            SymmetricAlgorithm symmetricAlgorithm = new RijndaelManaged()
            {
                Key = Convert.FromBase64String(theCripto.LlaveEncripcion),
                IV = Convert.FromBase64String(theCripto.ViEncripcion)
            };

            var encryptionMemoryStream = new MemoryStream();
            var initializationVector = new byte[symmetricAlgorithm.IV.Length];

            try

            {
                var totalBytes = Convert.FromBase64String(txtTextoEncrypado);
                Array.Copy(totalBytes, initializationVector, symmetricAlgorithm.IV.Length);
                var decryptedBytes = new byte[totalBytes.Length - symmetricAlgorithm.IV.Length];
                Array.Copy(totalBytes, symmetricAlgorithm.IV.Length, decryptedBytes, 0,
                    totalBytes.Length - symmetricAlgorithm.IV.Length);

                symmetricAlgorithm.Mode = CipherMode.CBC;
                symmetricAlgorithm.IV = initializationVector;

                var cryptographicTransform = symmetricAlgorithm.CreateDecryptor(symmetricAlgorithm.Key,
                    symmetricAlgorithm.IV);

                using (
                    var encryptionCryptoStream = new CryptoStream(encryptionMemoryStream, cryptographicTransform,
                        CryptoStreamMode.Write))
                {
                    encryptionCryptoStream.Write(decryptedBytes, 0, decryptedBytes.Length);
                    encryptionCryptoStream.FlushFinalBlock();
                }

                theCripto.CadenaDesencriptada = Encoding.Unicode.GetString(encryptionMemoryStream.ToArray());
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException("Archivo inválido para desencriptar.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Archivo inválido.", e);
            }
            finally
            {
                encryptionMemoryStream.Close();
            }
            return theCripto;
        }
        
        public Cripto Encrypt(string pCadena)
        {
            var theCripto = new Cripto();
            SymmetricAlgorithm symmetricAlgorithm = new RijndaelManaged();
            symmetricAlgorithm.GenerateKey();
            symmetricAlgorithm.GenerateIV();

            theCripto.CadenaDesencriptada = pCadena;

            theCripto.ViEncripcion = Convert.ToBase64String(symmetricAlgorithm.IV);
            theCripto.LlaveEncripcion = Convert.ToBase64String(symmetricAlgorithm.Key);
            symmetricAlgorithm.Mode = CipherMode.CBC;

            var cryptoTransform = symmetricAlgorithm.CreateEncryptor(symmetricAlgorithm.Key, symmetricAlgorithm.IV);

            var plainBytes = Encoding.Unicode.GetBytes(pCadena);
            var encryptionMemoryStream = new MemoryStream();
            using (var encryptionCryptoStream = new
                CryptoStream(encryptionMemoryStream, cryptoTransform,
                    CryptoStreamMode.Write))
            {
                encryptionCryptoStream.Write(plainBytes, 0, plainBytes.Length);
                encryptionCryptoStream.FlushFinalBlock();
            }

            var cypherBytes = encryptionMemoryStream.ToArray();

            var totalSize = cypherBytes.Length + symmetricAlgorithm.IV.Length;

            var completedBytes = new byte[totalSize];

            Array.Copy(symmetricAlgorithm.IV, 0, completedBytes, 0,
                symmetricAlgorithm.IV.Length);

            Array.Copy(cypherBytes, 0, completedBytes, symmetricAlgorithm.IV.Length,
                cypherBytes.Length);

            theCripto.CadenaEncriptada = Convert.ToBase64String(completedBytes);

            if (_oldKey != null || _oldVector != null) return theCripto;

            _oldKey = theCripto.LlaveEncripcion;
            _oldVector = theCripto.ViEncripcion;
            return theCripto;
        }
    }
}