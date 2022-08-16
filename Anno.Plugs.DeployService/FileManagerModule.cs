using Anno.Const;
using Anno.Const.Attribute;
using Anno.EngineData;
using Anno.Rpc;
using Anno.Rpc.Client;
using Anno.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anno.Plugs.DeployService
{
    public class FileManagerModule : LogBaseModule
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="formData"></param>
		/// <returns></returns>
		[AnnoInfo(Desc = "上传文件到服务器")]
		public ActionResult UpLoadFile(UpLoadFormData formData)
		{
			if (!formData.deploySecret.Equals(CustomConfiguration.Settings["deploySecret"]))
			{
				UpLoadFormData formData2 = formData;
				base.Warn(((formData2 != null) ? formData2.workingDirectory : null) + "-部署口令错误", "UpLoadFile");
				return new ActionResult(false, null, null, "部署口令错误！");
			}
			string currenRroot = formData.workingDirectory;
			if (currenRroot.ToLower().Equals("proc"))
			{
				UpLoadFormData formData3 = formData;
				base.Warn(((formData3 != null) ? formData3.workingDirectory : null) + "-proc为保留名称，请更改你的WorkingDirectory", "UpLoadFile");
				return new ActionResult(false, null, null, "proc为保留名称，请更改你的WorkingDirectory！");
			}
			string baseRoot = CustomConfiguration.Settings["work_directory"];
			List<AnnoFile> annoFiles = this.GetAnnoFiles();
			if (annoFiles.Count > 0)
			{
				string pathRoot = this.GetRootName(annoFiles[0].FileName);
				if (string.IsNullOrEmpty(currenRroot))
				{
					currenRroot = pathRoot;
				}
				AnnoProcessInfo annoProcesseByWorkingDirectory = AnnoProcess.GetAnnoProcesseByWorkingDirectory(currenRroot, baseRoot);
				if (annoProcesseByWorkingDirectory != null)
				{
					if (Bash.Native)
					{
						Bash.Linux.KillProcessAndChildren(annoProcesseByWorkingDirectory.Id);
					}
					else
					{
						Bash.Windows.KillProcessAndChildren(annoProcesseByWorkingDirectory.Id);
					}
					AnnoProcess.RemoveAnnoProcesseByWorkingDirectory(currenRroot, baseRoot);
				}
				foreach (AnnoFile annoFile in annoFiles)
				{
					string text = Path.Combine(baseRoot, currenRroot, annoFile.FileName.Substring(pathRoot.Length + 1));
					string fileName = this.GetFileName(annoFile.FileName);
					string path = text.Substring(0, text.Length - fileName.Length - 1);
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
				}
				Parallel.ForEach<AnnoFile>(annoFiles, delegate (AnnoFile file)
				{
					string path2 = Path.Combine(baseRoot, currenRroot, file.FileName.Substring(pathRoot.Length + 1));
					if (File.Exists(path2))
					{
						File.Delete(path2);
					}
					using (FileStream fileStream = File.Create(path2))
					{
						fileStream.Write(file.Content, 0, file.Length);
					}
				});
			}
			else
			{
				AnnoProcessInfo annoProcesseByWorkingDirectory2 = AnnoProcess.GetAnnoProcesseByWorkingDirectory(currenRroot, baseRoot);
				if (annoProcesseByWorkingDirectory2 != null)
				{
					if (Bash.Native)
					{
						Bash.Linux.KillProcessAndChildren(annoProcesseByWorkingDirectory2.Id);
					}
					else
					{
						Bash.Windows.KillProcessAndChildren(annoProcesseByWorkingDirectory2.Id);
					}
					AnnoProcess.RemoveAnnoProcesseByWorkingDirectory(currenRroot, baseRoot);
				}
			}
			if (formData.autoStart.Equals("1"))
			{
#if NET40
				TaskEx.Run(delegate
#else
				Task.Run(delegate
#endif
				{
					BashResult bashResult;
					if (Bash.Native)
					{
						bashResult = Bash.Linux.Command(formData.cmd, Path.Combine(baseRoot, currenRroot), true);
					}
					else
					{
						bashResult = Bash.Windows.StartProcess(formData.cmd, Path.Combine(baseRoot, currenRroot));
					}
					if (bashResult != null)
					{
						AnnoProcess.WriteAnnoProcesseByWorkingDirectory(new AnnoProcessInfo
						{
							Cmd = formData.cmd,
							NodeName = SettingService.AppName,
							WorkingDirectory = currenRroot,
							Id = bashResult.Id,
							Running = true,
							AnnoProcessDescription = formData.annoProcessDescription
						}, baseRoot);
					}
				});
			}
			else
			{
				AnnoProcess.WriteAnnoProcesseByWorkingDirectory(new AnnoProcessInfo
				{
					Cmd = formData.cmd,
					NodeName = SettingService.AppName,
					WorkingDirectory = currenRroot,
					Id = -1,
					Running = false,
					AnnoProcessDescription = formData.annoProcessDescription
				}, baseRoot);
			}
			UpLoadFormData formData4 = formData;
			base.Info(((formData4 != null) ? formData4.workingDirectory : null) + "-上传成功", "UpLoadFile");
			return new ActionResult(true, null, null, "上传成功");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private List<AnnoFile> GetAnnoFiles()
		{
			ConcurrentQueue<AnnoFile> annoFiles = new ConcurrentQueue<AnnoFile>();
			Parallel.ForEach<KeyValuePair<string, string>>(base.Input, delegate (KeyValuePair<string, string> file)
			{
				if (file.Key.StartsWith("annoDeploy___"))
				{
					AnnoFile item = JsonHelper.DeserializeObjectByJson<AnnoFile>(file.Value);
					annoFiles.Enqueue(item);
				}
			});
			return annoFiles.ToList<AnnoFile>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileFullName"></param>
		/// <returns></returns>
		private string GetFileName(string fileFullName)
		{
			string result = null;
			if (!string.IsNullOrWhiteSpace(fileFullName))
			{
				int val = fileFullName.LastIndexOf("/");
				int val2 = fileFullName.LastIndexOf("\\");
				int num = Math.Max(val, val2) + 1;
				if (num < fileFullName.Length)
				{
					result = fileFullName.Substring(num);
				}
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uploadFileName"></param>
		/// <returns></returns>
		private string GetRootName(string uploadFileName)
		{
			string result = null;
			if (!string.IsNullOrWhiteSpace(uploadFileName))
			{
				int num = uploadFileName.IndexOf("/");
				int num2 = uploadFileName.IndexOf("\\");
				if (num < 0 && num2 < 0)
				{
					return result;
				}
				int num3 = Math.Min(num, num2);
				if (num3 <= 0 && num < 0)
				{
					num3 = num2;
				}
				else if (num3 <= 0 && num2 < 0)
				{
					num3 = num;
				}
				if (num3 > 0)
				{
					result = uploadFileName.Substring(0, num3);
				}
			}
			return result;
		}
	}
}
