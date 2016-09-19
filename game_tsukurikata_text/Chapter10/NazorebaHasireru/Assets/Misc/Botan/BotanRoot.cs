using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotanRoot : MonoBehaviour {

	public List<Botan.Group>	groups;

	protected Botan.Group		default_group;

	public static float	FORCUSED_BUTTON_DEPTH = 0.1f;

	public Botan.InputData	input;

	public GameObject		fader_prefab;

	public Texture2D		texture_white8x8;
	public Texture2D		texture_waku_kado;
	public Texture2D		texture_waku_ue;

	// ================================================================ //
	// MonoBehaviour からの継承.
	
	void	Awake()
	{
		this.groups = new List<Botan.Group>();

		this.default_group = new Botan.Group();

		this.default_group.name = "@default";

		this.groups.Add(this.default_group);

		this.input.enable = true;
		this.input.mouse_position = Input.mousePosition;
		this.input.button.trigger_on = false;
		this.input.button.current    = false;

	}
	
	void	Start()
	{
	}

	public void		setInputEnable(bool is_enable)
	{
		this.input.enable = is_enable;
	}
	
	void	Update()
	{
		// ---------------------------------------------------------------- //
		// マウスの状態を調べておく.

		if(this.input.enable) {

			this.input.mouse_position    = Input.mousePosition;
			this.input.button.trigger_on = Input.GetMouseButtonDown(0);
			this.input.button.current    = Input.GetMouseButton(0);

		} else {

			this.input.button.trigger_on = false;
			this.input.button.current    = false;
		}

		// ---------------------------------------------------------------- //
		//

		foreach(Botan.Group group in this.groups) {

			if(!group.is_visible) {

				continue;
			}

			// ---------------------------------------------------------------- //
			// 前のフレームでフォーカスしていたボタンを最初に実行.

			Botan.ItemBase	next_focused = group.focused_item;

			if(group.focused_item != null) {

				group.focused_item.execute(true);

				if(!group.focused_item.focused.current) {

					next_focused = null;
				}
			}

			// 他のボタンを実行.

			foreach(Botan.ItemBase item in group.items) {

				if(item == group.focused_item) {

					continue;
				}

				item.execute(next_focused == null);

				if(next_focused == null) {

					if(item.focused.current) {

						next_focused = item;
					}
				}
			}

			group.focused_item = next_focused;
		}
	}

	// ================================================================ //

	// グループを作る.
	public Botan.Group	createGroup(string name)
	{
		Botan.Group		group = null;

		group = this.findGroup(name);

		if(group == null) {

			group      = new Botan.Group();
			group.name = name;

			this.groups.Add(group);
		}

		return(group);
	}

	// グループを探す.
	public Botan.Group	findGroup(string name)
	{
		return(this.groups.Find(x => x.name == name));
	}

	// ボタンをつくる.
	public Botan.Button	createButton(string name, Texture texture)
	{
		return(this.createButton(name, texture, new Vector2(texture.width, texture.height), ""));
	}

	// ボタンをつくる.
	public Botan.Button	createButton(string name, Texture texture, Vector2 size, string group_name)
	{
		GameObject		go = new GameObject();

		Botan.Button	button = go.AddComponent<Botan.Button>();

		button.create(texture, size);

		// グループに登録する.
		this.resist_item_to_group(group_name, button);

		button.name = name;
		button.root = this;

		return(button);
	}
	
	public Botan.Waku	createWaku(Vector2 size)
	{
		GameObject		go = new GameObject();

		Botan.Waku	waku = go.AddComponent<Botan.Waku>();

		waku.root = this;
		waku.create(size);

		// グループに登録する.
		this.resist_item_to_group("", waku);

		waku.name = name;
		waku.root = this;

		return(waku);
	}
	
	public Botan.TextView	createTextView(Font font, Vector2 window_size, int font_size)
	{
		GameObject		go = new GameObject();

		Botan.TextView	text_view = go.AddComponent<Botan.TextView>();

		text_view.create(window_size, font, font_size);

		return(text_view);
	}

	// グループに登録する.
	protected void	resist_item_to_group(string group_name, Botan.ItemBase item)
	{
		Botan.Group	group = this.findGroup(group_name);

		if(group == null) {

			group = this.default_group;
		}
		group.items.Add(item);

		item.group_name = group_name;
	}

	// ================================================================ //

	public Botan.Fader	createFader()
	{
		Botan.Fader	fader = (GameObject.Instantiate(this.fader_prefab) as GameObject).GetComponent<Botan.Fader>();

		return(fader);
	}

	// ================================================================ //
	// インスタンス.
	
	private	static BotanRoot	instance = null;
	
	public static BotanRoot	get()
	{
		if(BotanRoot.instance == null) {
		
			GameObject	go = GameObject.Find("BotanRoot");

			if(go != null) {

				BotanRoot.instance = go.GetComponent<BotanRoot>();

			} else {

				Debug.LogError("Can't find game object \"BotanRoot\".");
			}
		}
		
		return(BotanRoot.instance);
	}
	
}

