using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Anno;
using Anno.Const;
using Anno.Log;
using Anno.Util;

namespace AnnoDeploy
{
	public static class ServiceControl
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public static void Start(string[] args)
		{
			ServiceControl.InitConfig();
			string text = CustomConfiguration.Settings["work_directory"];
			string text2 = ArgsValue.GetValueByName("start", args);
			if (string.IsNullOrWhiteSpace(text2))
			{
				Log.WriteLine("!!! 没有指定程序名称", ConsoleColor.Yellow);
				return;
			}
			text2 = text2.Trim();
			AnnoProcessInfo annoProcesseByWorkingDirectory = AnnoProcess.GetAnnoProcesseByWorkingDirectory(text2, text);
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
				AnnoProcess.RemoveAnnoProcesseByWorkingDirectory(text2, text);
				string str = "未找到指定的服务";
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
						str = bashResult.ErrorMsg;
					}
					AnnoProcess.WriteAnnoProcesseByWorkingDirectory(annoProcesseByWorkingDirectory, text);
					if (annoProcesseByWorkingDirectory.Running)
					{
						Log.WriteLine(text2 + " 已启动！", ConsoleColor.White);
					}
					else
					{
						Log.WriteLine(text2 + " 启动失败！" + str, ConsoleColor.White);
					}
					ServiceControl.WriteLineProcessesInfo(new List<AnnoProcessInfo>
					{
						annoProcesseByWorkingDirectory
					});
					return;
				}
				catch (Exception ex)
				{
					str = ex.Message;
					AnnoProcess.WriteAnnoProcesseByWorkingDirectory(annoProcesseByWorkingDirectory, text);
					Log.WriteLine(text2 + " 启动失败！" + str, ConsoleColor.White);
					return;
				}
			}
			Log.WriteLine("!!! 没有找到程序 " + text2, ConsoleColor.Yellow);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public static void Stop(string[] args)
		{
			ServiceControl.InitConfig();
			string baseWorking = CustomConfiguration.Settings["work_directory"];
			string text = ArgsValue.GetValueByName("stop", args);
			if (string.IsNullOrWhiteSpace(text))
			{
				Log.WriteLine("!!! 没有指定程序名称", ConsoleColor.Yellow);
				return;
			}
			text = text.Trim();
			AnnoProcessInfo annoProcesseByWorkingDirectory = AnnoProcess.GetAnnoProcesseByWorkingDirectory(text, baseWorking);
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
				AnnoProcess.RemoveAnnoProcesseByWorkingDirectory(text, baseWorking);
				annoProcesseByWorkingDirectory.Running = false;
				annoProcesseByWorkingDirectory.Id = -1;
				AnnoProcess.WriteAnnoProcesseByWorkingDirectory(annoProcesseByWorkingDirectory, baseWorking);
				Log.WriteLine(text + " 已停止！", ConsoleColor.White);
				ServiceControl.WriteLineProcessesInfo(new List<AnnoProcessInfo>
				{
					annoProcesseByWorkingDirectory
				});
				return;
			}
			Log.WriteLine("!!! 没有找到程序 " + text, ConsoleColor.Yellow);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public static void Status(string[] args)
		{
			ServiceControl.InitConfig();
			ServiceControl.WriteLineProcessesInfo(AnnoProcess.GetAnnoProcesses(CustomConfiguration.Settings["work_directory"]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="annoProcesses"></param>
		private static void WriteLineProcessesInfo(List<AnnoProcessInfo> annoProcesses)
		{
			if (annoProcesses == null)
			{
				return;
			}
			foreach (AnnoProcessInfo annoProcessInfo in annoProcesses)
			{
				string text = string.Empty.PadRight(26);
				if (annoProcessInfo.Running && annoProcessInfo.Id > 0)
				{
					Process processById = Process.GetProcessById(annoProcessInfo.Id);
					if (processById != null && !processById.HasExited)
					{
						TimeSpan timeSpan = DateTime.Now - processById.StartTime;
						text = string.Format("uptime {0} days,{1}:{2}:{3},", new object[]
						{
							timeSpan.Days,
							timeSpan.Hours.ToString().PadLeft(2, '0'),
							timeSpan.Minutes.ToString().PadLeft(2, '0'),
							timeSpan.Seconds.ToString().PadLeft(2, '0')
						}).PadRight(26, ' ');
					}
				}
				Log.WriteLineNoDate(string.Concat(new string[]
				{
					annoProcessInfo.WorkingDirectory.PadRight(30, ' '),
					" ",
					annoProcessInfo.Running ? "UP".PadRight(5, ' ') : "DOWN".PadRight(5, ' '),
					" pid ",
					annoProcessInfo.Id.ToString().PadRight(5, ' '),
					",",
					text,
					annoProcessInfo.AnnoProcessDescription
				}));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private static void InitConfig()
		{
			SettingService.InitConfig();
		}
	}
}
