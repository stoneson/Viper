using Anno.Const;
using Anno.Const.Attribute;
using Anno.EngineData;
using Anno.Rpc;
using Anno.Rpc.Client;
using Anno.Util;
#if NETSTANDARD
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Anno.Plugs.DeployService
{
    public class DeployManagerModule : LogBaseModule
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[AnnoInfo(Desc = "AnnoDeploy集合")]
		public ActionResult GetDeployServices()
		{
			List<DeployAgent> list = new List<DeployAgent>();
			List<Micro> list2 = (from m in Connector.Micros
								 where m.Tags.Contains("Anno.Plugs.Deploy")
								 select m.Mi).ToList<Micro>();
			if (list2 != null && list2.Count > 0)
			{
				using (List<Micro>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Micro service = enumerator.Current;
						if (!list.Any((DeployAgent it) => it.Host == service.Ip && it.Port == service.Port))
						{
							list.Add(new DeployAgent
							{
								Host = service.Ip,
								Port = service.Port,
								Timeout = service.Timeout,
								Weight = service.Weight,
								Nickname = service.Nickname,
								Path = service.Path
							});
						}
					}
				}
			}
			return new ActionResult(true, list);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[AnnoInfo(Desc = "获取此Node上面的服务")]
		public ActionResult GetServices()
		{
			List<AnnoProcessInfo> annoProcesses = AnnoProcess.GetAnnoProcesses(CustomConfiguration.Settings["work_directory"]);
			return new ActionResult(true, annoProcesses);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workingName"></param>
		/// <returns></returns>
		[AnnoInfo(Desc = "获取此Node上面的服务")]
		public ActionResult GetServiceByWorkingName(string workingName)
		{
			string baseWorking = CustomConfiguration.Settings["work_directory"];
			AnnoProcessInfo annoProcesseByWorkingDirectory = AnnoProcess.GetAnnoProcesseByWorkingDirectory(workingName, baseWorking);
			return new ActionResult(true, annoProcesseByWorkingDirectory);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workingName"></param>
		/// <returns></returns>
		[AnnoInfo(Desc = "根据工作目录停止服务")]
		[DeployAuthorization]
		public ActionResult StopServiceByWorkingDirectory(string workingName)
		{
			string baseWorking = CustomConfiguration.Settings["work_directory"];
			AnnoProcessInfo annoProcesseByWorkingDirectory = AnnoProcess.GetAnnoProcesseByWorkingDirectory(workingName, baseWorking);
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
				AnnoProcess.RemoveAnnoProcesseByWorkingDirectory(workingName, baseWorking);
				annoProcesseByWorkingDirectory.Running = false;
				annoProcesseByWorkingDirectory.Id = -1;
				AnnoProcess.WriteAnnoProcesseByWorkingDirectory(annoProcesseByWorkingDirectory, baseWorking);
				base.Info(string.Format("{0}-[{1}]-停止服务", workingName, annoProcesseByWorkingDirectory.Id), "AnnoDeploy");
				return new ActionResult(true);
			}
			base.Error(workingName + "-未找到指定的服务", "AnnoDeploy");
			return new ActionResult(false, null, null, "未找到指定的服务");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workingName"></param>
		/// <returns></returns>
		[AnnoInfo(Desc = "根据工作目录启动或重启服务")]
		[DeployAuthorization]
		public ActionResult StartServiceByWorkingDirectory(string workingName)
		{
			string text = CustomConfiguration.Settings["work_directory"];
			AnnoProcessInfo annoProcesseByWorkingDirectory = AnnoProcess.GetAnnoProcesseByWorkingDirectory(workingName, text);
			string text2 = "未找到指定的服务";
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
				AnnoProcess.RemoveAnnoProcesseByWorkingDirectory(workingName, text);
				try
				{
					BashResult bashResult;
					if (Bash.Native)
					{
						bashResult = Bash.Linux.Command(annoProcesseByWorkingDirectory.Cmd, Path.Combine(text, annoProcesseByWorkingDirectory.WorkingDirectory), true);
					}
					else
					{
						bashResult = Bash.Windows.StartProcess(annoProcesseByWorkingDirectory.Cmd, Path.Combine(text, annoProcesseByWorkingDirectory.WorkingDirectory));
					}
					if (bashResult.Id > 0)
					{
						annoProcesseByWorkingDirectory.Running = true;
						annoProcesseByWorkingDirectory.Id = bashResult.Id;
					}
					else
					{
						annoProcesseByWorkingDirectory.Running = false;
						text2 = bashResult.ErrorMsg;
					}
					AnnoProcess.WriteAnnoProcesseByWorkingDirectory(annoProcesseByWorkingDirectory, text);
				}
				catch (Exception ex)
				{
					text2 = ex.Message;
					AnnoProcess.WriteAnnoProcesseByWorkingDirectory(annoProcesseByWorkingDirectory, text);
				}
				base.Info(string.Format("{0}-[{1}]-启动或重启服务", workingName, (annoProcesseByWorkingDirectory != null) ? new int?(annoProcesseByWorkingDirectory.Id) : null), "AnnoDeploy");
				return new ActionResult(true, null, null, text2);
			}
			base.Error(workingName + "-" + text2, "AnnoDeploy");
			return new ActionResult(false, null, null, text2);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workingName"></param>
		/// <returns></returns>
		[AnnoInfo(Desc = "根据工作目录停止和移除服务")]
		[DeployAuthorization]
		public ActionResult StopAndRemoveServiceByWorkingDirectory(string workingName)
		{
			string text = CustomConfiguration.Settings["work_directory"];
			AnnoProcessInfo annoProcesseByWorkingDirectory = AnnoProcess.GetAnnoProcesseByWorkingDirectory(workingName, text);
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
				AnnoProcess.RemoveAnnoProcesseByWorkingDirectory(workingName, text);
				string path = Path.Combine(text, workingName);
				if (Directory.Exists(path))
				{
					new DirectoryInfo(path).Attributes = FileAttributes.Normal;
					Directory.Delete(path, true);
				}
				base.Info(string.Format("{0}-[{1}]-停止和移除服务", workingName, (annoProcesseByWorkingDirectory != null) ? new int?(annoProcesseByWorkingDirectory.Id) : null), "AnnoDeploy");
				return new ActionResult(true);
			}
			base.Error(workingName + "-未找到指定的服务", "AnnoDeploy");
			return new ActionResult(false, null, null, "未找到指定的服务");
		}
	}
}
