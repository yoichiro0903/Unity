using UnityEngine;
using System.Collections;
using MathExtension;

namespace Botan {

public class Button : ItemBase {

	public StateBit		pressed;

	public Sprite2DControl	sprite = null;

	protected float		scale_timer = 0.0f;


	public delegate	void	Func(string name);
	public Func		on_trigger_pressed = (name)=>{};

	protected struct PressingAction {

		public bool		is_active;
		public bool		press_down;
	};
	protected PressingAction	press_act;

	// ================================================================ //
	// MonoBehaviour からの継承.

	public Sprite2DControl	getSprite()
	{
		return(this.sprite);
	}

	public void		setActive(bool is_active)
	{
		this.is_active = is_active;

		if(this.is_active) {

			this.sprite.setVertexColor(Color.white);

		} else {

			this.sprite.setVertexColor(new Color(0.7f, 0.7f, 0.7f));
		}
	}

	void	Awake()
	{
		this.reset();
	}
	
	void	Start()
	{
	}
	
	void	Update()
	{
	}

	public override void		reset()
	{
		base.reset();

		this.pressed = this.focused;

		this.press_act.is_active = false;
		this.press_act.press_down = false;

		this.scale_timer = 0.0f;

		if(this.sprite != null) {

			this.sprite.setScale(Vector2.one);
		}
	}

	public override void	execute_entity()
	{
		// ---------------------------------------------------------------- //
		// クリック.

		this.pressed.previous = this.pressed.current;
		this.pressed.current  = false;
		
		if(this.root.input.button.trigger_on) {

			if(this.is_active) {

				if(this.focused.current) {
	
					this.pressed.current = true;
				}
			}
		}

		// 押した演出中は、押されている状態にする.
		if(this.press_act.is_active) {

			this.pressed.current = true;
		}

		this.pressed.update_trigger();

		if(this.pressed.trigger_on) {

			//this.on_trigger_pressed(this.name);

			this.press_act.is_active  = true;
			this.press_act.press_down = true;
		}

		// ---------------------------------------------------------------- //
		// 押された演出.

		if(this.press_act.is_active) {

			if(this.press_act.press_down) {

				// 最初の位置から押し込まれるまで.
				this.scale_timer -= Time.deltaTime;

				if(this.scale_timer <= 0.0f) {

					this.scale_timer = 0.0f;

					this.press_act.press_down = false;
				}

			} else {

				// 押し込まれたところから、元の位置にもどるまで.
				this.scale_timer += Time.deltaTime;

				if(this.scale_timer >= 0.1f) {

					this.scale_timer = 0.1f;
					this.press_act.is_active = false;

					this.on_trigger_pressed(this.name);
				}
			}

			this.sprite.setScale(Vector2.one*Mathf.Lerp(1.0f, 1.2f, this.scale_timer/0.1f));

		} else {

			if(this.focused.current) {
				
				this.scale_timer += Time.deltaTime;
				
			} else {
				
				this.scale_timer -= Time.deltaTime;
			}

			this.scale_timer = Mathf.Clamp(this.scale_timer, 0.0f, 0.1f);

			this.sprite.setScale(Vector2.one*Mathf.Lerp(1.0f, 1.2f, this.scale_timer/0.1f));
		}

		// ---------------------------------------------------------------- //

		// フォーカス中のボタンが一番手前にくるように.
		if(this.focused.current) {

			this.sprite.setDepthOffset(BotanRoot.FORCUSED_BUTTON_DEPTH);

		} else {

			this.sprite.setDepthOffset(0.0f);
		}
	}

	// ================================================================ //

	// 生成する.
	public void		create(Texture texture, Vector2 size)
	{
		this.sprite = Sprite2DRoot.get().createSprite(texture, 2, true);
		this.sprite.setSize(size);
		this.sprite.setDepthLayer("ui.item");
	}	

	// 位置をセットする.
	public override void		setPosition(Vector2 position)
	{
		this.sprite.setPosition(position);
	}

	// 表示/非表示をセットする.
	public override void		setVisible(bool is_visible)
	{
		base.setVisible(is_visible);

		this.sprite.setVisible(this.is_visible);
	}

	// ポイントがアイテムの上にある？.
	public override bool		isContainPoint(Vector2 point)
	{
		return(this.sprite.isContainPoint(point));
	}

}

} // namespace Botan
