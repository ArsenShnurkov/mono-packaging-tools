using System.IO;
using System.Text;

public class PublicKeyUtils
{
	public static byte[] GetPublicKey(byte[] snk)
	{
		var snkp = new System.Reflection.StrongNameKeyPair(snk);
		byte[] publicKey = snkp.PublicKey;
		return publicKey;
	}

	public static byte[] GetPublicKey(string container)
	{
		var snkp = new System.Reflection.StrongNameKeyPair(container);
		byte[] publicKey = snkp.PublicKey;
		return publicKey;
	}

	public static string ByteArrayToString(byte[] ba)
	{
		StringBuilder hex = new StringBuilder(ba.Length * 2);
		foreach (byte b in ba)
			hex.AppendFormat("{0:x2}", b);
		return hex.ToString();
	}

	public static string GetPublicKeyStringFromContainer(string AssemblyKeyContainerName)
	{
		byte[] publicKey2 = GetPublicKey(AssemblyKeyContainerName);
		string output2 = ByteArrayToString(publicKey2);
		return output2;
	}

	public static string GetPublicKeyStringFromFilename(string AssemblyOriginatorKeyFile)
	{
		byte[] snk = File.ReadAllBytes(AssemblyOriginatorKeyFile);
		byte[] publicKey1 = GetPublicKey(snk);
		string output1 = ByteArrayToString(publicKey1);
		return output1;
	}
}
