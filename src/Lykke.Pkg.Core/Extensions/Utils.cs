using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Core
{
	public static class Utils
	{
		public static IEnumerable<IEnumerable<T>> ToPieces<T>(this IEnumerable<T> src, int countInPicese)
		{
			var result = new List<T>();

			foreach (var itm in src)
			{
				result.Add(itm);
				if (result.Count >= countInPicese)
				{
					yield return result;
					result = new List<T>();
				}
			}

			if (result.Count > 0)
				yield return result;
		}

		public static byte[] ToBytes(this Stream src)
		{
			var memoryStream = src as MemoryStream;

			if (memoryStream != null)
				return memoryStream.ToArray();


			src.Position = 0;
			var result = new MemoryStream();

			src.CopyTo(result);
			return result.ToArray();
		}

		public static MemoryStream ToStream(this string src)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(src))
			{
				Position = 0
			};
		}

		public static Stream ToStream(this byte[] src)
		{
			if (src == null)
				return null;

			return new MemoryStream(src) { Position = 0 };
		}

		public static T ParseEnum<T>(this string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}


		public static T ParseEnum<T>(this string value, T defaultValue)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value, true);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

	    public static byte[] Encrypt<T>(this T src, string key)
	    {
	        byte[] result;
	        using (var aes = Aes.Create())
	        using (var md5 = MD5.Create())
	        using (var sha256 = SHA256.Create())
	        {
	            aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
	            aes.IV = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

	            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
	            using (var resultStream = new MemoryStream())
	            {
	                using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
	                using (var plainStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(src))))
	                {
	                    plainStream.CopyTo(aesStream);
	                }

	                result = resultStream.ToArray();
	            }
	        }

           
	            return result;
	    }

	    public static T Decrypt<T>(this byte[] message, string key)
	    {
	        byte[] result;
	        using (var aes = Aes.Create())
	        using (var md5 = MD5.Create())
	        using (var sha256 = SHA256.Create())
	        {
	            aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
	            aes.IV = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

	            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
	            using (var resultStream = new MemoryStream())
	            {
	                using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
	                using (var plainStream = new MemoryStream(message))
	                {
	                    plainStream.CopyTo(aesStream);
	                }

	                result = resultStream.ToArray();
	            }
	        }

            
	        return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(result));
	    }
    }
}
