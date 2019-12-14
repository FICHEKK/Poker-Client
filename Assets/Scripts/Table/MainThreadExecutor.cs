using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Table {
	public sealed class MainThreadExecutor : MonoBehaviour {
		public static MainThreadExecutor Instance { get; private set; }
		private readonly ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();

		public void Awake() {
			if (Instance != null && Instance != this) {
				Destroy(gameObject);
			} else {
				Instance = this;
			}
		}
		
		public void Update() {
			while (actionQueue.Count > 0) {
				actionQueue.TryDequeue(out var action);
				action();
			}
		}

		public void Enqueue(Action action) {
			actionQueue.Enqueue(action);
		}
	}
}
