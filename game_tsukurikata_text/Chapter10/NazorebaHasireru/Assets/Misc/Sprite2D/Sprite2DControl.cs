using UnityEngine;
using System.Collections;
using MathExtension;

// 2D スプライト.
public class Sprite2DControl : MonoBehaviour {
	
	protected Vector2		size = Vector2.zero;
	protected float			angle = 0.0f;

	public string	depth_layer = "";
	public float	depth_offset = 0.0f;			// 同一レイヤー内のデプス.

	protected Vector2	position = Vector2.zero;
	public Vector2		rotation_center = Vector2.zero;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
	}
	
	void	Update()
	{
	}

	// ================================================================ //

	// 位置をセットする.
	public void	setPosition(Vector2 position)
	{
		this.position = position;
		this.transform.localPosition = new Vector3(this.position.x, this.position.y, this.transform.localPosition.z);
	}

	// 位置をゲットする.
	public Vector2	getPosition()
	{
		//return(this.transform.localPosition.xy());
		return(this.position);
	}

	// デプスレイヤーをセットする.
	public void		setDepthLayer(string depth_layer)
	{
		this.depth_layer = depth_layer;

		if(!Sprite2DRoot.get().isHasLayer(this.depth_layer)) {

			Debug.LogError("Depth Layer \"" + depth_layer + "\" not exists.");
		}

		this.calc_depth();
	}

	// デプスのレイヤー内オフセットをセットする.
	public void		setDepthOffset(float offset)
	{
		offset = Mathf.Clamp(offset, 0.0f, 1.0f - float.Epsilon);

		this.depth_offset = offset;

		this.calc_depth();
	}

	// デプスのレイヤー内オフセットをゲットする.
	public float	getDepthOffset()
	{
		return(this.depth_offset);
	}

	// 奥行き値をセットする.
	protected void		calc_depth()
	{
		float	depth = Sprite2DRoot.get().depthLayerToFloat(this.depth_layer, depth_offset);

		Vector3		position = this.transform.localPosition;

		position.z = depth;

		this.transform.localPosition = position;
	}

	// 奥行き値をゲットする.
	public float	getDepth()
	{
		return(this.transform.localPosition.z);
	}

	// [degree] アングル（Z軸周りの回転）をセットする.
	public void	setAngle(float angle)
	{
		this.angle = angle;
#if true
		Vector2		center = this.rotation_center;
		center.x *= this.transform.lossyScale.x;
		center.y *= this.transform.lossyScale.y;

		this.transform.localPosition = this.position.to_vector3(this.transform.localPosition.z);
		this.transform.localRotation = Quaternion.identity;
		this.transform.Translate( center, Space.Self);
		this.transform.Rotate(Vector3.forward, this.angle);
		this.transform.Translate(-center, Space.Self);
#else
		this.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
#endif
	}

	// [degree] アングル（Z軸周りの回転）をゲットする.
	public float	getAngle()
	{
		return(this.angle);
	}

	// 回転の中心位置をセットする.

	public void	setRotationCenter(Vector2 center)
	{
		this.rotation_center = center;
	}

	// スケールをセットする.
	public void	setScale(Vector2 scale)
	{
		this.transform.localScale = new Vector3(scale.x, scale.y, 1.0f);
	}
	
	// サイズをセットする.
	public void		setSize(Vector2 size)
	{
		Sprite2DRoot.get().setSizeToSprite(this, size);
	}
	// サイズをゲットする.
	public Vector2 getSize()
	{
		return(this.size);
	}
	
	// 頂点カラーをセットする.
	public void		setVertexColor(Color color)
	{
		Sprite2DRoot.get().setVertexColorToSprite(this, color);
	}

	// 頂点カラーのアルファーをセットする.
	public void		setVertexAlpha(float alpha)
	{
		Sprite2DRoot.get().setVertexColorToSprite(this, new Color(1.0f, 1.0f, 1.0f, alpha));
	}

	// テクスチャーをセットする.
	public void		setTexture(Texture texture)
	{
		this.GetComponent<MeshRenderer>().material.mainTexture = texture;
	}
	// テクスチャーをセットする（サイズも変更）.
	public void		setTextureWithSize(Texture texture)
	{
		this.GetComponent<MeshRenderer>().material.mainTexture = texture;
		Sprite2DRoot.get().setSizeToSprite(this, new Vector2(texture.width, texture.height));
	}

	// テクスチャーをゲットする.
	public Texture	getTexture()
	{
		return(this.GetComponent<MeshRenderer>().material.mainTexture);
	}

	// マテリアルをセットする.
	public void		setMaterial(Material material)
	{
		this.GetComponent<MeshRenderer>().material = material;
	}
	// マテリアルをゲットする.
	public Material		getMaterial()
	{
		return(this.GetComponent<MeshRenderer>().material);
	}

	// 左右/上下反転する.
	public void		setFlip(bool horizontal, bool vertical)
	{
		Vector2		scale  = Vector2.one;
		Vector2		offset = Vector2.zero;

		if(horizontal) {

			scale.x  = -1.0f;
			offset.x = 1.0f;
		}
		if(vertical) {

			scale.y = -1.0f;
			offset.y = 1.0f;
		}

		this.GetComponent<MeshRenderer>().material.mainTextureScale  = scale;
		this.GetComponent<MeshRenderer>().material.mainTextureOffset = offset;
	}

	// ポイントがスプライトの上にある？.
	public bool		isContainPoint(Vector2 point)
	{
		bool	ret = false;

		Vector2		position = this.transform.localPosition.xy();
		Vector2		scale    = this.transform.localScale.xy();

		do {

			if(point.x < position.x - this.size.x/2.0f*scale.x || position.x + this.size.x/2.0f*scale.x < point.x) {

				break;
			}
			if(point.y < position.y - this.size.y/2.0f*scale.y || position.y + this.size.y/2.0f*scale.y < point.y) {

				break;
			}

			ret = true;

		} while(false);

		return(ret);
	}

	// 表示/非表示をセットする.
	public void		setVisible(bool is_visible)
	{
		this.GetComponent<MeshRenderer>().enabled = is_visible;
	}
	// 表示中？.
	public bool		isVisible()
	{
		return(this.GetComponent<MeshRenderer>().enabled);
	}

	// 頂点の位置をゲットする.
	public Vector3[]	getVertexPositions()
	{
		return(Sprite2DRoot.get().getVertexPositionsFromSprite(this));
	}

	// 頂点の位置をセットする(2D).
	public void		setVertexPositions(Vector2[] positions)
	{
		Vector3[]		positions_3d = new Vector3[positions.Length];

		for(int i = 0;i < positions.Length;i++) {

			positions_3d[i] = positions[i];
		}
		Sprite2DRoot.get().setVertexPositionsToSprite(this, positions_3d);
	}

	// 頂点の位置をセットする(3D).
	public void		setVertexPositions(Vector3[] positions)
	{
		Sprite2DRoot.get().setVertexPositionsToSprite(this, positions);
	}

	// UV をセットする.
	public void		setVertexUVs(Vector2[] uvs)
	{
		Sprite2DRoot.get().setVertexUVsToSprite(this, uvs);
	}

	// メッシュの頂点数をゲットする.
	public int		getDivCount()
	{
		return(this.div_count);
	}

	// 破棄する.
	public void		destroy()
	{
		GameObject.Destroy(this.gameObject);
	}

	// 親をセットする.
	public void		setParent(Sprite2DControl parent)
	{
		this.transform.parent = parent.transform;
	}

	// メッシュコライダーをつける.
	public void		createCollider()
	{
		this.gameObject.AddComponent<MeshCollider>();
	}

	// 画面内にいる？.
	public bool		isInScreen()
	{
		bool		ret = false;
		Vector2		viewport_size = Sprite2DRoot.get().viewport_size;
		Vector2		position = this.transform.localPosition.xy();

		do {

			float	w = viewport_size.x/2.0f + this.size.x/2.0f;
			float	h = viewport_size.y/2.0f + this.size.y/2.0f;

			if(position.x < -w || w < position.x) {

				break;
			}
			if(position.y < -h || h < position.y) {

				break;
			}

			ret = true;

		} while(false);

		return(ret);
	}

	// ================================================================ //
	// Sprite2DRoot よう.

	// サイズをセットする.
	public void	internalSetSize(Vector2 size)
	{
		this.size = size;
	}

	protected int	div_count = 2;

	public void	internalSetDivCount(int div_count)
	{
		this.div_count = div_count;
	}

}
