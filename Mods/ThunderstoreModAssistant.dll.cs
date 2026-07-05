using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BoneLib;
using BoneLib.BoneMenu;
using BoneLib.BoneMenu.UI;
using BoneLib.Notifications;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using ThunderstoreModAssistant;
using ThunderstoreModAssistant.Behaviors;
using ThunderstoreModAssistant.Blacklist;
using ThunderstoreModAssistant.Install;
using ThunderstoreModAssistant.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: MelonInfo(typeof(MainClass), "ThunderstoreModAssistant", "1.2.1", "notnotnotswipez", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
[assembly: AssemblyTitle("ThunderstoreModAssistant")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ThunderstoreModAssistant")]
[assembly: AssemblyCopyright("Copyright ©  2023")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("313a216b-9d0e-436b-8275-42ca67068367")]
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
	public class AssistantAssets
	{
		public static GameObject mainMenu;

		public static GameObject modInformation;

		public static GameObject updateEntry;

		public static void LoadAssets(AssetBundle bundle)
		{
			mainMenu = bundle.LoadPersistentAsset<GameObject>("assets/thunderstoreassistant/rootmenu.prefab");
			modInformation = bundle.LoadPersistentAsset<GameObject>("assets/thunderstoreassistant/modinformation.prefab");
			updateEntry = bundle.LoadPersistentAsset<GameObject>("assets/thunderstoreassistant/updateentry.prefab");
		}
	}
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

		public bool IsInstalled()
		{
			CodeModInfo codeModInfo = null;
			foreach (CodeModInfo pendingUninstallCodeMod in MainClass.pendingUninstallCodeMods)
			{
				if (name == pendingUninstallCodeMod.name)
				{
					return false;
				}
			}
			foreach (CodeModInfo installedCodeMod in MainClass.installedCodeMods)
			{
				if (name == installedCodeMod.name)
				{
					codeModInfo = installedCodeMod;
					break;
				}
			}
			foreach (CodeModInfo pendingCodeMod in MainClass.pendingCodeMods)
			{
				if (name == pendingCodeMod.name)
				{
					codeModInfo = pendingCodeMod;
					break;
				}
			}
			if (codeModInfo == null)
			{
				return false;
			}
			return true;
		}

		public bool IsPending()
		{
			foreach (CodeModInfo pendingCodeMod in MainClass.pendingCodeMods)
			{
				if (name == pendingCodeMod.name)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasUpdate()
		{
			CodeModInfo codeModInfo = null;
			foreach (CodeModInfo value in ThunderstoreParser.codeMods.Values)
			{
				if (name == value.name)
				{
					codeModInfo = value;
					break;
				}
			}
			if (codeModInfo == null)
			{
				return false;
			}
			if (codeModInfo.version != version)
			{
				return true;
			}
			return false;
		}
	}
	public static class ModInfo
	{
		public const string MOD_VERSION = "1.2.1";
	}
	public class MainClass : MelonMod
	{
		public static string MOD_ASSISTANT = Path.Combine(MelonUtils.GameDirectory, "TSModAssistant");

		public static string MOD_DOWNLOADS_QUEUED = Path.Combine(MOD_ASSISTANT, "ModsQueued");

		public static string MODS_INSTALLED = Path.Combine(MOD_ASSISTANT, "ModsInstalled");

		public static string INSTRUCTIONS = Path.Combine(MOD_ASSISTANT, "bootStruct.txt");

		public static Page mainCategory;

		public static List<CodeModInfo> installedCodeMods = new List<CodeModInfo>();

		public static List<CodeModInfo> pendingUninstallCodeMods = new List<CodeModInfo>();

		public static List<CodeModInfo> pendingCodeMods = new List<CodeModInfo>();

		public override void OnInitializeMelon()
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (!Directory.Exists(MOD_ASSISTANT))
			{
				Directory.CreateDirectory(MOD_ASSISTANT);
			}
			if (!Directory.Exists(MOD_DOWNLOADS_QUEUED))
			{
				Directory.CreateDirectory(MOD_DOWNLOADS_QUEUED);
			}
			if (!Directory.Exists(MODS_INSTALLED))
			{
				Directory.CreateDirectory(MODS_INSTALLED);
			}
			BlacklistedPlatformMods.LoadFromURL();
			mainCategory = Page.Root.CreatePage("Thunderstore Mod Assistant", Color.cyan, 0, true);
			ThunderstoreParser.ParseThunderstore();
			AssetBundle val = null;
			val = (HelperMethods.IsAndroid() ? BundleUtilities.LoadBundleFromInternalAssembly("tsassistant.quest.assets") : BundleUtilities.LoadBundleFromInternalAssembly("tsassistant.assets"));
			AssistantAssets.LoadAssets(val);
			ParseInstalled(MODS_INSTALLED);
			Hooking.OnLevelLoaded += delegate
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Expected O, but got Unknown
				if (AssistantMenuController.GetModsToUpdateCount() > 0)
				{
					Notifier.Send(new Notification
					{
						Title = NotificationText.op_Implicit("ThunderstoreModAssistant"),
						Message = NotificationText.op_Implicit("Code mod updates available!"),
						PopupLength = 2f,
						Type = (NotificationType)0,
						ShowTitleOnPopup = true
					});
				}
			};
		}

		public void AddManualInstalled()
		{
			CodeModInfo codeModInfo = new CodeModInfo
			{
				name = "BoneLib",
				version = "3.0.0",
				iconLink = "https://gcdn.thunderstore.io/live/repository/icons/gnonme-BoneLib-3.0.0.png"
			};
			if (!codeModInfo.IsInstalled())
			{
				installedCodeMods.Add(codeModInfo);
			}
			CodeModInfo codeModInfo2 = new CodeModInfo
			{
				name = "ThunderstoreModAssistant",
				version = "1.2.1",
				iconLink = "https://gcdn.thunderstore.io/live/repository/icons/notnotnotswipez-ThunderstoreModAssistant-1.0.0.png"
			};
			if (!codeModInfo2.IsInstalled())
			{
				installedCodeMods.Add(codeModInfo2);
			}
		}

		public void ParseInstalled(string folder)
		{
			string[] files = Directory.GetFiles(folder);
			foreach (string path in files)
			{
				string text = File.ReadAllText(path);
				CodeModInfo item = JsonConvert.DeserializeObject<CodeModInfo>(text);
				installedCodeMods.Add(item);
			}
			AddManualInstalled();
		}

		public override void OnUpdate()
		{
			ThumbnailThreader.HandleQueue();
			InstallManager.HandleQueue();
			MainThreadManager.HandleQueue();
			TimerManager.Update();
		}
	}
	public class ThunderstoreParser
	{
		public static Dictionary<string, CodeModInfo> codeMods = new Dictionary<string, CodeModInfo>();

		private static string THUNDERSTORE_BONELAB = "https://thunderstore.io/c/bonelab/api/v1/package/";

		public static void ParseThunderstore()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Invalid comparison between Unknown and I4
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Invalid comparison between Unknown and I4
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected O, but got Unknown
			codeMods.Clear();
			UnityWebRequest val = UnityWebRequest.Get(THUNDERSTORE_BONELAB);
			val.SendWebRequest();
			while (!val.isDone)
			{
			}
			if ((int)val.result == 2 || (int)val.result == 3)
			{
				return;
			}
			string text = val.downloadHandler.text;
			dynamic val2 = JsonConvert.DeserializeObject<object>(text, new JsonSerializerSettings
			{
				MaxDepth = int.MaxValue
			});
			foreach (dynamic item in val2)
			{
				if ((bool)item["is_deprecated"])
				{
					continue;
				}
				string text2 = (string)item["name"];
				string author = (string)item["owner"];
				string fullName = "";
				string iconLink = "";
				string downloadLink = "";
				string version = "";
				string description = "";
				if (ThunderstoreModAssistant.Blacklist.Blacklist.IsBlacklisted(text2))
				{
					continue;
				}
				List<string> list = new List<string>();
				{
					IEnumerator enumerator2 = ((IEnumerable)item["versions"]).GetEnumerator();
					try
					{
						if (enumerator2.MoveNext())
						{
							dynamic current2 = enumerator2.Current;
							fullName = (string)current2["full_name"];
							iconLink = (string)current2["icon"];
							downloadLink = (string)current2["download_url"];
							version = (string)current2["version_number"];
							description = (string)current2["description"];
							foreach (dynamic item2 in current2["dependencies"])
							{
								list.Add((string)item2);
							}
						}
					}
					finally
					{
						IDisposable disposable = enumerator2 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				CodeModInfo value = new CodeModInfo
				{
					name = text2,
					fullName = fullName,
					iconLink = iconLink,
					dependencies = list,
					downloadLink = downloadLink,
					version = version,
					author = author,
					description = description
				};
				try
				{
					codeMods.Add(text2, value);
				}
				catch (Exception)
				{
				}
			}
		}

		public static CodeModInfo GetMostRecentCodeModFromName(string name)
		{
			CodeModInfo result = null;
			foreach (CodeModInfo value in codeMods.Values)
			{
				if (name == value.name)
				{
					result = value;
					break;
				}
			}
			return result;
		}

		public static CodeModInfo GetCodeModInfoFromFullName(string fullName)
		{
			string[] array = fullName.Split(new char[1] { '-' });
			string text = array[^1];
			string text2 = array[0];
			string text3 = fullName.Replace(text2 + "-", "").Replace("-" + text, "");
			foreach (CodeModInfo value in codeMods.Values)
			{
				if (value.name == text3)
				{
					return value;
				}
			}
			return null;
		}
	}
}
namespace ThunderstoreModAssistant.Utilities
{
	public class BundleUtilities
	{
		public static byte[] GetResourceBytes(string filename)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			foreach (string text in manifestResourceNames)
			{
				if (!text.Contains(filename))
				{
					continue;
				}
				using Stream stream = executingAssembly.GetManifestResourceStream(text);
				if (stream == null)
				{
					return null;
				}
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, array.Length);
				return array;
			}
			return null;
		}

		public static AssetBundle LoadBundleFromInternalAssembly(string filename)
		{
			return AssetBundle.LoadFromMemory(Il2CppStructArray<byte>.op_Implicit(GetResourceBytes(filename)));
		}
	}
	public static class AssetBundleExtension
	{
		public static T LoadPersistentAsset<T>(this AssetBundle bundle, string name) where T : Object
		{
			Object val = bundle.LoadAsset(name);
			if (val != (Object)null)
			{
				val.hideFlags = (HideFlags)32;
				return ((Il2CppObjectBase)val).TryCast<T>();
			}
			return default(T);
		}
	}
	public class KeyboardManager
	{
		public static string typed = "";

		public static void Append(string character)
		{
			typed += character;
		}

		public static void Backspace()
		{
			typed = typed.Substring(0, typed.Length - 1);
		}
	}
	public class MainThreadManager
	{
		private static ConcurrentQueue<GenericThreadingJob> genericThreadJobs = new ConcurrentQueue<GenericThreadingJob>();

		public static void HandleQueue()
		{
			if (genericThreadJobs.Count > 0 && genericThreadJobs.TryDequeue(out GenericThreadingJob result))
			{
				result.action();
			}
		}

		public static void QueueAction(Action action)
		{
			genericThreadJobs.Enqueue(new GenericThreadingJob
			{
				action = action
			});
		}
	}
	public class GenericThreadingJob
	{
		public Action action;
	}
	public class ThumbnailThreader
	{
		private static ConcurrentQueue<ThumbnailCompletionJob> thumbnailCompletionJobs = new ConcurrentQueue<ThumbnailCompletionJob>();

		public static void HandleQueue()
		{
			if (thumbnailCompletionJobs.Count > 0 && thumbnailCompletionJobs.TryDequeue(out ThumbnailCompletionJob result))
			{
				result.callback();
			}
		}

		public static void DownloadThumbnail(string url, Action<Texture> action)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			Action<Texture> action2 = action;
			UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
			webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerTexture(true);
			UnityWebRequestAsyncOperation val = webRequest.SendWebRequest();
			((AsyncOperation)val).m_completeCallback = ((AsyncOperation)val).m_completeCallback + Action<AsyncOperation>.op_Implicit((Action<AsyncOperation>)delegate
			{
				ThumbnailCompletionJob item = new ThumbnailCompletionJob
				{
					callback = delegate
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_000d: Invalid comparison between Unknown and I4
						if ((int)webRequest.result == 1)
						{
							DownloadHandlerTexture val2 = ((Il2CppObjectBase)webRequest.downloadHandler).TryCast<DownloadHandlerTexture>();
							Texture texture = (Texture)(object)val2.texture;
							action2(texture);
						}
					}
				};
				thumbnailCompletionJobs.Enqueue(item);
			});
		}
	}
	public class ThumbnailCompletionJob
	{
		public Action callback;
	}
	public class TimerManager
	{
		private static List<TimerDelayedAction> timerDelayedJobs = new List<TimerDelayedAction>();

		public static void Update()
		{
			foreach (TimerDelayedAction timerDelayedJob in timerDelayedJobs)
			{
				timerDelayedJob.time -= Time.deltaTime;
				if (timerDelayedJob.time <= 0f)
				{
					timerDelayedJob.onTimeOver();
					timerDelayedJob.completed = true;
				}
			}
			timerDelayedJobs.RemoveAll((TimerDelayedAction x) => x.completed);
		}

		public static void DelayAction(float time, Action onCompleted)
		{
			timerDelayedJobs.Add(new TimerDelayedAction
			{
				time = time,
				onTimeOver = onCompleted
			});
		}
	}
	public class TimerDelayedAction
	{
		public float time;

		public Action onTimeOver;

		public bool completed = false;
	}
	public class ZippingUtilities
	{
		public static void UnZipProperly(string zipPath, string exportDirectory)
		{
			using ZipArchive zipArchive = ZipFile.OpenRead(zipPath);
			foreach (ZipArchiveEntry entry in zipArchive.Entries)
			{
				string text = Path.Combine(exportDirectory, entry.FullName);
				if (entry.FullName.EndsWith("/"))
				{
					text = text.Substring(0, text.Length - 1);
					Directory.CreateDirectory(text);
					continue;
				}
				Directory.CreateDirectory(Path.GetDirectoryName(text));
				string fileName = Path.GetFileName(text);
				string text2 = Path.Combine(Path.GetDirectoryName(text), "tempExtractedFile.temp");
				entry.ExtractToFile(text2, overwrite: true);
				File.Move(text2, Path.Combine(Path.GetDirectoryName(text2), fileName));
			}
			zipArchive.Dispose();
		}
	}
}
namespace ThunderstoreModAssistant.Patches
{
	public class BonelibUIPatch
	{
		[HarmonyPatch(typeof(GUIMenu), "OnPageOpened")]
		public class CategoryUpdatePatch
		{
			private static GameObject activeUIObject;

			public static void Postfix(GUIMenu __instance, Page page)
			{
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				if (page == MainClass.mainCategory)
				{
					if (!Object.op_Implicit((Object)(object)activeUIObject))
					{
						GameObject val = Object.Instantiate<GameObject>(AssistantAssets.mainMenu);
						val.gameObject.AddComponent<AssistantMenuController>();
						val.transform.parent = ((Component)__instance).gameObject.transform;
						val.transform.localPosition = Vector3.forward + new Vector3(10f, 0f, 0f);
						val.transform.localRotation = Quaternion.identity;
						val.transform.localScale = Vector3.one;
						activeUIObject = val;
					}
					else
					{
						activeUIObject.SetActive(true);
					}
				}
				else if (page == Page.Root && Object.op_Implicit((Object)(object)activeUIObject))
				{
					activeUIObject.SetActive(false);
					AssistantMenuController.instance.Reset();
				}
			}
		}
	}
}
namespace ThunderstoreModAssistant.Install
{
	public class InstallManager
	{
		public static List<CodeModInfo> installQueue = new List<CodeModInfo>();

		public static CodeModInfo currentlyInstalling;

		public static UnityWebRequest activeDownloadWebRequest;

		public static void InstallMod(CodeModInfo codeModInfo, bool checkForUpdate = true)
		{
			if (codeModInfo == null || ThunderstoreModAssistant.Blacklist.Blacklist.IsBlacklisted(codeModInfo.name))
			{
				return;
			}
			foreach (CodeModInfo item in installQueue)
			{
				if (item.name == codeModInfo.name)
				{
					return;
				}
			}
			if (codeModInfo.IsInstalled() && checkForUpdate && !codeModInfo.HasUpdate())
			{
				return;
			}
			installQueue.Add(codeModInfo);
			foreach (string dependency in codeModInfo.dependencies)
			{
				CodeModInfo codeModInfoFromFullName = ThunderstoreParser.GetCodeModInfoFromFullName(dependency);
				if (codeModInfoFromFullName != null && !ThunderstoreModAssistant.Blacklist.Blacklist.IsDependencyBlacklisted(codeModInfoFromFullName.name))
				{
					InstallMod(codeModInfoFromFullName);
				}
			}
		}

		public static void DownloadFileUnityWeb(string url, string path)
		{
			DownloadFileHttpClient(url, path);
		}

		public static void DownloadUnityWeb(string url, string path)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			string path2 = path;
			UnityWebRequest httpWebRequest = UnityWebRequest.Get(url);
			httpWebRequest.method = "GET";
			httpWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			UnityWebRequestAsyncOperation val = httpWebRequest.SendWebRequest();
			((AsyncOperation)val).m_completeCallback = ((AsyncOperation)val).m_completeCallback + Action<AsyncOperation>.op_Implicit((Action<AsyncOperation>)delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Invalid comparison between Unknown and I4
				if ((int)httpWebRequest.result == 1)
				{
					DownloadHandlerBuffer val2 = ((Il2CppObjectBase)httpWebRequest.downloadHandler).TryCast<DownloadHandlerBuffer>();
					File.WriteAllBytes(path2, Il2CppArrayBase<byte>.op_Implicit((Il2CppArrayBase<byte>)(object)((DownloadHandler)val2).data));
				}
			});
		}

		public static async void DownloadFileHttpClient(string url, string path)
		{
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
				}
				catch (Exception ex)
				{
					MelonLogger.Error("Attempted to delete already existing file at: " + path + " but got exception: " + ex);
				}
			}
			using HttpClient client = new HttpClient(new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback = (HttpRequestMessage httpRequestMessage, X509Certificate2? cert, X509Chain? cetChain, SslPolicyErrors policyErrors) => true
			});
			using (HttpResponseMessage response = await client.GetAsync(url))
			{
				using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
				_ = response.Content.Headers.ContentLength.Value;
				long bytesRead = 0L;
				byte[] buffer = new byte[4096];
				using FileStream fs = new FileStream(path, FileMode.CreateNew);
				while (true)
				{
					int num;
					int bytesReceived = (num = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length));
					if (num <= 0)
					{
						break;
					}
					await fs.WriteAsync(buffer, 0, bytesReceived);
					bytesRead += bytesReceived;
				}
			}
			OnDownloadFileCompleted(null, null, path);
		}

		public static async Task DownloadFileAsync(string url, string path)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			HttpWebResponse response = (await request.GetResponseAsync()) as HttpWebResponse;
			if (response.StatusCode == HttpStatusCode.OK)
			{
				using (Stream stream = response.GetResponseStream())
				{
					_ = response.ContentLength;
					long bytesRead = 0L;
					byte[] buffer = new byte[4096];
					using FileStream fs = new FileStream(path, FileMode.CreateNew);
					while (true)
					{
						int num;
						int bytesReceived = (num = await stream.ReadAsync(buffer, 0, buffer.Length));
						if (num <= 0)
						{
							break;
						}
						await fs.WriteAsync(buffer, 0, bytesReceived);
						bytesRead += bytesReceived;
					}
				}
				OnDownloadFileCompleted(null, null, path);
			}
			else
			{
				MelonLogger.Error("Failed to download file");
			}
		}

		private static void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs args, string path)
		{
			string name = currentlyInstalling.name;
			string text = MainClass.MOD_DOWNLOADS_QUEUED + "/" + name;
			if (Directory.Exists(text))
			{
				Directory.Delete(text, recursive: true);
			}
			ZippingUtilities.UnZipProperly(path, text);
			File.Delete(path);
			WriteInstallToInstructions(name);
			string path2 = text + "/codeModInfo.json";
			string contents = JsonConvert.SerializeObject((object)currentlyInstalling);
			File.WriteAllText(path2, contents);
			CodeModInfo codeModInfo = null;
			foreach (CodeModInfo pendingUninstallCodeMod in MainClass.pendingUninstallCodeMods)
			{
				if (pendingUninstallCodeMod.name == currentlyInstalling.name)
				{
					codeModInfo = pendingUninstallCodeMod;
				}
			}
			if (codeModInfo != null)
			{
				MainClass.pendingUninstallCodeMods.Remove(codeModInfo);
				RemoveLineFromFile("uninstall " + name, MainClass.INSTRUCTIONS);
			}
			MainClass.pendingCodeMods.Add(currentlyInstalling);
			MainThreadManager.QueueAction(delegate
			{
				AssistantMenuController.UpdateDisplays();
				AssistantMenuController.restartRequired = true;
				AssistantMenuController.instance.UpdateRestartRequiredDisplay();
			});
			currentlyInstalling = null;
			activeDownloadWebRequest = null;
			MelonLogger.Msg("Finished installing " + name + "!");
		}

		public static void UninstallMod(CodeModInfo codeModInfo)
		{
			if (codeModInfo == null)
			{
				return;
			}
			CodeModInfo codeModInfo2 = null;
			foreach (CodeModInfo pendingCodeMod in MainClass.pendingCodeMods)
			{
				if (pendingCodeMod.name == codeModInfo.name)
				{
					codeModInfo2 = pendingCodeMod;
				}
			}
			if (codeModInfo2 != null)
			{
				MainClass.pendingCodeMods.Remove(codeModInfo2);
				RemoveLineFromFile("install " + codeModInfo.name, MainClass.INSTRUCTIONS);
				string path = MainClass.MOD_DOWNLOADS_QUEUED + "/" + codeModInfo.name;
				if (Directory.Exists(path))
				{
					Directory.Delete(path, recursive: true);
				}
			}
			else
			{
				WriteUnInstallToInstructions(codeModInfo.name);
				MainClass.pendingUninstallCodeMods.Add(codeModInfo);
				AssistantMenuController.restartRequired = true;
				AssistantMenuController.instance.UpdateRestartRequiredDisplay();
			}
			AssistantMenuController.UpdateDisplays();
		}

		public static void RemoveLineFromFile(string line, string directory)
		{
			string text = Path.Combine(Path.GetDirectoryName(directory), "temp.txt");
			using (StreamReader streamReader = new StreamReader(directory))
			{
				using StreamWriter streamWriter = new StreamWriter(text);
				string text2;
				while ((text2 = streamReader.ReadLine()) != null)
				{
					if (text2 != line)
					{
						streamWriter.WriteLine(text2);
					}
				}
			}
			File.Delete(directory);
			File.Move(text, directory);
		}

		private static void WriteInstallToInstructions(string name)
		{
			using StreamWriter streamWriter = new StreamWriter(MainClass.INSTRUCTIONS, append: true);
			streamWriter.WriteLine("install " + name);
		}

		public static void WriteUnInstallToInstructions(string name)
		{
			using StreamWriter streamWriter = new StreamWriter(MainClass.INSTRUCTIONS, append: true);
			streamWriter.WriteLine("uninstall " + name);
		}

		public static void HandleQueue()
		{
			if (currentlyInstalling == null && installQueue.Count > 0)
			{
				currentlyInstalling = installQueue[0];
				installQueue.RemoveAt(0);
				MelonLogger.Msg("Installing " + currentlyInstalling.name + "...");
				DownloadFileUnityWeb(currentlyInstalling.downloadLink, MainClass.MOD_ASSISTANT + "/temp.zip");
			}
		}
	}
}
namespace ThunderstoreModAssistant.Blacklist
{
	public class Blacklist
	{
		private static List<string> blacklistedMods = new List<string> { "MelonLoader", "r2modman" };

		private static List<string> dependencyBlacklist = new List<string> { "BoneLib", "ThunderstoreModAssistant" };

		public static bool IsBlacklisted(string name)
		{
			return blacklistedMods.Contains(name) || BlacklistedPlatformMods.IsMasterListBlacklisted(name);
		}

		public static bool IsDependencyBlacklisted(string name)
		{
			return dependencyBlacklist.Contains(name) || BlacklistedPlatformMods.IsMasterListBlacklisted(name);
		}

		public static bool IsBlacklistedAtAll(string name)
		{
			return IsBlacklisted(name) || IsDependencyBlacklisted(name);
		}
	}
	public class BlacklistedPlatformMods
	{
		public static string PC_BLACKLIST = "https://raw.githubusercontent.com/Minibattle/Thunderstore-Blacklist/refs/heads/main/PC.txt";

		public static string QUEST_BLACKLIST = "https://raw.githubusercontent.com/Minibattle/Thunderstore-Blacklist/refs/heads/main/Quest.txt";

		private static List<string> blacklistedMods = new List<string>();

		public static void LoadFromURL()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			string text = PC_BLACKLIST;
			if (HelperMethods.IsAndroid())
			{
				text = QUEST_BLACKLIST;
			}
			UnityWebRequest val = UnityWebRequest.Get(text);
			val.SendWebRequest();
			while (!val.isDone)
			{
			}
			if ((int)val.result != 2 && (int)val.result != 3)
			{
				string text2 = val.downloadHandler.text;
				string[] array = text2.Split('\n');
				string[] array2 = array;
				foreach (string item in array2)
				{
					blacklistedMods.Add(item);
				}
			}
		}

		public static bool IsMasterListBlacklisted(string name)
		{
			return blacklistedMods.Contains(name);
		}
	}
}
namespace ThunderstoreModAssistant.Behaviors
{
	[RegisterTypeInIl2Cpp]
	public class AssistantMenuController : MonoBehaviour
	{
		public GameObject gridView;

		public Animator animator;

		public int pageNumber;

		public int maxPages;

		public GameObject modInfoDisplay;

		public CanvasGroup mainGroup;

		public CanvasGroup modInfoGroup;

		private Button upArrowButton;

		private Button downArrowButton;

		private Button toThunderstoreButton;

		private Button toInstalledButton;

		private int maxDisplayPerPage = 4;

		public static AssistantMenuController instance;

		public CodeModInfo currentlySelectedInfo;

		public bool showingInstalled = false;

		public GameObject notificationTray;

		public GameObject sidePullout;

		public GameObject notificationBell;

		public GameObject notificationAlert;

		public GameObject keyboardPopup;

		private TMP_Text typeBarText;

		private GameObject typeBarTextObject;

		private GameObject typeBarEmptyTextObject;

		private GameObject typeBarObject;

		public bool notificationTrayPulledOut = false;

		public List<CodeModInfo> activeDisplayed = null;

		public string activeQuery = "";

		public static bool restartRequired;

		public GameObject restartRequiredObject;

		public AssistantMenuController(IntPtr intPtr)
			: base(intPtr)
		{
		}

		private void Awake()
		{
			activeDisplayed = ThunderstoreParser.codeMods.Values.ToList();
			ResetPages();
			instance = this;
			animator = ((Component)this).GetComponent<Animator>();
			animator.keepAnimatorControllerStateOnDisable = true;
			animator.keepAnimatorStateOnDisable = true;
			keyboardPopup = ((Component)((Component)this).transform.Find("KeyboardOverlay")).gameObject;
			typeBarObject = ((Component)keyboardPopup.transform.Find("TypeBar")).gameObject;
			typeBarTextObject = ((Component)typeBarObject.transform.Find("TypedOutText")).gameObject;
			typeBarEmptyTextObject = ((Component)typeBarObject.transform.Find("EmptyTextDisplay")).gameObject;
			typeBarText = typeBarTextObject.GetComponent<TMP_Text>();
			restartRequiredObject = ((Component)((Component)this).transform.Find("Main").Find("RestartPending")).gameObject;
			notificationTray = ((Component)((Component)this).transform.Find("SidePullout").Find("GridView")).gameObject;
			sidePullout = ((Component)((Component)this).transform.Find("SidePullout")).gameObject;
			notificationBell = ((Component)((Component)this).transform.Find("Main").Find("MainMask").Find("NotificationBell")).gameObject;
			notificationAlert = ((Component)notificationBell.transform.Find("Alert")).gameObject;
			((UnityEvent)((Component)((Component)this).transform.Find("Main").Find("MainMask").Find("SearchButton")).GetComponent<Button>().onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				PopoutKeyboard();
			}));
			((UnityEvent)((Component)notificationBell.transform.Find("Button")).GetComponent<Button>().onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				if (!notificationTrayPulledOut)
				{
					PullOutNotificationTray();
					notificationTrayPulledOut = true;
				}
				else
				{
					PullInNotificationTray();
					notificationTrayPulledOut = false;
				}
				PopulateUpdateRequiredMods();
			}));
			mainGroup = ((Component)((Component)this).transform.Find("Main")).GetComponent<CanvasGroup>();
			modInfoGroup = ((Component)((Component)this).transform.Find("ModDisplay")).GetComponent<CanvasGroup>();
			maxPages = (int)Math.Ceiling((double)ThunderstoreParser.codeMods.Count / (double)maxDisplayPerPage);
			gridView = ((Component)((Component)this).transform.Find("Main").Find("MainMask").Find("GridLayout")).gameObject;
			toThunderstoreButton = ((Component)((Component)this).transform.Find("Main").Find("MainMask").Find("ThunderstoreButton")).GetComponent<Button>();
			((UnityEvent)toThunderstoreButton.onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				SwitchToAllOfThunderstore();
			}));
			toInstalledButton = ((Component)((Component)this).transform.Find("Main").Find("MainMask").Find("InstallButton")).GetComponent<Button>();
			((UnityEvent)toInstalledButton.onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				if (activeQuery == "")
				{
					SwitchToInstalled();
				}
				else
				{
					SwitchToAllOfThunderstore();
					activeQuery = "";
					KeyboardManager.typed = "";
				}
			}));
			upArrowButton = ((Component)((Component)this).transform.Find("Main").Find("UpArrow").Find("Button")).GetComponent<Button>();
			((UnityEvent)upArrowButton.onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				OnArrowPress(up: true);
			}));
			downArrowButton = ((Component)((Component)this).transform.Find("Main").Find("DownArrow").Find("Button")).GetComponent<Button>();
			((UnityEvent)downArrowButton.onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				OnArrowPress(up: false);
			}));
			modInfoDisplay = ((Component)((Component)this).transform.Find("ModDisplay").Find("MainMask").Find("Holder")).gameObject;
			Button component = ((Component)modInfoDisplay.transform.Find("SafeExitOne")).GetComponent<Button>();
			((UnityEvent)component.onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				ReturnModInfo();
			}));
			Button component2 = ((Component)modInfoDisplay.transform.Find("SafeExitTwo")).GetComponent<Button>();
			((UnityEvent)component2.onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				ReturnModInfo();
			}));
			GameObject gameObject = ((Component)modInfoDisplay.transform.Find("InstallButton")).gameObject;
			((UnityEvent)((Component)gameObject.transform.Find("Button")).GetComponent<Button>().onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				InstallManager.InstallMod(currentlySelectedInfo);
			}));
			GameObject gameObject2 = ((Component)modInfoDisplay.transform.Find("UninstallButton")).gameObject;
			((UnityEvent)((Component)gameObject2.transform.Find("Button")).GetComponent<Button>().onClick).AddListener(UnityAction.op_Implicit((Action)delegate
			{
				InstallManager.UninstallMod(currentlySelectedInfo);
			}));
			PopulateGrid(pageNumber);
			UpdateArrowDisplays();
			notificationAlert.gameObject.SetActive(false);
			UpdateAlert();
			RegisterWholeKeyboard();
			UpdateRestartRequiredDisplay();
		}

		private void SwitchToInstalled()
		{
			showingInstalled = true;
			pageNumber = 0;
			activeDisplayed = GetInstalledCodeModList();
			PopulateGrid(pageNumber);
			((Component)toThunderstoreButton).gameObject.SetActive(true);
			((Component)toInstalledButton).gameObject.SetActive(false);
		}

		private void SwitchToAllOfThunderstore()
		{
			showingInstalled = false;
			pageNumber = 0;
			activeDisplayed = ThunderstoreParser.codeMods.Values.ToList();
			PopulateGrid(pageNumber);
			((Component)toThunderstoreButton).gameObject.SetActive(false);
			((Component)toInstalledButton).gameObject.SetActive(true);
		}

		private void PopoutKeyboard()
		{
			animator.SetTrigger("KeyboardPullOut");
			ModifyCanvasGroup(mainGroup, enable: false);
		}

		private void ReturnKeyboard()
		{
			animator.SetTrigger("KeyboardPullIn");
			ModifyCanvasGroup(mainGroup, enable: true);
		}

		private void RegisterWholeKeyboard()
		{
			RegisterKey("Q");
			RegisterKey("W");
			RegisterKey("E");
			RegisterKey("R");
			RegisterKey("T");
			RegisterKey("Y");
			RegisterKey("U");
			RegisterKey("I");
			RegisterKey("O");
			RegisterKey("P");
			RegisterKey("A");
			RegisterKey("S");
			RegisterKey("D");
			RegisterKey("F");
			RegisterKey("G");
			RegisterKey("H");
			RegisterKey("J");
			RegisterKey("K");
			RegisterKey("L");
			RegisterKey("Z");
			RegisterKey("X");
			RegisterKey("C");
			RegisterKey("V");
			RegisterKey("B");
			RegisterKey("N");
			RegisterKey("M");
			RegisterKey("1");
			RegisterKey("2");
			RegisterKey("3");
			RegisterKey("4");
			RegisterKey("5");
			RegisterKey("6");
			RegisterKey("7");
			RegisterKey("8");
			RegisterKey("9");
			RegisterKey("0");
			RegisterKey(".");
			RegisterKey(",");
			RegisterKey("'");
			RegisterKey("-");
			RegisterKey("=");
			SetKeyAction("Backspace", delegate
			{
				KeyboardManager.Backspace();
			});
			SetKeyAction("Space", delegate
			{
				KeyboardManager.Append(" ");
			});
			SetKeyAction("Enter", delegate
			{
				Search(KeyboardManager.typed);
			});
			SetKeyAction("Exit", delegate
			{
				ReturnKeyboard();
			});
		}

		private void Search(string query)
		{
			showingInstalled = false;
			((Component)toThunderstoreButton).gameObject.SetActive(false);
			((Component)toInstalledButton).gameObject.SetActive(true);
			activeQuery = query;
			List<CodeModInfo> list = new List<CodeModInfo>();
			if (query != "")
			{
				foreach (CodeModInfo value in ThunderstoreParser.codeMods.Values)
				{
					if (value.name.ToLower().Contains(query.ToLower()))
					{
						list.Add(value);
					}
				}
			}
			else
			{
				list = ThunderstoreParser.codeMods.Values.ToList();
			}
			ResetPages();
			activeDisplayed = list;
			PopulateGrid(pageNumber);
			ReturnKeyboard();
		}

		private List<CodeModInfo> GetInstalledCodeModList()
		{
			List<CodeModInfo> list = new List<CodeModInfo>();
			foreach (CodeModInfo value in ThunderstoreParser.codeMods.Values)
			{
				if (value.IsInstalled())
				{
					list.Add(value);
				}
			}
			return list;
		}

		private void Update()
		{
			if (Object.op_Implicit((Object)(object)keyboardPopup))
			{
				typeBarText.text = KeyboardManager.typed;
				if (KeyboardManager.typed == "")
				{
					typeBarTextObject.SetActive(false);
					typeBarEmptyTextObject.SetActive(true);
				}
				else
				{
					typeBarTextObject.SetActive(true);
					typeBarEmptyTextObject.SetActive(false);
				}
			}
		}

		public void UpdateRestartRequiredDisplay()
		{
			restartRequiredObject.SetActive(restartRequired);
		}

		private void RegisterKey(string keyName)
		{
			string keyName2 = keyName;
			SetKeyAction(keyName2, delegate
			{
				KeyboardManager.Append(keyName2);
			});
		}

		private void SetKeyAction(string keyName, Action action)
		{
			GameObject gameObject = ((Component)keyboardPopup.transform.Find("Keyboard").Find(keyName)).gameObject;
			Button component = gameObject.GetComponent<Button>();
			((UnityEvent)component.onClick).AddListener(UnityAction.op_Implicit(action));
		}

		private void RefreshUpdates()
		{
			UpdateAlert();
			PopulateUpdateRequiredMods();
		}

		private void UpdateAlert()
		{
			int modsToUpdateCount = GetModsToUpdateCount();
			if (modsToUpdateCount > 0)
			{
				notificationAlert.gameObject.SetActive(true);
				((Component)notificationAlert.transform.Find("Count")).GetComponent<TMP_Text>().text = modsToUpdateCount.ToString() ?? "";
			}
		}

		public static int GetModsToUpdateCount()
		{
			int num = 0;
			foreach (CodeModInfo installedCodeMod in MainClass.installedCodeMods)
			{
				if (installedCodeMod.HasUpdate() && !installedCodeMod.IsPending())
				{
					num++;
				}
			}
			return num;
		}

		public void PopulateUpdateRequiredMods()
		{
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			int childCount = notificationTray.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = notificationTray.transform.GetChild(i);
				RawImage component = ((Component)((Component)child).transform.Find("Icon")).GetComponent<RawImage>();
				if (Object.op_Implicit((Object)(object)component))
				{
					Object.DestroyImmediate((Object)(object)component.texture);
				}
				Object.Destroy((Object)(object)((Component)child).gameObject);
			}
			foreach (CodeModInfo installedCodeMod in MainClass.installedCodeMods)
			{
				if (installedCodeMod.HasUpdate() && !installedCodeMod.IsPending())
				{
					CodeModInfo mostRecent = ThunderstoreParser.GetMostRecentCodeModFromName(installedCodeMod.name);
					GameObject val = Object.Instantiate<GameObject>(AssistantAssets.updateEntry);
					val.transform.parent = notificationTray.transform;
					val.transform.localPosition = Vector3.forward;
					val.transform.localRotation = Quaternion.identity;
					val.transform.localScale = Vector3.one;
					RawImage icon = ((Component)val.transform.Find("Icon")).GetComponent<RawImage>();
					TMP_Text component2 = ((Component)val.transform.Find("Title")).GetComponent<TMP_Text>();
					TMP_Text component3 = ((Component)val.transform.Find("Version")).GetComponent<TMP_Text>();
					component2.text = installedCodeMod.name;
					component3.text = $"({installedCodeMod.version}) > ({mostRecent.version})";
					ThumbnailThreader.DownloadThumbnail(installedCodeMod.iconLink, delegate(Texture texture)
					{
						icon.texture = texture;
					});
					Button component4 = ((Component)val.transform.Find("Button")).GetComponent<Button>();
					((UnityEvent)component4.onClick).AddListener(UnityAction.op_Implicit((Action)delegate
					{
						InstallManager.InstallMod(mostRecent, checkForUpdate: false);
					}));
				}
			}
		}

		private void ModifyCanvasGroup(CanvasGroup group, bool enable)
		{
			foreach (BoxCollider componentsInChild in ((Component)group).gameObject.GetComponentsInChildren<BoxCollider>(true))
			{
				((Collider)componentsInChild).enabled = enable;
			}
		}

		private void PullOutNotificationTray()
		{
			animator.SetTrigger("PopOutSidePanel");
		}

		private void PullInNotificationTray()
		{
			animator.SetTrigger("PopInSidePanel");
		}

		private void PullOutModInfo()
		{
			animator.SetTrigger("PopOutModDisplay");
			ModifyCanvasGroup(mainGroup, enable: false);
		}

		private void ReturnModInfo()
		{
			animator.SetTrigger("ReturnModDisplay");
			ModifyCanvasGroup(mainGroup, enable: true);
		}

		public static void UpdateDisplays()
		{
			if (instance.currentlySelectedInfo != null)
			{
				instance.UpdateModDisplay(instance.currentlySelectedInfo, redownloadThumbnail: false);
			}
			instance.RefreshUpdates();
		}

		public void UpdateModDisplay(CodeModInfo info, bool redownloadThumbnail = true)
		{
			currentlySelectedInfo = info;
			RawImage thumbnail = ((Component)modInfoDisplay.transform.Find("ModIcon")).GetComponent<RawImage>();
			if (redownloadThumbnail)
			{
				ThumbnailThreader.DownloadThumbnail(info.iconLink, delegate(Texture texture)
				{
					thumbnail.texture = texture;
				});
			}
			TMP_Text component = ((Component)modInfoDisplay.transform.Find("Title")).GetComponent<TMP_Text>();
			component.text = info.name;
			TMP_Text component2 = ((Component)modInfoDisplay.transform.Find("Author")).GetComponent<TMP_Text>();
			component2.text = "By " + info.author;
			TMP_Text component3 = ((Component)modInfoDisplay.transform.Find("Description")).GetComponent<TMP_Text>();
			component3.text = info.description;
			TMP_Text component4 = ((Component)modInfoDisplay.transform.Find("Version")).GetComponent<TMP_Text>();
			component4.text = info.version;
			GameObject gameObject = ((Component)modInfoDisplay.transform.Find("InstallButton")).gameObject;
			GameObject uninstallBar = ((Component)modInfoDisplay.transform.Find("UninstallButton")).gameObject;
			if (info.IsInstalled())
			{
				gameObject.SetActive(false);
				if (redownloadThumbnail)
				{
					uninstallBar.SetActive(true);
				}
				TimerManager.DelayAction(0.1f, delegate
				{
					uninstallBar.SetActive(true);
				});
			}
			else
			{
				gameObject.SetActive(true);
				if (redownloadThumbnail)
				{
					uninstallBar.SetActive(false);
				}
				TimerManager.DelayAction(0.1f, delegate
				{
					uninstallBar.SetActive(false);
				});
			}
		}

		private void ResetPages()
		{
			maxPages = 0;
			pageNumber = 0;
		}

		private void OnArrowPress(bool up)
		{
			if (!up)
			{
				pageNumber++;
			}
			else
			{
				pageNumber--;
			}
			if (pageNumber < 0)
			{
				pageNumber = 0;
			}
			if (pageNumber > maxPages)
			{
				pageNumber = maxPages;
			}
			PopulateGrid(pageNumber);
		}

		private void PopulateGrid(int page)
		{
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			maxPages = (int)Math.Ceiling((double)activeDisplayed.Count / (double)maxDisplayPerPage);
			int childCount = gridView.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = gridView.transform.GetChild(i);
				CodeModInfoDisplay componentInChildren = ((Component)child).GetComponentInChildren<CodeModInfoDisplay>();
				if (Object.op_Implicit((Object)(object)componentInChildren))
				{
					Object.DestroyImmediate((Object)(object)componentInChildren.thumbnailImage.texture);
				}
				Object.Destroy((Object)(object)((Component)child).gameObject);
			}
			int num = 0;
			int num2 = 0;
			int num3 = page * maxDisplayPerPage;
			if (page > 0)
			{
				num3++;
			}
			foreach (CodeModInfo modInfo in activeDisplayed)
			{
				num2++;
				if (num2 >= num3)
				{
					GameObject val = Object.Instantiate<GameObject>(AssistantAssets.modInformation);
					CodeModInfoDisplay codeModInfoDisplay = val.AddComponent<CodeModInfoDisplay>();
					codeModInfoDisplay.SetModInfo(modInfo);
					((UnityEvent)((Component)val.transform.Find("Button")).GetComponent<Button>().onClick).AddListener(UnityAction.op_Implicit((Action)delegate
					{
						PullOutModInfo();
						UpdateModDisplay(modInfo);
					}));
					val.transform.parent = gridView.transform;
					val.transform.localPosition = Vector3.forward;
					val.transform.localRotation = Quaternion.identity;
					val.transform.localScale = Vector3.one;
					num++;
					if (num >= maxDisplayPerPage)
					{
						break;
					}
				}
			}
			UpdateArrowDisplays();
		}

		public void Reset()
		{
		}

		private void UpdateArrowDisplays()
		{
			GameObject gameObject = ((Component)((Component)upArrowButton).transform.parent).gameObject;
			GameObject gameObject2 = ((Component)((Component)downArrowButton).transform.parent).gameObject;
			gameObject.SetActive(pageNumber > 0);
			gameObject2.SetActive(pageNumber < maxPages - 1);
		}
	}
	[RegisterTypeInIl2Cpp]
	public class CodeModInfoDisplay : MonoBehaviour
	{
		public RawImage thumbnailImage;

		public TMP_Text modName;

		public TMP_Text modAuthor;

		public TMP_Text modVersion;

		public CodeModInfo modInfo;

		public CodeModInfoDisplay(IntPtr intPtr)
			: base(intPtr)
		{
		}

		private void Awake()
		{
			thumbnailImage = ((Component)((Component)this).transform.Find("ModIcon")).GetComponent<RawImage>();
			modName = ((Component)((Component)this).transform.Find("Title")).GetComponent<TMP_Text>();
			modAuthor = ((Component)((Component)this).transform.Find("Author")).GetComponent<TMP_Text>();
			modVersion = ((Component)((Component)this).transform.Find("Version")).GetComponent<TMP_Text>();
		}

		public void SetModInfo(CodeModInfo modInfo)
		{
			this.modInfo = modInfo;
			modName.text = modInfo.name;
			modAuthor.text = "By " + modInfo.author;
			modVersion.text = modInfo.version;
			ThumbnailThreader.DownloadThumbnail(modInfo.iconLink, delegate(Texture texture)
			{
				if (Object.op_Implicit((Object)(object)thumbnailImage))
				{
					thumbnailImage.texture = texture;
				}
			});
		}
	}
}