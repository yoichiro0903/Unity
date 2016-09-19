using UnityEngine;
using System.Collections;
using MathExtension;

namespace Botan {

public class ItemBase : MonoBehaviour {

	protected bool		is_visible  = true;
	protected bool		is_active   = true;
	public bool		is_can_drag = false;

	public string		group_name;
	
	public struct StateBit {

		public bool	trigger_on;

		public bool	previous;
		public bool	current;

		public void	update_trigger()
		{
			this.trigger_on = (!this.previous && this.current);
		}
	};

	public StateBit		focused;
	public StateBit		dragging;

	protected Vector2	grab_offset = Vector2.zero;

	public BotanRoot	root;

	// ================================================================ //

	public virtual void		reset()
	{
		this.focused.trigger_on = false;
		this.focused.previous   = false;
		this.focused.current    = false;
	}

	// 位置をセットする.
	public virtual void		setPosition(Vector2 position)
	{
	}

	// 位置をゲットする.
	public virtual Vector2	getPosition()
	{
		return(Vector2.zero);
	}

	// 表示/非表示をセットする.
	public virtual void		setVisible(bool is_visible)
	{
		this.is_visible = is_visible;

		if(!this.is_visible) {

			this.reset();
		}
	}

	public void		execute(bool is_focusable)
	{
		if(this.is_visible) {

			this.update_focuse_status(is_focusable);

			if(this.is_can_drag) {

				this.execute_dragging();
			}

			this.execute_entity();
		}
	}

	public virtual void		execute_entity()
	{

	}

	// フォーカス状態の更新.
	protected void	update_focuse_status(bool is_focusable)
	{
		this.focused.previous = this.focused.current;
		
		// ---------------------------------------------------------------- //
		// フォーカス（ロールオーバー）.

		Vector2		mouse_pos = this.root.input.mouse_position;

		mouse_pos = Sprite2DRoot.get().convertMousePosition(mouse_pos);

		this.focused.current = false;

		if(this.is_active) {

			if(is_focusable) {

				if(this.isContainPoint(mouse_pos)) {
	
					this.focused.current = true;	
				}
			}

		} else {

			// フォーカスが禁止にされる前からフォーカスしていた場合は、
			// フォーカスアウトするまでフォーカス状態を維持する.

			if(this.focused.previous) {

				if(is_focusable) {
	
					if(this.isContainPoint(mouse_pos)) {
		
						this.focused.current = true;	
					}
				}
			}
		}

		this.focused.update_trigger();
	}

	// ドラッグの実行.
	protected void		execute_dragging()
	{
		Vector2		mouse_pos = this.root.input.mouse_position;

		mouse_pos = Sprite2DRoot.get().convertMousePosition(mouse_pos);

		if(this.dragging.current) {

			if(!this.root.input.button.current) {

				this.dragging.current = false;
			}

		} else {

			if(this.focused.current) {

				if(this.root.input.button.current) {

					this.grab_offset = this.getPosition() - mouse_pos;

					this.dragging.current = true;
				}
			}
		}

		if(this.dragging.current) {

			this.setPosition(mouse_pos + this.grab_offset);
		}
	}

	// ポイントがアイテムの上にある？.
	public virtual bool		isContainPoint(Vector2 point)
	{
		return(false);
	}
}


} // namespace Botan}
