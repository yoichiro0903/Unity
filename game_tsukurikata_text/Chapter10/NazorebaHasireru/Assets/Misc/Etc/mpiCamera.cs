using UnityEngine;
using System.Collections;

public class mpiCamera : MonoBehaviour {
	
	public float		wheel_sense = 2.0f;		// ホイールの感度.
	public float		move_sense  = 0.01f;	// マウス移動の感度.
	public float		dolly_sense = 0.02f;	// ドリーのときのマウス移動の感度.
	
	public struct Config {
		
		public bool	is_controlable;
		public bool	is_use_alt_key;				// Alt キーを押してる間だけコントロールできる.
		public bool	is_enable_dolly_limit;
	};
	protected Config config;

	public struct Status {
		
		public bool	is_now_control;
	};
	protected Status	status;

	public struct Mouse {

		public struct Position {

			public Vector2	current;
			public Vector2	previous;
			public Vector2	move;
		};
		public Position	position;

		public struct Wheel {
			
			public float	delta;
		};
		public Wheel	wheel;
	};
	protected Mouse	mouse;

	public struct Posture {
		
		public Vector3	eye;
		public Vector3	interest;
		public Vector3	up;
	};
	protected Posture		posture;
	protected Posture		initial;					// resetPosture() したときの姿勢.

	protected float	dolly_limit_near;
	protected float	dolly_limit_far;
	
	// ================================================================ //

	void	Awake()
	{
		this.config.is_controlable        = true;
		this.config.is_use_alt_key        = true;
		this.config.is_enable_dolly_limit = false;
		
		this.status.is_now_control = false;

		this.posture.eye      = this.transform.position;
		this.posture.interest = this.transform.TransformPoint(Vector3.forward*10.0f);
		this.posture.up       = this.transform.up;

		this.dolly_limit_near = 1.0f;
		this.dolly_limit_far  = 100.0f;

		this.initial = this.posture;
	}

	void	Start()
	{
	}
	
	void	Update()
	{
		if(this.config.is_controlable) {

			this.update_entity();

			// カーソルの位置にあるオブジェクトに注視点を移動する.
			if(Input.GetKeyDown(KeyCode.F)) {

				do {

					Ray			ray = this.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

					RaycastHit	hit;

					if(!Physics.Raycast(ray, out hit)) {

						break;
					}

					float		depth = this.transform.InverseTransformPoint(hit.point).z;
					
					Vector3		eye_vector = this.posture.interest - this.posture.eye;

					eye_vector.Normalize();

					if(eye_vector.magnitude == 0.0f) {

						break;
					}

					eye_vector = this.transform.InverseTransformDirection(eye_vector);
					eye_vector *= depth/Mathf.Abs(eye_vector.z);
					eye_vector = this.transform.TransformDirection(eye_vector);

					this.posture.interest = hit.point;
					this.posture.eye = this.posture.interest - eye_vector;

					this.update_transform();

				} while(false);
			}
		}
	}

	// ================================================================ //

	// カメラの姿勢を取得する.
	public Posture	getPosture()
	{
		return(this.posture);
	}

	// カメラの姿勢をセットする.
	public void		setPosture(Posture posture)
	{
		this.posture = posture;

		this.update_transform();			
	}

	// 注視点が指定の場所になるよう、平行移動する.
	public void		parallelInterestTo(Vector3 interest)
	{
		this.posture.eye += interest - this.posture.interest;
		this.posture.interest = interest;

		this.update_transform();			
	}

	// 注視点からの距離が指定の値になるよう、視点を前後させる.
	public void		dolly(float distance)
	{
		Vector3		eye_vector = this.posture.eye - this.posture.interest;

		eye_vector.Normalize();

		if(eye_vector.magnitude == 0.0f) {

			eye_vector = -this.transform.forward;
		}

		this.posture.eye = this.posture.interest + eye_vector*distance;

		this.update_transform();			
	}

	// ================================================================ //

	protected void	update_entity() 
	{
		float			dx, dy;
		Vector3			move;
		float			length;
		bool			is_updated;
		
		this.mouse.position.previous = this.mouse.position.current;
		this.mouse.position.current  = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		
		this.mouse.wheel.delta = Input.GetAxisRaw("Mouse ScrollWheel");

		float	depth_revise = Vector3.Distance(this.posture.interest, this.posture.eye);

		depth_revise = Mathf.Max(0.0f, depth_revise);
		
		if(!this.status.is_now_control) {
			
			do {
				
				if(this.config.is_use_alt_key) {
					
					if(!Input.GetKey(KeyCode.LeftAlt)) {
						
						break;
					}
				}
				
				//
				
				this.status.is_now_control = true;
				
				if(Input.GetMouseButtonDown(0)) {
					
					break;
				}
				if(Input.GetMouseButtonDown(2)) {
					
					break;
				}
				if(Input.GetMouseButtonDown(1)) {
				
					break;
				}
				
				this.status.is_now_control = false;
				
			} while(false);

			if(this.status.is_now_control) {

				this.mouse.position.previous = this.mouse.position.current;
			}

		} else {
			
			do {
				
				if(Input.GetMouseButton(0)) {
					
					break;
				}
				if(Input.GetMouseButton(2)) {
					
					break;
				}
				if(Input.GetMouseButton(1)) {
					
					break;
				}
				
				this.status.is_now_control = false;
				
			} while(false);
			
			if(this.config.is_use_alt_key) {
				
				if(!Input.GetKey(KeyCode.LeftAlt)) {
					
					this.status.is_now_control = false;
				}
			}
		}

		this.mouse.position.move = this.mouse.position.current - this.mouse.position.previous;
		
		//
		
		do {
			
			is_updated = false;
			
			//

			if(mouse.wheel.delta != 0) {
				
				// 注視点に近づく／遠ざかる.

				this.calc_dolly(-mouse.wheel.delta*this.wheel_sense*depth_revise);
				
				is_updated = true;
			}
			
			//
			
			if(!this.status.is_now_control) {
				
				break;
			}
			
			dx = this.mouse.position.move.x;
			dy = this.mouse.position.move.y;

			if(Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl)) {

				// 注視点を中心に回転.
				
				this.rotate_around_interest(dx, -dy);
				
			} else if(Input.GetMouseButton(2) || (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))) {
				
				// 平行移動.

				move.x = -dx*this.move_sense;
				move.y = -dy*this.move_sense;
				move.z = 0.0f;
				move = this.transform.TransformVector(move);
	
				this.posture.interest += move;
				this.posture.eye      += move;
			
			} else if(Input.GetMouseButton(1)) {
				
				// 注視点に近づく／遠ざかる.
						
				length = 0.0f;
				
				length += -dx*this.dolly_sense*depth_revise;
				length += -dy*this.dolly_sense*depth_revise;
				
				this.calc_dolly(length);
			
			} else {
				
				break;
			}
			
			is_updated = true;
			
		} while(false);
		
		if(is_updated) {

			this.update_transform();			
		}
	}

	protected void	update_transform()
	{
		this.transform.position = this.posture.eye;
		this.transform.LookAt(this.posture.interest, this.posture.up);
	}

	// ドリー　注視点に近づいたり、遠ざかったり.
	protected void	calc_dolly(float dolly)
	{
		Vector3		v;
		float		length;
		
		v = this.posture.eye - this.posture.interest;
		
		length = v.magnitude;
		
		length += dolly;

		if(this.config.is_enable_dolly_limit) {
			
			length = Mathf.Clamp(length, this.dolly_limit_near, this.dolly_limit_far);
			
		} else {
			
			length = Mathf.Max(this.dolly_limit_near, length);
		}
	
		v.Normalize();
		if(v.magnitude == 0.0f) {

			v = Vector3.forward;
		}
		v *= length;
		
		this.posture.eye = this.posture.interest + v;
	}

	// 注視点周りの回転.
	protected void	rotate_around_interest(float dx, float dy)
	{
		Vector3		eye_vector = this.posture.eye - this.posture.interest;

		eye_vector = Quaternion.AngleAxis(dy, this.transform.right)*eye_vector;
		eye_vector = Quaternion.AngleAxis(dx, Vector3.up)*eye_vector;

		this.posture.eye = this.posture.interest + eye_vector;

		this.posture.up = Quaternion.AngleAxis(dy,  this.transform.right)*this.posture.up;
		this.posture.up = Quaternion.AngleAxis(dx, Vector3.up)*this.posture.up;
		this.posture.up.Normalize();
	}

	// ================================================================ //
}
