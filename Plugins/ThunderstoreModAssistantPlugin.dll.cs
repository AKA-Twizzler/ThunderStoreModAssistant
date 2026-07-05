using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using MelonLoader;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using ThunderstoreModAssistant;
using ThunderstoreModAssistantPlugin;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: MelonInfo(typeof(MainClass), "ThunderstoreModAssistantPlugin", "1.0.0", "notnotnotswipez", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
[assembly: MelonPriority(-20000)]
[assembly: AssemblyTitle("ThunderstoreModAssistantPlugin")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ThunderstoreModAssistantPlugin")]
[assembly: AssemblyCopyright("Copyright ©  2023")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("71d6414d-4cfa-410a-8604-271964788e6a")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName = ".NET 6.0")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableAttribute : Attribute
	{
		public readonly byte[] NullableFlags;

		public NullableAttribute(byte P_0)
		{
			NullableFlags = new byte[1] { P_0 };
		}

		public NullableAttribute(byte[] P_0)
		{
			NullableFlags = P_0;
		}
	}
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableContextAttribute : Attribute
	{
		public readonly byte Flag;

		public NullableContextAttribute(byte P_0)
		{
			Flag = P_0;
		}
	}
}
namespace ThunderstoreModAssistant
{
	public class CodeModInfo
	{
		public string name;

		public string fullName;

		public string iconLink;

		public string downloadLink;

		public string author;

		public string version;

		public string description;

		public List<string> dependencies = new List<string>();

		public List<string> affectedFiles = new List<string>();

		public void Uninstall()
		{
			foreach (string affectedFile in affectedFiles)
			{
				try
				{
					File.Delete(affectedFile);
				}
				catch (Exception)
				{
					MelonLogger.Error("Ignoring deletion on file: " + affectedFile);
				}
			}
			File.Delete(Path.Combine(MainClass.MODS_INSTALLED, name + ".json"));
		}
	}
}
namespace ThunderstoreModAssistantPlugin
{
	public class BlacklistedPlatformMods
	{
		public static string PC_BLACKLIST = "https://raw.githubusercontent.com/Minibattle/Thunderstore-Blacklist/refs/heads/main/PC.txt";

		public static string QUEST_BLACKLIST = "https://raw.githubusercontent.com/Minibattle/Thunderstore-Blacklist/refs/heads/main/Quest.txt";

		private static List<string> blacklistedMods = new List<string>();

		private static List<string> blacklistedModsFromFile = new List<string>();

		private static readonly bool isAndroid = (int)MelonUtils.CurrentPlatform == 3;

		public static void LoadFromFile()
		{
			if (CacheBlacklistExists())
			{
				string[] array = File.ReadAllLines(MainClass.BLACKLIST_CACHE);
				foreach (string item in array)
				{
					blacklistedModsFromFile.Add(item);
				}
			}
		}

		public static async void LoadFromURL()
		{
			string targetBlacklist = PC_BLACKLIST;
			if (isAndroid)
			{
				targetBlacklist = QUEST_BLACKLIST;
			}
			string[] lines = (await new HttpClient(new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback = (HttpRequestMessage httpRequestMessage, X509Certificate2? cert, X509Chain? cetChain, SslPolicyErrors policyErrors) => true
			}).GetStringAsync(targetBlacklist)).Split('\n');
			string[] array = lines;
			foreach (string line in array)
			{
				blacklistedMods.Add(line);
			}
			if (CacheBlacklistExists())
			{
				List<string> diff = GetBlacklistDifference();
				if (diff.Count > 0)
				{
					foreach (string name in diff)
					{
						MainClass.TriggerUninstall(name);
					}
				}
			}
			WriteDownloadedBlacklistToFile();
		}

		public static void WriteDownloadedBlacklistToFile()
		{
			if (File.Exists(MainClass.BLACKLIST_CACHE))
			{
				File.Delete(MainClass.BLACKLIST_CACHE);
			}
			string text = "";
			foreach (string blacklistedMod in blacklistedMods)
			{
				text += blacklistedMod;
				text += "\n";
			}
			File.WriteAllText(MainClass.BLACKLIST_CACHE, text);
		}

		public static List<string> GetBlacklistDifference()
		{
			List<string> list = new List<string>(blacklistedMods);
			foreach (string item in blacklistedModsFromFile)
			{
				list.Remove(item);
			}
			return list;
		}

		public static bool CacheBlacklistExists()
		{
			return File.Exists(MainClass.BLACKLIST_CACHE);
		}
	}
	public class MainClass : MelonPlugin
	{
		public static string MOD_ASSISTANT = Path.Combine(MelonUtils.GameDirectory, "TSModAssistant");

		public static string MOD_DOWNLOADS_QUEUED = Path.Combine(MOD_ASSISTANT, "ModsQueued");

		public static string MODS_INSTALLED = Path.Combine(MOD_ASSISTANT, "ModsInstalled");

		public static string INSTRUCTIONS = Path.Combine(MOD_ASSISTANT, "bootStruct.txt");

		public static string BLACKLIST_CACHE = Path.Combine(MOD_ASSISTANT, "blacklist_cache.txt");

		public override void OnPreInitialization()
		{
			if (!Directory.Exists(MODS_INSTALLED))
			{
				Directory.CreateDirectory(MODS_INSTALLED);
			}
			if (File.Exists(INSTRUCTIONS))
			{
				ReadInstructions();
			}
			BlacklistedPlatformMods.LoadFromFile();
			BlacklistedPlatformMods.LoadFromURL();
		}

		private void ReadInstructions()
		{
			string[] array = File.ReadAllLines(INSTRUCTIONS);
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.StartsWith("install") && text != "")
				{
					InstallQueuedMod(text.Replace("install ", ""));
				}
				if (text.StartsWith("uninstall") && text != "")
				{
					string name = text.Replace("uninstall ", "");
					TriggerUninstall(name);
				}
			}
			File.Delete(INSTRUCTIONS);
		}

		public static void TriggerUninstall(string name)
		{
			if (File.Exists(Path.Combine(MODS_INSTALLED, name + ".json")))
			{
				string text = File.ReadAllText(Path.Combine(MODS_INSTALLED, name + ".json"));
				CodeModInfo codeModInfo = JsonConvert.DeserializeObject<CodeModInfo>(text);
				codeModInfo.Uninstall();
			}
		}

		private void InstallQueuedMod(string name)
		{
			string text = Path.Combine(MOD_DOWNLOADS_QUEUED, name);
			MelonLogger.Msg("Installing " + text);
			if (!Directory.Exists(text))
			{
				MelonLogger.Msg("Mod not found in filesystem: " + name);
				return;
			}
			string text2 = File.ReadAllText(Path.Combine(text, "codeModInfo.json"));
			CodeModInfo codeModInfo = JsonConvert.DeserializeObject<CodeModInfo>(text2);
			codeModInfo.affectedFiles = new List<string>();
			RecursiveCodeModMove(0, text, text, MelonUtils.GameDirectory, codeModInfo);
			string path = Path.Combine(MODS_INSTALLED, name + ".json");
			string contents = JsonConvert.SerializeObject((object)codeModInfo);
			File.WriteAllText(path, contents);
			MelonLogger.Msg("Finished installing " + codeModInfo.name);
			Directory.Delete(text, recursive: true);
		}

		private void RecursiveCodeModMove(int level, string originalPath, string currentPath, string destinationPath, CodeModInfo info)
		{
			string[] directories = Directory.GetDirectories(currentPath);
			foreach (string text in directories)
			{
				if (!File.Exists(text))
				{
					RecursiveCodeModMove(1, originalPath, text, destinationPath, info);
				}
			}
			string[] files = Directory.GetFiles(currentPath);
			foreach (string text2 in files)
			{
				if (level <= 0)
				{
					continue;
				}
				string text3 = text2.Replace(originalPath, "");
				string text4 = destinationPath + text3;
				string directoryName = Path.GetDirectoryName(text4);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				if (File.Exists(text4))
				{
					try
					{
						File.Delete(text4);
					}
					catch (Exception)
					{
						MelonLogger.Error("Ignoring deletion on file: " + text4);
					}
				}
				try
				{
					if (File.Exists(text2))
					{
						MelonLogger.Msg("Affected file: " + text4);
						info.affectedFiles.Add(text4);
						File.Move(text2, text4);
					}
				}
				catch (Exception)
				{
				}
			}
		}
	}
}