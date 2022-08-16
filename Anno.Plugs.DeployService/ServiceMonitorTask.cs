using Anno.Const;
using Anno.CronNET;
using Anno.Loader;
using Anno.Util;
using Anno.ViperLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Anno.Plugs.DeployService
{
    public class ServiceMonitorTask
	{
		/// <summary>
		/// 
		/// </summary>
		static ServiceMonitorTask()
		{
			ServiceMonitorTask.cron_daemon.AddJob("*/30 * * * * ? *", new ThreadStart(ServiceMonitorTask.MonitorTask));
		}

		/// <summary>
		/// 
		/// </summary>
		internal static void Start()
		{
			CronDaemon obj = ServiceMonitorTask.cron_daemon;
			lock (obj)
			{
				ServiceMonitorTask.cron_daemon.Start();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal static void Stop()
		{
			CronDaemon obj = ServiceMonitorTask.cron_daemon;
			lock (obj)
			{
				ServiceMonitorTask.cron_daemon.Stop();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private static void MonitorTask()
		{
			try
			{
				List<AnnoProcessInfo> annoProcesses = AnnoProcess.GetAnnoProcesses(CustomConfiguration.Settings["work_directory"]);
				if (annoProcesses != null && annoProcesses.Count > 0)
				{
					Parallel.ForEach<AnnoProcessInfo>(annoProcesses, delegate (AnnoProcessInfo proc)
					{
						if (proc != null && proc.Id > 0 && proc.ReStartErrorCount < 3 && !proc.Running)
						{
							ServiceMonitorTask.ReStartService(proc);
						}
					});
				}
			}
			catch (Exception exception)
			{
				try
				{
					IocLoader.Resolve<IViperLogService>().Error(new
					{
						Exception = exception
					}, "ServiceMonitorTask", "AnnoDeploy", "ServiceMonitorTask");
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ap"></param>
		private static void ReStartService(AnnoProcessInfo ap)
		{
			string text = CustomConfiguration.Settings["work_directory"];
			string workingDirectory = ap.WorkingDirectory;
			if (ap != null)
			{
				if (Bash.Native)
				{
					Bash.Linux.KillProcessAndChildren(ap.Id);
				}
				else
				{
					Bash.Windows.KillProcessAndChildren(ap.Id);
				}
				AnnoProcess.RemoveAnnoProcesseByWorkingDirectory(workingDirectory, text);
				try
				{
					BashResult bashResult;
					if (Bash.Native)
					{
						bashResult = Bash.Linux.Command(ap.Cmd, Path.Combine(text, ap.WorkingDirectory), true);
					}
					else
					{
						bashResult = Bash.Windows.StartProcess(ap.Cmd, Path.Combine(text, ap.WorkingDirectory));
					}
					if (bashResult.Id > 0)
					{
						Process processById = Process.GetProcessById(bashResult.Id);
						if (processById == null || processById.HasExited)
						{
							ap.Running = false;
							int reStartErrorCount = ap.ReStartErrorCount;
							ap.ReStartErrorCount = reStartErrorCount + 1;
							IocLoader.Resolve<IViperLogService>().Fatal("异常恢复失败-" + ap.WorkingDirectory, "ServiceMonitorTask", "AnnoDeploy", "ServiceMonitorTask");
						}
						else
						{
							ap.Running = true;
							ap.Id = bashResult.Id;
							ap.ReStartErrorCount = 0;
							IocLoader.Resolve<IViperLogService>().Fatal("异常恢复成功-" + ap.WorkingDirectory, "ServiceMonitorTask", "AnnoDeploy", "ServiceMonitorTask");
						}
					}
					AnnoProcess.WriteAnnoProcesseByWorkingDirectory(ap, text);
				}
				catch (Exception exception)
				{
					int reStartErrorCount = ap.ReStartErrorCount;
					ap.ReStartErrorCount = reStartErrorCount + 1;
					AnnoProcess.WriteAnnoProcesseByWorkingDirectory(ap, text);
					try
					{
						IocLoader.Resolve<ViperLog.IViperLogService>().Error(new
						{
							Appservice = ap,
							Exception = exception
						}, "ServiceMonitorTask", "AnnoDeploy", "ServiceMonitorTask");
					}
					catch
					{
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private static CronNET.CronDaemon cron_daemon = new CronNET.CronDaemon();
	}
}
