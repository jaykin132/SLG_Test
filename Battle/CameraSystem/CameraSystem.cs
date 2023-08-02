using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ET.Battle
{
	public class CameraSystem : MonoBehaviour
	{
		public float ZoomSpeed		= 6f;
		public float RotateVSpeed	= 80f;
		public float RotateHSpeed	= 80f;
		public float Distance		= 10f;

		public void Init()
		{
			Camera.main.transform.SetParent(transform);
			Camera.main.transform.localPosition = Vector3.zero;
			Camera.main.transform.localEulerAngles = Vector3.zero;

			// 之後要改成讀取設定檔
			Camera.main.transform.Translate(Distance * Vector3.back);
			transform.localEulerAngles = new Vector3(60f, -40f, 0f);
		}

		public void MoveLeft()
		{
			transform.Translate(Time.deltaTime * ZoomSpeed * Vector3.left);
		}

		public void MoveRight()
		{
			transform.Translate(Time.deltaTime * ZoomSpeed * Vector3.right);
		}

		public void MoveForward()
		{
			var dir = Time.deltaTime * ZoomSpeed * transform.forward;
			dir.y = 0f;

			transform.Translate(dir, Space.World);
		}	

		public void MoveBack()
		{
			var dir = Time.deltaTime * ZoomSpeed * transform.forward;

			dir.y = 0f;
			dir = -dir;

			transform.Translate(dir, Space.World);
		}

		public async Task MoveTo(int cX, int cY, float duration = 0.2f, Action onEnd = null)
		{
			var coord = new Vector2Int(cX, cY);
			await MoveTo(coord, duration, onEnd);
		}

		public async Task MoveTo(Vector2Int coord, float duration = 0.2f, Action onEnd = null)
		{
			var grid = MapSystem.main.GetGridCenter(coord);
			await MoveTo(grid, duration, onEnd);
		}

		public async Task MoveTo(GridData grid, float duration = 0.2f, Action onEnd = null)
		{
			var target = grid.Center;
			await MoveTo(target, duration, onEnd);
		}

		public async Task MoveTo(Vector3 target, float duration = 0.2f, Action onEnd = null)
		{
			var src = transform.position;

			for (float t = 0f; t < duration; t += Time.deltaTime)
			{
				var p = Vector3.Lerp(src, target, t / duration);
				p.y = src.y;

				transform.position = p;
				await Task.Yield();
			}

			onEnd?.Invoke();
		}

		public void ZoomIn()
		{ // zoom by transform position

			Camera.main.transform.Translate(Time.deltaTime * ZoomSpeed * Vector3.forward);
			Distance = Vector3.Distance(Camera.main.transform.position, transform.position);
		}

		public void ZoomOut()
		{
			Camera.main.transform.Translate(Time.deltaTime * ZoomSpeed * Vector3.back);
			Distance = Vector3.Distance(Camera.main.transform.position, transform.position);
		}

		public void RotateLeft()
		{
			transform.Rotate(Vector3.up, Time.deltaTime * RotateHSpeed, Space.World);
		}

		public void RotateRight()
		{
			transform.Rotate(Vector3.up, -Time.deltaTime * RotateHSpeed, Space.World);
		}

		public void RotateUp()
		{
			transform.Rotate(Vector3.right, Time.deltaTime * RotateVSpeed);
		}

		public void RotateDown()
		{
			transform.Rotate(Vector3.right, -Time.deltaTime * RotateVSpeed);
		}

		private void Start()
		{
			
		}

		private void LateUpdate()
		{
			if (Input.GetKey(KeyCode.A))
				MoveLeft();
			if (Input.GetKey(KeyCode.D))
				MoveRight();
			if (Input.GetKey(KeyCode.W))
				MoveForward();
			if (Input.GetKey(KeyCode.S))
				MoveBack();

			if (Input.GetKey(KeyCode.LeftArrow))
				RotateLeft();
			if (Input.GetKey(KeyCode.RightArrow))
				RotateRight();
			if (Input.GetKey(KeyCode.UpArrow))
				RotateUp();
			if (Input.GetKey(KeyCode.DownArrow))
				RotateDown();

			if (Input.GetKey(KeyCode.PageUp))
				ZoomIn();
			if (Input.GetKey(KeyCode.PageDown))
				ZoomOut();
		}

		protected static CameraSystem _main = null;
		public static CameraSystem main
		{
			get
			{
				if (_main == null)
				{
					var go = new GameObject("CameraSystem");
					_main = go.AddComponent<CameraSystem>();
				}

				return _main;
			}
		}
	}
}