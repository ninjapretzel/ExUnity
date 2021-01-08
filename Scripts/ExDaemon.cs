using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Ex;
using ExClient.Utils;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace ExClient {
	/// <summary> Behaviour class used to begin connection to the server </summary>
	public class ExDaemon : MonoBehaviour {

		/// <summary> Toggle this on if you are using unity's light theme, to make logged items more readable. </summary>
		public bool lightTheme = false;
		/// <summary> Place to connect to </summary>
		public string targetHost = "localhost";
		/// <summary> Port to connect on </summary>
		public int targetPort = 32055;

		/// <summary> Log level for messages appearing in unity console </summary>
		public LogLevel consoleLogLevel = LogLevel.Info;
		/// <summary> Log level for messages sent to log files </summary>
		public LogLevel fileLogLevel = LogLevel.Debug;
		/// <summary> <see cref="Ex.Client"/> instance </summary>
		public Client client;

		/// <summary> If true, when disconnected, reconnection should be tried eagerly. </summary>
		public bool retryConnection = false;

		/// <summary> Actions to run on the main thread (◔_◔) </summary>
		private ConcurrentQueue<Action> toRun = new ConcurrentQueue<Action>();
		/// <summary> Helper function to marshal code back into Unity's main thread (◔_◔) </summary>
		public void RunOnMainThread(Action action) { toRun.Enqueue(action); }
		/// <summary> Current logfile name. Automatically set to timestamp upon <see cref="Awake"/>. </summary>
		private string logfile = "oops.log";
		/// <summary> Thread handling connection/reconnection </summary>
		private Thread connector;
		
		void Awake() {
			// Unfortunately, DontDestroyOnLoad is now really bad for performance. 凸(￣ヘ￣)
			// Scenes/prefabs should instead be loaded additavely as needed.
			//DontDestroyOnLoad(gameObject);
			logfile = $"{DateTime.UtcNow.UnixTimestamp()}.log";
		}
		
		void Start() {
			connector = new Thread(Connect);
			connector.Start();
		}

		void OnEnable() {
			if (lightTheme) {
				Log.LEVEL_CODES[(int)LogLevel.Info] = "\\k";
			}

			Log.logHandler += OnLog;
		}

		void OnDisable() {
			Debug.Log("ExDaemon disabled. Forcing Disconnection.");
			retryConnection = false;
			client?.DisconnectSlave();
			Log.logHandler -= OnLog;
		}
	
		void Update() {
			Action action;
			while (toRun.TryDequeue(out action)) {
				try {
					action();
				} catch (Exception e) {
					Debug.LogWarning($"Error running function on main thread {e.InfoString()}");
				}
			}
		}
		
		void OnLog(LogInfo info) {
			if (info.level <= consoleLogLevel) {
				Debug.unityLogger.Log(info.tag, info.message.ReplaceMarkdown());
			}
			if (info.level <= fileLogLevel) {
				try {
					File.AppendAllText(logfile, $"{info.tag}: {info.message}\n");
				} catch (Exception) { }
			}
		}
		
		void Connect() {
			Socket tcp = null;
			try {
				while (true) {
			
					// tcp = new TcpClient(targetHost, targetPort);
					tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					tcp.Connect(targetHost, targetPort);
					Client client = new Client(tcp);
		
					client.AddService<DebugService>();
					client.AddService<LoginService>();
					client.AddService<EntityService>();
					client.AddService<MapService>();
					client.AddService<SyncService>();


					//*
					// client.AddService<ExPlayerLink.>().Bind(this);
					client.AddService<ExEntityLink.ExEntityLinkService>().Bind(this);
					ExEntityLink.AutoRegisterComponentChangeCallbacks();
					// ExEntityLink.Register<Ex.Terrain>(TerrainGenerator)
					//*/

					client.ConnectSlave();
					this.client = client;
					RunOnMainThread(() => {
						BroadcastMessage("OnExConnected", SendMessageOptions.DontRequireReceiver);
					});

			
					if (!retryConnection) { break; }
				}
			} catch (Exception e) {
				Debug.LogWarning($"Error on connection: {e}");
				if (tcp != null) {
					tcp.Dispose();
				}
			} 
		}
		
	} 

}
