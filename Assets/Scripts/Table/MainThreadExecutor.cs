using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Table {
	
	/// <summary>
	/// A simple consumer object that performs all of the actions queued
	/// by other threads. These actions will be performed on the main thread.
	/// Newly added actions are performed on the next frame (the next Unity's
	/// "Update" method call).
	/// </summary>
	public sealed class MainThreadExecutor : MonoBehaviour {
		
		/// <summary>
		/// The singleton instance.
		/// </summary>
		public static MainThreadExecutor Instance { get; private set; }
		
		/// <summary>
		/// The queue of actions to be performed.
		/// </summary>
		private readonly ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();

		/// <summary>
		/// Initialize the singleton.
		/// </summary>
		private void Awake() {
			if (Instance != null && Instance != this) {
				Destroy(gameObject);
			}
			else {
				Instance = this;
			}
		}
		
		/// <summary>
		/// Performs all of the actions that were queued since the last frame.
		/// </summary>
		private void Update() {
			while (!actionQueue.IsEmpty) {
				if (actionQueue.TryDequeue(out var action)) {
					action();
				}
			}
		}

		/// <summary>
		/// Adds a new action to the queue. This action will be performed on
		/// the next frame.
		/// </summary>
		/// <param name="action">The action to be queued.</param>
		public void Enqueue(Action action) {
			actionQueue.Enqueue(action);
		}
	}
}