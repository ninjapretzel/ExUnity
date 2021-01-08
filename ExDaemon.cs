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

	public class ExDaemon : MonoBehaviour {

		public bool lightTheme = false;
		public string targetHost = "localhost";
		public int targetPort = 32055;

		public LogLevel consoleLogLevel = LogLevel.Info;
		public LogLevel fileLogLevel = LogLevel.Debug;
		public Client client;

		public bool retryConnection = false;

		public float time = 0;
		private ConcurrentQueue<Action> toRun = new ConcurrentQueue<Action>();
		public void RunOnMainThread(Action action) { toRun.Enqueue(action); }
		private string logfile = "oops.log";
		
		void Awake() {
			//DontDestroyOnLoad(gameObject);
			logfile = $"{DateTime.UtcNow.UnixTimestamp()}.log";
		}
	
		void Start() {
			Thread connector = new Thread(Connect);
			connector.Start();

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
		
		public void OnLog(LogInfo info) {
			if (info.level <= consoleLogLevel) {
				Debug.unityLogger.Log(info.tag, info.message.ReplaceMarkdown());
			}
			if (info.level <= fileLogLevel) {
				try {
					File.AppendAllText(logfile, $"{info.tag}: {info.message}\n");
				} catch (Exception) { }
			}
		}
		
		void Stop(object what = null) {
			Debug.Log("Stopping.");
			if (what != null) {
				Debug.Log(what);
			}
		}

		void OnEnable() {
			if (lightTheme) {
				Log.LEVEL_CODES[(int)LogLevel.Info] = "\\k";
			}

			Log.logHandler += OnLog;
		}

		void OnDisable() {
			Debug.Log("ExDaemon disabled. Forcing Disconnection.");
			client?.DisconnectSlave();
			Log.logHandler -= OnLog;
		}


		public void Connect() {
			while (true) {
			
				Socket tcp = null;
				try {
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

				} catch (Exception e) {
					Debug.LogWarning($"Error on connection: {e}");
					if (tcp != null) {
						tcp.Dispose();
					}
				} 
			
				if (!retryConnection) { break; }
			}
		}
	
	}


}
