﻿namespace Chiota.Services
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Text;

  using Chiota.Models;

  using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Encrypt.NTRU;
  using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Interfaces;
  using VTDev.Libraries.CEXEngine.CryptoException;

  public class NtruKex
  {
    private const int EncryptedTextSize = 1022;

    // APR2011743FAST
    private readonly NTRUParameters encParams = NTRUParamSets.APR2011743FAST; // Alternative EES743EP1 

    /// <summary>
    /// Creates a NTRU Keypair based on your seed and one address
    /// </summary>
    /// <param name="seed">String of your seed</param>
    /// <param name="saltAddress">String of your address</param>
    /// <returns>Key Pair</returns>
    public IAsymmetricKeyPair CreateAsymmetricKeyPair(string seed, string saltAddress)
    {
      var passphrase = Encoding.UTF8.GetBytes(seed);
      var salt = Encoding.UTF8.GetBytes(saltAddress);

      // Can not be parallel!
      var keyGen = new NTRUKeyGenerator(this.encParams, false);
      var keys = keyGen.GenerateKeyPair(passphrase, salt);

      return keys;
    }

    /// <summary>
    /// Encrypts messages with NTRU
    /// </summary>
    /// <param name="publicKey">public key</param>
    /// <param name="input">input text</param>
    /// <returns>byte array</returns>
    public byte[] Encrypt(IAsymmetricKey publicKey, string input)
    {
      var bytes = new List<byte[]>();
      using (var cipher = new NTRUEncrypt(this.encParams))
      {
        var splitText = this.SplitByLength(input, ChiotaConstants.CharacterLimit);
        foreach (var text in splitText)
        {
          cipher.Initialize(publicKey);
          var data = Encoding.UTF8.GetBytes(text);
          bytes.Add(cipher.Encrypt(data));
        }
      }

      return bytes.SelectMany(a => a).ToArray();
    }

    /// <summary>
    /// Decrypts a byte array
    /// </summary>
    /// <param name="keyPair">The correct key pair</param>
    /// <param name="encryptedText">The encrypted byte array</param>
    /// <returns>Decrypted string</returns>
    public string Decrypt(IAsymmetricKeyPair keyPair, byte[] encryptedText)
    {
      var splitArray = encryptedText.Select((x, i) => new { Key = i / EncryptedTextSize, Value = x })
        .GroupBy(x => x.Key, x => x.Value, (k, g) => g.ToArray())
        .ToArray();
      var decryptedText = string.Empty;

      foreach (var bytes in splitArray)
      {
        using (var cipher = new NTRUEncrypt(this.encParams))
        {
          try
          {
            cipher.Initialize(keyPair);
            var dec = cipher.Decrypt(bytes);
            decryptedText += Encoding.UTF8.GetString(dec);
          }
          catch (CryptoAsymmetricException e)
          {
            Trace.WriteLine(e);
            decryptedText = null;
          }
        }
      }

      return decryptedText;
    }

    private IEnumerable<string> SplitByLength(string str, int maxLength)
    {
      var index = 0;
      while (true)
      {
        if (index + maxLength >= str.Length)
        {
          yield return str.Substring(index);
          yield break;
        }

        yield return str.Substring(index, maxLength);
        index += maxLength;
      }
    }
  }
}
